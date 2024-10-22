// Services/UploadService.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace WebApplication1.Services
{

    public class UploadOnfile
    {
        [Required(ErrorMessage = "Chon file de upload")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "png,jpg,jpeg,gif")]
        [Display(Name = "chon file updload")]
        public IFormFile FileUpload { get; set; }
    }
    public class UploadService
    {
        //private readonly IWebHostEnvironment _webHostEnvironment;

        //public UploadService(IWebHostEnvironment webHostEnvironment)
        //{
        //    _webHostEnvironment = webHostEnvironment;
        //}

        //public async Task<string> UploadFileAsync(IFormFile file)
        //{
        //    if (file == null) return null;

        //    var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(file.FileName);
        //    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Image", fileName);

        //    using (var fileStream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(fileStream);
        //    }

        //    return fileName;
        //}

        // Services/UploadService.cs

        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file), "File không thể null");

            var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Image", fileName);

            // Kiểm tra xem thư mục có tồn tại hay không
            if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Image")))
            {
                Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "Image"));
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;
        }
    }
}
