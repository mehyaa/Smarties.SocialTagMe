using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Controllers
{
    [Route("api/query")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ITagService _tagService;

        public QueryController(IImageService imageService, ITagService tagService)
        {
            _imageService = imageService;
            _tagService = tagService;
        }

        [HttpPost]
        public async Task<SocialInfo> Get(IFormFile file)
        {
            if (file.Length == 0)
            {
                return null;
            }

            var imagePath = $"{Path.GetTempFileName()}.{file.FileName}";

            using (var fileStream = new FileStream(imagePath, FileMode.Append))
            {
                await file.CopyToAsync(fileStream);
            }

            var faces = await _imageService.DetectFaceAsync(imagePath);

            var biggestFace = faces.OrderByDescending(x => x.Width * x.Height).FirstOrDefault();

            if (biggestFace != null)
            {
                var id = await _imageService.RecognizeAsync(biggestFace.Path);

                if (id.HasValue)
                {
                    return await _tagService.GetAsync(id.Value);
                }
            }

            return null;
        }
    }
}