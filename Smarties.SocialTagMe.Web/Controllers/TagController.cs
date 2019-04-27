using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;

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

        [HttpPost("query")]
        public async Task<SocialInfo> Query(IFormFile file)
        {
            FileStream fs = null;

            if (file.Length > 0)
            {
                await file.CopyToAsync(fs);
            }

            return await _tagService.QueryAsync(fs); 
        }

        [HttpPost("update/{id}")]
        public async Task Update(string id, [FromBody] SocialInfo socialInfo)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return;
            }

            await _tagService.UpdateAsync(id, socialInfo);
        }

        [HttpPost("tag")]
        public async Task<string> Tag(IFormFile file, [FromForm] SocialInfo socialInfo)
        {
            FileStream fs = null;

            if (file.Length > 0)
            {
                await file.CopyToAsync(fs);
            }

            return await _tagService.TagAsync(fs, socialInfo);
        }
    }
}