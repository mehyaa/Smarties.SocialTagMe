using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Controllers
{
    [Route("api/tag")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IIdService _idService;
        private readonly IImageService _imageService;
        private readonly ITagService _tagService;

        public TagController(IIdService idService, IImageService imageService, ITagService tagService)
        {
            _idService = idService;
            _imageService = imageService;
            _tagService = tagService;
        }

        [HttpPost]
        public async Task<int?> Add(IFormFileCollection files, [FromForm] SocialInfo socialInfo)
        {
            if (files?.Count > 0)
            {
                var imagePaths = new List<string>();

                foreach (var file in files)
                {
                    if (file.Length == 0)
                    {
                        continue;
                    }

                    var imagePath = Path.GetTempFileName();

                    using (var fileStream = new FileStream(imagePath, FileMode.Append))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var faces = await _imageService.DetectFaceAsync(imagePath);

                    var biggestFace = faces.OrderByDescending(x => x.Width * x.Height).FirstOrDefault();

                    if (biggestFace == null)
                    {
                        continue;
                    }

                    imagePaths.Add(biggestFace.Path);
                }

                if (imagePaths.Count > 0)
                {
                    var id = await _idService.GetAsync();

                    await _imageService.AddAsync(id, imagePaths);

                    if (socialInfo != null)
                    {
                        await _tagService.AddOrUpdateAsync(id, socialInfo);
                    }

                    return id;
                }
            }

            return null;
        }

        [HttpPut("{id:int}")]
        public async Task Update(int id, [FromBody] SocialInfo socialInfo)
        {
            if (id == default || socialInfo == null)
            {
                return;
            }

            await _tagService.AddOrUpdateAsync(id, socialInfo);
        }
    }
}