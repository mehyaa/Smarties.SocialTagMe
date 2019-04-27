using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Controllers
{
    [Route("api/tag")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpPost("tag")]
        public async Task<int> Tag(IFormFileCollection files, [FromForm] SocialInfo socialInfo)
        {
            if (files?.Count > 0)
            {
                var filePaths = new List<string>();

                foreach (var file in files)
                {
                    if (file.Length == 0)
                    {
                        continue;
                    }

                    var filePath = Path.GetTempFileName();

                    using (var fileStream = new FileStream(filePath, FileMode.Append))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    filePaths.Add(filePath);
                }

                return await _tagService.TagAsync(filePaths, socialInfo);
            }

            return -1;
        }

        [HttpPost("update/{id:int}")]
        public async Task Update(int id, [FromBody] SocialInfo socialInfo)
        {
            if (id == default || socialInfo == null)
            {
                return;
            }

            await _tagService.UpdateAsync(id, socialInfo);
        }

        [HttpPost("query")]
        public async Task<SocialInfo> Query(IFormFile file)
        {
            if (file.Length == 0)
            {
                return null;
            }

            var filePath = Path.GetTempFileName();

            using (var fileStream = new FileStream(filePath, FileMode.Append))
            {
                await file.CopyToAsync(fileStream);
            }

            return await _tagService.QueryAsync(filePath);
        }
    }
}