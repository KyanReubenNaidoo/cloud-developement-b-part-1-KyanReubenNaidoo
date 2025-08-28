using POECLDV6212.Models;
using POECLDV6212.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace POECLDV6212.Controllers
{
    public class FilesController : Controller
    {
        private readonly File_Service _file;

        public FilesController(File_Service file)
        {
            _file = file;
        }

        public async Task<IActionResult> Index()
        {
            List<FileModel> files;
            try
            {
                files = await _file.ListFilesAsync("uploads");
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Failed to load files : {ex.Message}";
                files = new List<FileModel>();
            }
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file to upload");
                return await Index();
            }
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    string directoryName = "uploads";
                    string fileName = file.FileName;
                    await _file.UploadFileAsync(directoryName, fileName, stream);
                }
                TempData["Message"] = $"File '{file.FileName}' uploaded successfully";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"File upload has failed : {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name cannot be null or empty");
            }
            try
            {
                var fileStream = await _file.DownloadFileAsync("uploads", fileName);
                if (fileStream == null)
                {
                    return NotFound($"File : {fileName}, was not found");
                }
                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error downloading file : {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                TempData["Message"] = "File name cannot be null or empty";
                return RedirectToAction("Index");
            }

            try
            {
                await _file.DeleteFileAsync("uploads", fileName);
                TempData["Message"] = $"File '{fileName}' deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Failed to delete file '{fileName}': {ex.Message}";
            }

            return RedirectToAction("Index");
        }

    }
}