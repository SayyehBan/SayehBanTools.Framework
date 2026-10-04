using Microsoft.AspNetCore.Http;
using SayehBanTools.Framework.Converter;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.ManageFile
{
    /// <summary>
    /// این کلاس برای مدیریت فایل‌ها استفاده می‌شود
    /// </summary>
    public class ManageFiles
    {
        private readonly IFileSystem _fileSystem;
        private const int DefaultChunkSize = 1024 * 1024; // 1 مگابایت

        /// <summary>
        /// سازنده کلاس سازگار با C# 7.3
        /// </summary>
        /// <param name="fileSystem">سیستم فایل اختیاری برای Unit Testing</param>
        public ManageFiles(IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
        }

        /// <summary>
        /// حذف فایل از سرور
        /// </summary>
        /// <param name="baseFilePath">مسیر فایل</param>
        public void DeleteFileServer(string baseFilePath)
        {
            if (_fileSystem.File.Exists(baseFilePath))
            {
                _fileSystem.File.Delete(baseFilePath);
            }
        }

        /// <summary>
        /// آپلود فایل به‌صورت کامل و async
        /// </summary>
        /// <param name="basePath">مسیر پایه</param>
        /// <param name="file">فایل ورودی</param>
        /// <returns>مسیر فایل آپلودشده</returns>
        public async Task<string> UploadFileAsync(string basePath, IFormFile file)
        {
            if (string.IsNullOrEmpty(basePath))
                throw new ArgumentException("مسیر پایه نمی‌تواند خالی باشد.", nameof(basePath));
            if (file == null)
                throw new ArgumentNullException(nameof(file), "فایل نمی‌تواند null باشد.");

            var fullPath = _fileSystem.Path.Combine(basePath, file.FileName);
            var directoryPath = _fileSystem.Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                _fileSystem.Directory.CreateDirectory(directoryPath);
            }

            var newFileName = Guid.NewGuid().ToString() + _fileSystem.Path.GetExtension(file.FileName);
            var newFullPath = _fileSystem.Path.Combine(basePath, newFileName);

            using (var stream = _fileSystem.FileStream.New(newFullPath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }

            var newdirect = StringExtensions.RemoveDirectWWWROOT(newFullPath);
            return newdirect ?? string.Empty;
        }

        /// <summary>
        /// آپلود فایل به‌صورت تکه‌ای (Chunked) و async
        /// </summary>
        /// <param name="basePath">مسیر پایه</param>
        /// <param name="file">فایل ورودی</param>
        /// <param name="chunkSize">اندازه هر تکه (بایت)</param>
        /// <returns>مسیر فایل آپلودشده</returns>
        public async Task<string> UploadFileChunkedAsync(string basePath, IFormFile file, int chunkSize = DefaultChunkSize)
        {
            if (string.IsNullOrEmpty(basePath))
                throw new ArgumentException("مسیر پایه نمی‌تواند خالی باشد.", nameof(basePath));
            if (file == null)
                throw new ArgumentNullException(nameof(file), "فایل نمی‌تواند null باشد.");
            if (chunkSize <= 0)
                throw new ArgumentException("اندازه تکه باید بزرگ‌تر از صفر باشد.", nameof(chunkSize));

            var fullPath = _fileSystem.Path.Combine(basePath, file.FileName);
            var directoryPath = _fileSystem.Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                _fileSystem.Directory.CreateDirectory(directoryPath);
            }

            string newFileName;
            string newFullPath;
            string tempMetadataPath;
            long uploadedBytes = 0;

            // بررسی وجود متادیتا برای ادامه آپلود
            var metadataFiles = _fileSystem.Directory.GetFiles(basePath, "*.metadata");
            if (metadataFiles.Length > 0)
            {
                tempMetadataPath = metadataFiles[0];
                var metadataContent = _fileSystem.File.ReadAllText(tempMetadataPath).Split('|');
                if (metadataContent.Length == 2 && long.TryParse(metadataContent[0], out var bytes) && !string.IsNullOrEmpty(metadataContent[1]))
                {
                    uploadedBytes = bytes;
                    newFileName = metadataContent[1];
                    newFullPath = _fileSystem.Path.Combine(basePath, newFileName);
                    tempMetadataPath = newFullPath + ".metadata";
                }
                else
                {
                    // متادیتا نامعتبر است، حذف متادیتا و فایل مرتبط
                    var oldFileName = _fileSystem.Path.GetFileNameWithoutExtension(tempMetadataPath);
                    var oldFilePath = _fileSystem.Path.Combine(basePath, oldFileName);
                    if (_fileSystem.File.Exists(oldFilePath))
                    {
                        _fileSystem.File.Delete(oldFilePath);
                    }
                    _fileSystem.File.Delete(tempMetadataPath);

                    newFileName = Guid.NewGuid().ToString() + _fileSystem.Path.GetExtension(file.FileName);
                    newFullPath = _fileSystem.Path.Combine(basePath, newFileName);
                    tempMetadataPath = newFullPath + ".metadata";
                }
            }
            else
            {
                newFileName = Guid.NewGuid().ToString() + _fileSystem.Path.GetExtension(file.FileName);
                newFullPath = _fileSystem.Path.Combine(basePath, newFileName);
                tempMetadataPath = newFullPath + ".metadata";
            }

            // حذف تمام فایل‌های متادیتا قدیمی
            foreach (var metadata in _fileSystem.Directory.GetFiles(basePath, "*.metadata"))
            {
                if (metadata != tempMetadataPath)
                {
                    _fileSystem.File.Delete(metadata);
                }
            }

            using (var inputStream = file.OpenReadStream())
            {
                inputStream.Seek(uploadedBytes, SeekOrigin.Begin);
                using (var outputStream = _fileSystem.FileStream.New(newFullPath, uploadedBytes == 0 ? FileMode.Create : FileMode.Append, FileAccess.Write))
                {
                    byte[] buffer = new byte[chunkSize];
                    int bytesRead;
                    while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        uploadedBytes += bytesRead;
                        _fileSystem.File.WriteAllText(tempMetadataPath, $"{uploadedBytes}|{newFileName}");
                    }
                }
            }

            // حذف فایل متادیتا پس از تکمیل آپلود
            if (_fileSystem.File.Exists(tempMetadataPath))
            {
                _fileSystem.File.Delete(tempMetadataPath);
            }

            var newdirect = StringExtensions.RemoveDirectWWWROOT(newFullPath);
            return newdirect ?? string.Empty;
        }
        /// <summary>
        /// فشرده‌سازی و تبدیل تصویر به فرمت استاندارد PNG با حفظ کامل شفافیت (Transparency) و بدون تغییر هدر استاندارد
        /// </summary>
        public static byte[] ImageToByteArray(Image image, int maxWidth = 500, int maxHeight = 500)
        {
            if (image == null) return null;

            // ۱. محاسبه نسبت ابعاد
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = image.Width;
            int newHeight = image.Height;

            if (ratio < 1.0)
            {
                newWidth = Math.Max(1, (int)(image.Width * ratio));
                newHeight = Math.Max(1, (int)(image.Height * ratio));
            }

            // ۲. تولید بیت‌مپ با رزولوشن بهینه برای وب/دسکتاپ (کاهش چشمگیر حجم)
            using (var newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb))
            {
                newImage.SetResolution(72, 72);

                using (var g = Graphics.FromImage(newImage))
                {
                    g.Clear(Color.Transparent);
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.CompositingQuality = CompositingQuality.HighSpeed;
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight));
                }

                // ۳. ذخیره مستقیم به فرمت معتبر PNG
                using (var ms = new MemoryStream())
                {
                    newImage.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// تغییر سایز دقیق با Interpolation باکیفیت و رزولوشن بهینه
        /// </summary>
        private static Bitmap ResizeImagePreserveAlpha(Image original, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / original.Width;
            double ratioY = (double)maxHeight / original.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = original.Width;
            int newHeight = original.Height;

            // اگر ابعاد بزرگتر از حد تعیین شده باشد، کوچک می‌شود
            if (ratio < 1.0)
            {
                newWidth = Math.Max(1, (int)(original.Width * ratio));
                newHeight = Math.Max(1, (int)(original.Height * ratio));
            }

            var newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
            newImage.SetResolution(96, 96); // استانداردسازی DPI جهت کاهش سربار حجم تصویر

            using (var g = Graphics.FromImage(newImage))
            {
                g.Clear(Color.Transparent);
                g.CompositingMode = CompositingMode.SourceOver;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(original, new Rectangle(0, 0, newWidth, newHeight));
            }

            return newImage;
        }

        /// <summary>
        /// فشرده‌سازی داخلی برای کاهش بایت‌های اضافی و متادیتاهای PNG
        /// </summary>
        private static byte[] OptimizePngBytes(byte[] input)
        {
            using (var inputStream = new MemoryStream(input))
            using (var outputStream = new MemoryStream())
            {
                // استفاده از فشرده‌سازی بهینه سازگار با ذخیره‌سازی داده‌های دیتابیس
                using (var deflate = new DeflateStream(outputStream, CompressionLevel.Optimal, true))
                {
                    inputStream.CopyTo(deflate);
                }

                // در صورت موثر بودن فشرده‌سازی، خروجی بهینه برگشت داده می‌شود
                byte[] compressed = outputStream.ToArray();
                return compressed.Length < input.Length ? compressed : input;
            }
        }
    }
}