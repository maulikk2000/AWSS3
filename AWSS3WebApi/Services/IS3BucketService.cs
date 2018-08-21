using AWSS3WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSS3WebApi.Services
{
    public interface IS3BucketService
    {
        Task<S3Response> CreateS3BucketAsync(string bucketName);
        Task UploadFileAsync(string bucketName);
        Task GetObjectFromS3BucketAsync(string bucketName);
    }
}
