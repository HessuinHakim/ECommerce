using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Files;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxSize = 2 * 1024 * 1024;   // 2MB

        private readonly string _root;

        public ImageService(IConfiguration config)
        {
            var relative = config["Storage:ImagesPath"] ?? "wwwroot/images";
            _root = Path.Combine(Directory.GetCurrentDirectory(), relative);
        }

        public async Task<string> SaveAsync(FileUpload file, string folder)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(ext))
                throw new ArgumentException("Unsupported image type.");
            if (file.Length > MaxSize)
                throw new ArgumentException("Image is too large (max 2MB).");

            var dir = Path.Combine(_root, folder);
            Directory.CreateDirectory(dir);

            var name = $"{Guid.NewGuid()}{ext}";          // اسم جديد، مش اسم المستخدم
            await using var fs = new FileStream(Path.Combine(dir, name), FileMode.Create);
            await file.Content.CopyToAsync(fs);

            return name;
        }

        public void Delete(string fileName, string folder)
        {
            var path = Path.Combine(_root, folder, fileName);
            if (File.Exists(path)) File.Delete(path);
        }

    }
}
