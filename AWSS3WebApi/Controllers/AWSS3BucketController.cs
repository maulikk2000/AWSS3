using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWSS3WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWSS3WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AWSS3BucketController : ControllerBase
    {
        private readonly IS3BucketService _bucketService;

        public AWSS3BucketController(IS3BucketService bucketService)
        {
            _bucketService = bucketService;
        }

        [HttpPost("{bucketName}")]
        public async Task<IActionResult> CreateS3BucketAsync([FromRoute]string bucketName)
        {
            var response = await _bucketService.CreateS3BucketAsync(bucketName);
            return Ok(response);
        }

        [HttpPost]
        [Route("AddFile/{bucketName}")]
        public async Task<IActionResult> AddFile([FromRoute] string bucketName)
        {
            await _bucketService.UploadFileAsync(bucketName);

            return Ok();
        }

        [HttpGet]
        [Route("GetFile/{bucketName}")]
        public async Task<IActionResult> GetObjectFromS3BucketAsync([FromRoute] string bucketName)
        {
            await _bucketService.GetObjectFromS3BucketAsync(bucketName);
            return Ok();
        }
    }
}