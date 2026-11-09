
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace RaceCompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;

        public ImagesController(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        // Upload an event banner or participant profile picture
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(
            IFormFile file,
            string folder)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please select an image.");

            if (folder != "events" && folder != "profiles")
                return BadRequest("Invalid image folder.");

            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Only JPG, PNG and WEBP images are allowed.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (extension != ".jpg" && extension != ".jpeg" &&
                extension != ".png" && extension != ".webp")
                return BadRequest("Invalid image extension.");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("Image must be smaller than 5 MB.");

            var container = _blobServiceClient.GetBlobContainerClient(
                "raceday-images");

            await container.CreateIfNotExistsAsync(
                PublicAccessType.None);

            var fileName = $"{folder}/{Guid.NewGuid()}{extension}";

            var blob = container.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(
                    stream,
                    new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = file.ContentType
                        }
                    });
            }

            return Ok(new
            {
                message = "Image uploaded successfully.",
                blobName = fileName,
                imageUrl = $"/api/images/{fileName}"
            });
        }

        // Retrieve an image from the private container
        [HttpGet("{*blobName}")]
        public async Task<IActionResult> GetImage(string blobName)
        {
            var container = _blobServiceClient.GetBlobContainerClient(
                "raceday-images");

            var blob = container.GetBlobClient(blobName);

            if (!await blob.ExistsAsync())
                return NotFound("Image not found.");

            var download = await blob.DownloadStreamingAsync();

            return File(
                download.Value.Content,
                download.Value.Details.ContentType ?? "application/octet-stream");
        }
    }
}

