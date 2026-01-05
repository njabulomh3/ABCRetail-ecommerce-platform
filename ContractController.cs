using ABCRetail_Cloud_.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail_Cloud_.Controllers
{
    public class ContractController : Controller
    {
        private readonly FileShareService _fileShareService;

        public ContractController(FileShareService fileShareService)
        {
            _fileShareService = fileShareService;
        }

        public async Task<IActionResult> Index()
        {
            var files = await _fileShareService.ListFilesAsync();
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please select a file to upload.");
                return await Index();
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    string fileName = file.FileName;
                    await _fileShareService.UploadFileAsync(fileName, stream);
                }

                TempData["Message"] = $"File '{file.FileName}' uploaded successfully!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"File upload failed: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name cannot be null or empty.");
            }

            try
            {
                var fileStream = await _fileShareService.DownloadFileAsync(fileName);

                if (fileStream == null)
                {
                    return NotFound($"File '{fileName}' not found.");
                }

                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error downloading file: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            try
            {
                await _fileShareService.DeleteFileAsync(fileName);
                TempData["Message"] = $"File '{fileName}' deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"File deletion failed: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}

