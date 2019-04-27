using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
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
        public async Task<int> Tag(IFormFile file, [FromForm] SocialInfo socialInfo)
        {
            if (file.Length == 0)
            {
                return default;
            }

            var filePath = Path.GetTempFileName();

            using (var fileStream = new FileStream(filePath, FileMode.Append))
            {
                await file.CopyToAsync(fileStream);
            }

            return await _tagService.TagAsync(filePath, socialInfo);
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