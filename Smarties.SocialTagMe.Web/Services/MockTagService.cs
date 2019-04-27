using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Services
{
    public class MockTagService : ITagService
    {
        public Task<SocialInfo> QueryAsync(Stream image)
        {
            throw new NotImplementedException();
        }

        public Task<string> TagAsync(Stream image, SocialInfo socialInfo = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateAsync(string id, SocialInfo socialInfo)
        {
            throw new NotImplementedException();
        }
    }
}