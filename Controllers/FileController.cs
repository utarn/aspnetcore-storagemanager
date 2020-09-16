using System.Collections.Generic;
using System.Threading.Tasks;
using aspnetcore_storagemanager.Data;
using aspnetcore_storagemanager.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnetcore_storagemanager.Controllers
{
    public class FileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageManager _storageManager;

        public FileController(ApplicationDbContext context,
                              IStorageManager storageManager)
        {
            _context = context;
            _storageManager = storageManager;

        }

        public async Task<IActionResult> Index()
        {
            var files = await _context.FileStorages.ToListAsync();

            var supportedType = new List<string>();
            foreach (var item in _storageManager.AcceptedExtension)
            {
                supportedType.Add(MimeTypes.GetMimeType("a." + item));
            }
            ViewData["SupportedType"] = string.Join(',', supportedType);
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            foreach (var file in files)
            {
                var fileStroage = await _storageManager.UploadAsync(file);
                await _context.FileStorages.AddAsync(fileStroage);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Download(string id)
        {
            var storage = await _context.FileStorages.FindAsync(id);
            if (storage != null)
            {
                var data = await _storageManager.DownloadAsync(storage.FileId);
                return File(data, storage.FileType);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _storageManager.DeleteAsync(id);
            var toRemove = await _context.FileStorages.FindAsync(id);
            if (toRemove != null)
            {
                _context.FileStorages.Remove(toRemove);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}