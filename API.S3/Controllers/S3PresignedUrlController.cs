using COMMON.AWS.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace API.S3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class S3PresignedUrlController : ControllerBase
    {
        private readonly IS3Service _s3Service;
        private readonly string? _bucket;
        private readonly ILogger<S3PresignedUrlController> _logger;

        public S3PresignedUrlController(IS3Service s3Service, IConfiguration configuration, ILogger<S3PresignedUrlController> logger)
        {
            _s3Service = s3Service ?? throw new ArgumentNullException(nameof(s3Service));
            _bucket = configuration.GetValue<string>("BUCKET_NAME") ?? throw new ArgumentNullException("BUCKET_NAME configuration is missing.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Generates a pre-signed S3 URL for a specific file in the bucket.
        /// </summary>
        [HttpGet()]
        public async Task<ActionResult<string>> GetPresignedURL([FromQuery] int client_id, [FromQuery] Guid job_id, [FromQuery] string file_name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(file_name))
                {
                    _logger.LogWarning("In GetPresignedURL: The file_name parameter is missing or empty.");
                    return BadRequest("File name is required.");
                }

                _logger.LogInformation("In GetPresignedURL: Generating S3 pre-signed URL for {ClientID}/{JobID}/{FileName} on bucket {Bucket}.",
                                        client_id, job_id, file_name, _bucket);

                var presignedUrl = await _s3Service.GenerateS3PresignedURL(_bucket, $"{client_id}/{job_id}/{file_name}");

                _logger.LogInformation("In GetPresignedURL: Successfully generated S3 pre-signed URL for {ClientID}/{JobID}/{FileName} on bucket {Bucket}.",
                                        client_id, job_id, file_name, _bucket);

                return Ok(presignedUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In GetPresignedURL: Failed to generate S3 pre-signed URL for {ClientID}/{JobID}/{FileName} on bucket {Bucket}.",
                                  client_id, job_id, file_name, _bucket);
                return StatusCode(500, " An error occurred while generating the S3 pre-signed URL. Please try again later.");
            }
        }

        /// <summary>
        /// Uploads a file to the specified S3 bucket using MinioS3Service.
        /// </summary>
        /// <param name="client_id">Client ID</param>
        /// <param name="job_id">Job ID</param>
        /// <param name="file">Uploaded file</param>
        /// <returns>Status of the upload operation</returns>
        [HttpPost()]
        public async Task<IActionResult> UploadFileAsync([FromQuery] int client_id, [FromQuery] Guid job_id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("In UploadFileAsync: No file was provided or the file is empty.");
                return BadRequest("File is required and cannot be empty.");
            }

            try
            {

                string fileName = file.FileName;

                _logger.LogInformation("In UploadFileAsync: Uploading file {FileName} to bucket {Bucket} for client {ClientID}, job {JobID}.",
                                        fileName, _bucket, client_id, job_id);


                string objectKey = $"{client_id}/{job_id}/{fileName}";


                using var fileStream = file.OpenReadStream();


                await _s3Service.UploadFileAsync(_bucket, objectKey, fileStream, file.Length, file.ContentType);

                _logger.LogInformation("In UploadFileAsync: Successfully uploaded file {FileName} for client {ClientID}, job {JobID} to bucket {Bucket}.",
                                        fileName, client_id, job_id, _bucket);

                return Ok($"File {fileName} uploaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In UploadFileAsync: Failed to upload file for {ClientID}/{JobID}.", client_id, job_id);
                return StatusCode(500, "An error occurred while uploading the file. Please try again later.");
            }
        }
    }
}