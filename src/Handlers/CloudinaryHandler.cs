using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace api_infor_cell.src.Handlers
{
    public class CloudinaryHandler(IWebHostEnvironment env)
    {
        private readonly string CloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL") ?? "";

        public async Task<string> UploadAttachment(string parent, IFormFile attachment)
        {
            string webRoot = Path.Combine(env.ContentRootPath, "wwwroot");

            string uploadPath = Path.Combine(webRoot, "uploads", parent);

            if (!Directory.Exists(uploadPath)) 
            {
                Directory.CreateDirectory(uploadPath);
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(attachment.FileName)}";
            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await attachment.CopyToAsync(stream);
            }

            return Path.Combine("uploads", parent, fileName);
        }
        
        public async Task<bool> Delete(string publicId, string folderProject, string folderModel)
        {
            Cloudinary cloudinary = new(CloudinaryUrl);
            cloudinary.Api.Secure = true;

            DeletionParams deletionParams = new ($"{folderProject}/{folderModel}/{publicId}");
            DeletionResult result = await cloudinary.DestroyAsync(deletionParams);

            return result.Result == "ok";
        }
    }
}