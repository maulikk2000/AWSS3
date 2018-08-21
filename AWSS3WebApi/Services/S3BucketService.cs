using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using AWSS3WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AWSS3WebApi.Services
{
    public class S3BucketService : IS3BucketService
    {
        private readonly IAmazonS3 _amazonClient;

        public S3BucketService(IAmazonS3 amazonClient) 
        {
            _amazonClient = amazonClient;
        }
        public async Task<S3Response> CreateS3BucketAsync(string bucketName)
        {
            try
            {
                if(await AmazonS3Util.DoesS3BucketExistAsync(_amazonClient, bucketName) == false)
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    var response = await _amazonClient.PutBucketAsync(putBucketRequest);

                    return new S3Response
                    {
                        Message = response.ResponseMetadata.RequestId,
                        Status = response.HttpStatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return new S3Response
                {
                    Message = ex.Message,
                    Status = HttpStatusCode.InternalServerError
                };
                throw;
            }

            return new S3Response
            {
                Message = "something went wrong",
                Status = HttpStatusCode.InternalServerError
            };
        }

        private const string FilePath = @"D:\AWS\S3Bucket\s3TestFile.txt";
        private const string UploadWithKeyName = "UploadWithKeyName";
        private const string FileStreamUpload = "FileStreamUpload";
        private const string AdvancedUpload = "AdvancedUpload";

        public async Task UploadFileAsync(string bucketName)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(_amazonClient);

                //here we are using different options to upload the same file

                //option1
                await fileTransferUtility.UploadAsync(FilePath, bucketName);

                //option2
                await fileTransferUtility.UploadAsync(FilePath, bucketName, UploadWithKeyName);

                //option3
                using (var fileToUpload = new FileStream(FilePath, FileMode.Open ,FileAccess.Read))
                {
                    await fileTransferUtility.UploadAsync(fileToUpload, bucketName, FileStreamUpload);
                }

                //option4
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    FilePath = FilePath,
                    StorageClass = S3StorageClass.Standard,
                    PartSize = 6291456, //6MB
                    Key = AdvancedUpload,
                    CannedACL = S3CannedACL.NoACL
                };

                fileTransferUtilityRequest.Metadata.Add("param1", "value1");
                fileTransferUtilityRequest.Metadata.Add("param2", "value2");

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task GetObjectFromS3BucketAsync(string bucketName)
        {
            //we are hardcoding the file name here, read from appsettings.json file or pass it as a param
            const string keyName = "s3TestFile.txt";

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                string responseBody;

                using(var response = await _amazonClient.GetObjectAsync(request))
                {
                    using(var responseSteram = response.ResponseStream)
                    {
                        using (var reader = new StreamReader(responseSteram))
                        {
                            //the metadata name must start with x-amz-meta. if it doesnt then it'll be prepended
                            //if no title, it'll return null
                            var title = response.Metadata["title"];
                            var contentType = response.Headers["Content-Type"];

                            responseBody = reader.ReadToEnd();
                        }
                    }
                }

                var pathAndFileName = $@"D:\{keyName}";

                File.WriteAllText(pathAndFileName, responseBody);

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
