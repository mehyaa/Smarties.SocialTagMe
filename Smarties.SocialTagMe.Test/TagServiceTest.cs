using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Smarties.SocialTagMe.Test
{
    public class TagServiceTest
    {
        private readonly HashSet<string> _ignoredFiles = new HashSet<string>
        {
            "unknown1.jpg"
        };

        [Fact]
        public async Task TagBatchAsync()
        {
            var idService = new IdService();

            var imageService = new ImageService();

            var tagService = new TagService();

            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            var groupedFiles =
                Directory
                    .GetFiles(dir, "*.*", SearchOption.AllDirectories)
                    .GroupBy(x => Path.GetFileName(Path.GetDirectoryName(x)));

            foreach (var files in groupedFiles)
            {
                var dirName = files.Key;

                var id = await idService.GetAsync();

                var imagePaths = new List<string>();

                foreach (var imagePath in files)
                {
                    var fileName = Path.GetFileName(imagePath);

                    if (_ignoredFiles.Contains(fileName))
                    {
                        continue;
                    }

                    var faces = await imageService.DetectFaceAsync(imagePath);

                    var biggestFace = faces.OrderByDescending(x => x.Width * x.Height).FirstOrDefault();

                    if (biggestFace == null)
                    {
                        continue;
                    }

                    imagePaths.Add(biggestFace.Path);
                }

                if (imagePaths.Count == 0)
                {
                    continue;
                }

                var socialInfo = new SocialInfo
                {
                    Name = dirName
                };

                await imageService.AddAsync(id, imagePaths, train: false);

                await tagService.AddOrUpdateAsync(id, socialInfo);
            }

            await imageService.TrainAsync();
        }

        [Fact]
        public async Task QueryRandomAsync()
        {
            var imageService = new ImageService();

            var tagService = new TagService();

            var imagePath = GetRandomImagePath();

            var fileName = Path.GetFileName(imagePath);

            var dirName = Path.GetFileName(Path.GetDirectoryName(imagePath));

            var id = await imageService.RecognizeAsync(imagePath);

            if (_ignoredFiles.Contains(fileName))
            {
                Assert.Null(id);
            }
            else
            {
                if (id.HasValue)
                {
                    var socialInfo = await tagService.GetAsync(id.Value);

                    Assert.Equal(dirName, socialInfo?.Name);
                }
                else
                {
                    throw new Exception("No id returned.");
                }
            }
        }

        private static string GetRandomImagePath()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            var files = Directory.GetFiles(dir);

            var random = new Random();

            return files[random.Next(files.Length)];
        }
    }
}