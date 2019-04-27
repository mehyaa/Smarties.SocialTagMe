using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Smarties.SocialTagMe.Test
{
    public class TagServiceTest
    {
        [Fact]
        public async Task TagTestAsync()
        {
            var imagePath = GetImagePath("face1.jpg");

            var tagService = new TagService();

            var socialInfo = new SocialInfo
            {
                Name = "face"
            };

            var id = await tagService.TagAsync(imagePath, socialInfo);
        }

        [Fact]
        public async Task Tag2TestAsync()
        {
            var imagePath = GetImagePath("mehyaa1.jpg");

            var tagService = new TagService();

            var socialInfo = new SocialInfo
            {
                Name = "mehyaa"
            };

            var id = await tagService.TagAsync(imagePath, socialInfo);
        }

        [Fact]
        public async Task QueryTestAsync()
        {
            var tagService = new TagService();

            var imagePath1 = GetImagePath("face1.jpg");

            var socialInfo1 = await tagService.QueryAsync(imagePath1);

            Assert.Equal("face", socialInfo1?.Name);

            var imagePath2 = GetImagePath("face2.jpg");

            var socialInfo2 = await tagService.QueryAsync(imagePath2);

            Assert.Equal("face", socialInfo2?.Name);

            var imagePath3 = GetImagePath("face3.jpg");

            var socialInfo3 = await tagService.QueryAsync(imagePath3);

            Assert.Equal("face", socialInfo3?.Name);

            var imagePath4 = GetImagePath("face4.jpg");

            var socialInfo4 = await tagService.QueryAsync(imagePath4);

            Assert.Equal("face", socialInfo4?.Name);
        }

        [Fact]
        public async Task Query2TestAsync()
        {
            var tagService = new TagService();

            var imagePath1 = GetImagePath("mehyaa1.jpg");

            var socialInfo1 = await tagService.QueryAsync(imagePath1);

            Assert.Equal("mehyaa", socialInfo1?.Name);

            var imagePath2 = GetImagePath("mehyaa2.jpg");

            var socialInfo2 = await tagService.QueryAsync(imagePath2);

            Assert.Equal("mehyaa", socialInfo2?.Name);

            var imagePath3 = GetImagePath("mehyaa3.jpg");

            var socialInfo3 = await tagService.QueryAsync(imagePath3);

            Assert.Equal("mehyaa", socialInfo3?.Name);

            var imagePath4 = GetImagePath("mehyaa4.jpg");

            var socialInfo4 = await tagService.QueryAsync(imagePath4);

            Assert.Equal("mehyaa", socialInfo4?.Name);

            var imagePath5 = GetImagePath("mehyaa5.jpg");

            var socialInfo5 = await tagService.QueryAsync(imagePath5);

            Assert.Equal("mehyaa", socialInfo5?.Name);

            var imagePath6 = GetImagePath("mehyaa6.jpg");

            var socialInfo6 = await tagService.QueryAsync(imagePath6);

            Assert.Equal("mehyaa", socialInfo6?.Name);

            var imagePath7 = GetImagePath("mehyaa7.jpg");

            var socialInfo7 = await tagService.QueryAsync(imagePath7);

            Assert.Equal("mehyaa", socialInfo7?.Name);
        }

        [Fact]
        public async Task QueryUnknownTestAsync()
        {
            var tagService = new TagService();

            var imagePath1 = GetImagePath("unknown1.jpg");

            var socialInfo1 = await tagService.QueryAsync(imagePath1);

            Assert.Null(socialInfo1);
        }

        private string GetImagePath(string name)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", name);
        }
    }
}