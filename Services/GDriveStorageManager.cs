using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using aspnetcore_storagemanager.Data;
using aspnetcore_storagemanager.Interfaces;
using aspnetcore_storagemanager.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace aspnetcore_storagemanager.Services
{
    public class GDriveStorageManager : IStorageManager
    {
        private readonly GDriveOption _option;
        private readonly DriveService _service;

        public GDriveStorageManager(IConfiguration configuration,
                                    IWebHostEnvironment environment)
        {
            _option = configuration.GetSection("GoogleDrive").Get<GDriveOption>();
            var fullPath = Path.Combine(environment.ContentRootPath, _option.CredentialPath);
            var credentialString = System.IO.File.ReadAllText(fullPath);
            var credential = GoogleCredential
            .FromJson(credentialString)
            .CreateScoped(new string[] {
                DriveService.Scope.Drive
            });
            _service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

        }

        public string[] AcceptedExtension { get; } = { "jpg", "pdf" };

        public async Task<bool> DeleteAsync(string fileId)
        {
            try
            {
                await _service.Files.Delete(fileId).ExecuteAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<byte[]> DownloadAsync(string fileId)
        {
            await using var stream = new MemoryStream();
            var request = _service.Files.Get(fileId);
            await request.DownloadAsync(stream);
            return stream.ToArray();
        }

        public async Task<FileStorage> UploadAsync(IFormFile file)
        {
            string extension = string.Empty;
            if (file.FileName.Contains("."))
            {
                extension = file.FileName.Split(".").Last();
            }
            if (string.IsNullOrEmpty(extension) || !AcceptedExtension.Contains(extension))
            {
                throw new Exception("Not supported extension file was uploaded.");
            }
            // await using var stream = new MemoryStream();
            // await file.CopyToAsync(stream);
            // var data = stream.ToArray();
            var fileName = Guid.NewGuid().ToString();
            var uploadRequest = _service.Files.Create(new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string> { _option.FolderId }
            }, file.OpenReadStream(), MimeTypes.GetMimeType(file.FileName));
            uploadRequest.Fields = "id,name,size,mimeType,createdTime";
            Google.Apis.Drive.v3.Data.File uploadedFile = null;
            while (uploadedFile == null)
            {
                try
                {
                    var status = await uploadRequest.UploadAsync();
                    uploadedFile = uploadRequest.ResponseBody;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return new FileStorage(uploadedFile.Id,
                                   file.FileName,
                                   MimeTypes.GetMimeType(file.FileName),
                                   file.Length,
                                   uploadedFile.CreatedTime.Value);

        }
    }
}