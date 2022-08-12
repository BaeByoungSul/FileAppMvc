using FileAppMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileAppMvc.Controllers
{
    
    
    [RequestSizeLimit(150 * 1024 * 1024)]       //unit is bytes => 150Mb
    [RequestFormLimits(MultipartBodyLengthLimit = 150 * 1024 * 1024)]
    public class FileServiceController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _fileDirectoryName = "Files";

        public FileServiceController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult FileUpload()
        {
            return View();
        }
        public IActionResult FileDownload()
        {
            string serverDirectory = Path.Combine(_env.WebRootPath, _fileDirectoryName);
            string[] filePaths = Directory.GetFiles(serverDirectory);

            List<FileViewModel> fileList = new List<FileViewModel>();
            foreach (string filepath in filePaths)
            {
                fileList.Add(new FileViewModel { FileName = Path.GetFileName(filepath) });
            }
            return View(fileList);
        }


        public FileResult Downloadfile(string fileName)
        {
            var filePath = Path.Combine(_env.WebRootPath, _fileDirectoryName, fileName);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);

            return File(bytes, "application/octet-stream", fileName);
        }


        [HttpPost]
        public async Task<IActionResult> ActionFileUpload(IFormFile file)
        {
            if (file == null) 
                TempData["msg"] = "File Upload fail ( null )";
            else
            {
                bool rtn = await UploadFile(file);
                if (rtn) TempData["msg"] = "File Uploaded sccessfully";
                else TempData["msg"] = "File Upload fail";

            }

            return View("FileUpload");

        }

        public async Task<bool> UploadFile(IFormFile file)
        {
            bool isCopied = false;
            try
            {
                if (file.Length > 0)
                {
                    //var filePath = Path.Combine(_env.ContentRootPath, "UploadFiles", file.FileName);
                    var filePath = Path.Combine(_env.WebRootPath, _fileDirectoryName, file.FileName);

                    // 파일이 있어면 새로 생성함
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);

                    isCopied = true;
                }
                else
                    isCopied = false;
            }
            catch (Exception)
            {
                throw;
            }
            return isCopied;
        }

    }
}
