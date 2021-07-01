using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GitInitTest.Common.Helpers
{
    public interface IImageWriter
    {
        Task<string> UploadImageAsync(IFormFile file, string savePath = null, string newFilename = null);
    }

    public class ImageWriter : IImageWriter
    {
        public async Task<string> UploadImageAsync(IFormFile file, string savePath = null, string newFilename = null)
        {
            if (CheckIfImageFile(file))
            {
                return await WriteFileAsync(file, savePath, newFilename);
            }

            return "Invalid image file";
        }

        /// <summary>
        /// Method to check if file is image file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckIfImageFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return ImageFormatHelper.GetImageFormat(fileBytes) != ImageFormatHelper.ImageFormat.unknown;
        }

        /// <summary>
        /// Method to write file onto the disk
        /// </summary>
        /// <param name="file"></param>
        /// <param name="savePath"></param>
        /// <param name="newFilename"></param>
        /// <returns></returns>
        public async Task<string> WriteFileAsync(IFormFile file, string savePath = null, string newFilename = null)
        {
            string fileName;
            string path;
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];

                if (newFilename == null) fileName = Guid.NewGuid().ToString() + extension; //Create a new Name
                else fileName = newFilename + extension;

                //for the file due to security reasons.
                if (savePath == null) path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                else path = Path.Combine(savePath, fileName);

                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return fileName;
        }
    }
}