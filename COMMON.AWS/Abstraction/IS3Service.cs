using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMMON.AWS.Abstraction
{
   public interface IS3Service
    {
        Task<string> GenerateS3PresignedURL(string bucketName, string objectKey, TimeSpan expiryIn = default);
        Task UploadFileAsync(string bucketName, string objectKey, Stream fileStream, long fileSize, string contentType = "application/octet-stream");
    }
}
