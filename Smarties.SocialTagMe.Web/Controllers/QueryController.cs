using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly ILogger<QueryController> _logger;
        private readonly IImageService _imageService;
        private readonly ITagService _tagService;

        public QueryController(
            ILogger<QueryController> logger,
            IImageService imageService,
            ITagService tagService)
        {
            _logger = logger;
            _imageService = imageService;
            _tagService = tagService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SocialInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(IFormFile file)
        {
            if (file.Length == 0)
            {
                return BadRequest();
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

                _logger.LogInformation("Recognized id: {0}", id);

                if (id.HasValue)
                {
                    var socialInfo = await _tagService.GetAsync(id.Value);

                    _logger.LogInformation("SocialInfo: {0}", JsonConvert.SerializeObject(socialInfo));

                    if (socialInfo != null)
                    {
                        _logger.LogInformation("SocialInfo.Name: {0}", socialInfo.Name);

                        return Ok(socialInfo);
                    }
                }

                return NotFound();
            }

            return BadRequest();
        }
    }
}