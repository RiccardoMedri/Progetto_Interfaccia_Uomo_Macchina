using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Web.Areas.Admin.Properties
{
    public interface IAdminPropertyMediaStorage
    {
        Task<AdminPropertyMediaStorageResult> SaveAsync(
            string reference,
            IEnumerable<IFormFile> files,
            CancellationToken cancellationToken);
    }

    public sealed class AdminPropertyMediaStorage : IAdminPropertyMediaStorage
    {
        private const long MaxFileSize = 8 * 1024 * 1024;

        private static readonly HashSet<string> AllowedExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

        private static readonly HashSet<string> AllowedContentTypes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

        private readonly IWebHostEnvironment webHostEnvironment;

        public AdminPropertyMediaStorage(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<AdminPropertyMediaStorageResult> SaveAsync(
            string reference,
            IEnumerable<IFormFile> files,
            CancellationToken cancellationToken)
        {
            var submittedFiles = (files ?? Array.Empty<IFormFile>())
                .Where(file => file != null && file.Length > 0)
                .ToArray();
            if (submittedFiles.Length == 0)
            {
                return AdminPropertyMediaStorageResult.Empty();
            }

            var referenceSegment = SafePathSegment(reference);
            var uploadDirectory = Path.Combine(
                WebRootPath(),
                "uploads",
                "properties",
                referenceSegment);
            Directory.CreateDirectory(uploadDirectory);

            var savedUrls = new List<string>();
            var rejectedCount = 0;
            foreach (var file in submittedFiles)
            {
                if (!await IsAllowedUploadAsync(file, cancellationToken))
                {
                    rejectedCount++;
                    continue;
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadDirectory, fileName);

                await using (var stream = new FileStream(filePath, FileMode.CreateNew))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                savedUrls.Add($"/uploads/properties/{referenceSegment}/{fileName}");
            }

            return new AdminPropertyMediaStorageResult
            {
                SavedUrls = savedUrls.ToArray(),
                RejectedCount = rejectedCount
            };
        }

        private string WebRootPath()
        {
            return string.IsNullOrWhiteSpace(webHostEnvironment.WebRootPath)
                ? Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot")
                : webHostEnvironment.WebRootPath;
        }

        private static async Task<bool> IsAllowedUploadAsync(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file == null || file.Length <= 0 || file.Length > MaxFileSize)
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension) ||
                !AllowedContentTypes.Contains(file.ContentType))
            {
                return false;
            }

            return await HasAllowedSignatureAsync(file, cancellationToken);
        }

        private static async Task<bool> HasAllowedSignatureAsync(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            var header = new byte[12];
            await using var stream = file.OpenReadStream();
            var bytesRead = await stream.ReadAsync(header, 0, header.Length, cancellationToken);

            return IsJpeg(header, bytesRead) ||
                IsPng(header, bytesRead) ||
                IsWebp(header, bytesRead);
        }

        private static bool IsJpeg(byte[] header, int bytesRead)
        {
            return bytesRead >= 3 &&
                header[0] == 0xFF &&
                header[1] == 0xD8 &&
                header[2] == 0xFF;
        }

        private static bool IsPng(byte[] header, int bytesRead)
        {
            return bytesRead >= 8 &&
                header[0] == 0x89 &&
                header[1] == 0x50 &&
                header[2] == 0x4E &&
                header[3] == 0x47 &&
                header[4] == 0x0D &&
                header[5] == 0x0A &&
                header[6] == 0x1A &&
                header[7] == 0x0A;
        }

        private static bool IsWebp(byte[] header, int bytesRead)
        {
            return bytesRead >= 12 &&
                header[0] == 0x52 &&
                header[1] == 0x49 &&
                header[2] == 0x46 &&
                header[3] == 0x46 &&
                header[8] == 0x57 &&
                header[9] == 0x45 &&
                header[10] == 0x42 &&
                header[11] == 0x50;
        }

        private static string SafePathSegment(string value)
        {
            var safeChars = (value ?? "property")
                .Where(character => char.IsLetterOrDigit(character) ||
                                    character == '-' ||
                                    character == '_')
                .ToArray();
            return safeChars.Length == 0 ? "property" : new string(safeChars);
        }
    }

    public sealed class AdminPropertyMediaStorageResult
    {
        public IReadOnlyList<string> SavedUrls { get; set; } =
            Array.Empty<string>();

        public int RejectedCount { get; set; }

        public static AdminPropertyMediaStorageResult Empty()
        {
            return new AdminPropertyMediaStorageResult();
        }
    }
}
