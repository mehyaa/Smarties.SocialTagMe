using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Services
{
    public class MockTagService : ITagService
    {
        public Task<int> TagAsync(IEnumerable<string> imagePaths, SocialInfo socialInfo = null, bool train = true)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, SocialInfo socialInfo)
        {
            throw new NotImplementedException();
        }

        public Task<SocialInfo> QueryAsync(string imagePath)
        {
            throw new NotImplementedException();
        }

        public Task TrainAsync()
        {
            throw new NotImplementedException();
        }
    }
}