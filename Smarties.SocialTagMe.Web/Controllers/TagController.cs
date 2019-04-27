using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smarties.SocialTagMe.Abstractions.Models;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Controllers
{
    [Route("api/tag")]
    [ApiController]
    public class TagController : ControllerBase
    {
        [HttpPost("query")]
        public async Task<SocialInfo> Query(IFormFile file)
        {
            return null;
        }

        [HttpPost("update/{id}")]
        public async Task Update(string id, [FromBody] SocialInfo socialInfo)
        {
        }

        [HttpPost("tag")]
        public async Task<string> Tag(IFormFile file, [FromForm] SocialInfo socialInfo)
        {
            return null;
        }
    }
}