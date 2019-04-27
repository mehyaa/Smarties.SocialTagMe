using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Services
{
    public class MockTagService : ITagService
    {
        public Task<int> TagAsync(Stream image, SocialInfo socialInfo = null)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, SocialInfo socialInfo)
        {
            throw new NotImplementedException();
        }

        public Task<SocialInfo> QueryAsync(Stream image)
        {
            throw new NotImplementedException();
        }
    }
}