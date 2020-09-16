using System.Threading.Tasks;
using aspnetcore_storagemanager.Data;
using Microsoft.AspNetCore.Http;

namespace aspnetcore_storagemanager.Interfaces
{
    public interface IStorageManager
    {
        string[] AcceptedExtension { get; }
        Task<bool> DeleteAsync(string fileId);
        Task<byte[]> DownloadAsync(string fileId);
        Task<FileStorage> UploadAsync(IFormFile file);
    }
}