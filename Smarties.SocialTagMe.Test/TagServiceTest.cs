using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Framework;
using System;
using System.Collections.Generic;
using System.IO;
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
            var tagService = new TagService();

            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);

            foreach (var imagePath in files)
            {
                var fileName = Path.GetFileName(imagePath);

                var dirName = Path.GetFileName(Path.GetDirectoryName(imagePath));

                if (_ignoredFiles.Contains(fileName))
                {
                    continue;
                }

                var imagePaths = new[] { imagePath };

                var socialInfo = new SocialInfo
                {
                    Name = dirName
                };

                var id = await tagService.TagAsync(imagePaths, socialInfo, train: false);
            }

            await tagService.TrainAsync();
        }

        [Fact]
        public async Task QueryRandomAsync()
        {
            var tagService = new TagService();

            var imagePath = GetRandomImagePath();

            var fileName = Path.GetFileName(imagePath);

            var dirName = Path.GetFileName(Path.GetDirectoryName(imagePath));

            var socialInfo = await tagService.QueryAsync(imagePath);

            if (_ignoredFiles.Contains(fileName))
            {
                Assert.Null(socialInfo);
            }
            else
            {
                Assert.Equal(dirName, socialInfo?.Name);
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