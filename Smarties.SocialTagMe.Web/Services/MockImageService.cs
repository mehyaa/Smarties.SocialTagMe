using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Services
{
    public class MockImageService : IImageService
    {
        public Task AddAsync(int id, IEnumerable<string> imagePaths, bool train = true)
        {
            throw new NotImplementedException();
        }

        public Task<int?> RecognizeAsync(string imagePath)
        {
            throw new NotImplementedException();
        }

        public Task TrainAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<DetectedFaceInfo>> DetectFaceAsync(string imagePath)
        {
            throw new NotImplementedException();
        }
    }
}