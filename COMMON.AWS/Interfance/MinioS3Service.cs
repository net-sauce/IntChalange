using COMMON.AWS.Abstraction;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using System;
using System.IO;
using System.Threading.Tasks;

namespace COMMON.AWS.Interfance
{
    public class MinioS3Service : IS3Service
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinioS3Service> _logger;

        public MinioS3Service(string bucket, string endpoint, string accessKey, string secretKey, ILogger<MinioS3Service> logger)
        {
            this._minioClient = new MinioClient()
                                .WithEndpoint(endpoint)
                                .WithCredentials(accessKey, secretKey)
                                .Build();
            this._logger = logger;
        }

        public async Task<string> GenerateS3PresignedURL(string bucketName, string objectKey, TimeSpan expiryIn = default)
        {
            try
            {
                if (expiryIn == default)
                {
                    expiryIn = TimeSpan.FromSeconds(120);
                }
                return await this._minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                                  .WithBucket(bucketName)
                                  .WithObject(objectKey)
                                  .WithExpiry((int)expiryIn.TotalSeconds));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"In GenerateS3PresignedURL: Error generating pre-signed URL for download");
                throw;
            }
        }


        public async Task UploadFileAsync(string bucketName, string objectKey, Stream fileStream, long fileSize, string contentType = "application/octet-stream")
        {
            try
            {
                _logger.LogInformation($"In UploadFileAsync: Uploading file to Minio: Bucket={bucketName}, ObjectKey={objectKey}, FileSize={fileSize}");

                var args = new PutObjectArgs()
                            .WithBucket(bucketName)
                            .WithObject(objectKey)
                            .WithStreamData(fileStream)
                            .WithObjectSize(fileSize)
                            .WithContentType(contentType);

                await _minioClient.PutObjectAsync(args);
                _logger.LogInformation($"In UploadFileAsync: Successfully uploaded file to Minio: Bucket={bucketName}, ObjectKey={objectKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"In UploadFileAsync: Error uploading file: Bucket={bucketName}, ObjectKey={objectKey}");
                throw;
            }
        }
    }
}