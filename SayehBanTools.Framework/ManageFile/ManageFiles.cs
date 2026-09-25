using Microsoft.AspNetCore.Http;
using SayehBanTools.Framework.Converter;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Abstractions;
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
        /// تبدیل تصویر به آرایه بایت با حفظ دقیق نوع اصلی (PNG، JPG، BMP، GIF و ...)
        /// مناسب برای تصاویر خیلی بزرگ یا مشکل‌دار
        /// </summary>
        /// <param name="image">تصویر</param>
        /// <returns>بایت آرایه تصویر اصلی</returns>

        public static byte[] ImageToByteArray(Image image, int maxWidth = 600, int maxHeight = 600, long jpegQuality = 85)
        {
            if (image == null) return null;

            // ۱. تغییر سایز اگر تصویر از حد مجاز بزرگتره
            Image resized = ResizeIfNeeded(image, maxWidth, maxHeight);

            using (var ms = new MemoryStream())
            {
                bool hasTransparency = ImageHasTransparency(resized);

                if (hasTransparency)
                {
                    // لوگوهای شفاف باید PNG بمونن
                    resized.Save(ms, ImageFormat.Png);
                }
                else
                {
                    // برای بقیه تصاویر PNG با کیفیت بالا => حجم خیلی کمتر
                    var jpegEncoder = ImageCodecInfo.GetImageDecoders()
                        .First(c => c.FormatID == ImageFormat.Png.Guid);

                    var encParams = new EncoderParameters(1);
                    encParams.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);

                    resized.Save(ms, jpegEncoder, encParams);
                }

                if (resized != image) resized.Dispose();

                return ms.ToArray();
            }
        }

        private static Image ResizeIfNeeded(Image original, int maxWidth, int maxHeight)
        {
            if (original.Width <= maxWidth && original.Height <= maxHeight)
                return original;

            double ratio = Math.Min((double)maxWidth / original.Width, (double)maxHeight / original.Height);
            int newWidth = (int)(original.Width * ratio);
            int newHeight = (int)(original.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(original, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        private static bool ImageHasTransparency(Image image)
        {
            if (!(image is Bitmap bmp)) return false;
            if (!Image.IsAlphaPixelFormat(bmp.PixelFormat)) return false;

            // بررسی سریع چند پیکسل (برای سرعت، نه همه پیکسل‌ها)
            for (int x = 0; x < bmp.Width; x += Math.Max(1, bmp.Width / 20))
                for (int y = 0; y < bmp.Height; y += Math.Max(1, bmp.Height / 20))
                    if (bmp.GetPixel(x, y).A < 255)
                        return true;

            return false;
        }
    }
}