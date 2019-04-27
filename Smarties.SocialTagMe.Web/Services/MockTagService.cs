using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Services
{
    public class MockTagService : ITagService
    {
        public Task<int> TagAsync(string imagePath, SocialInfo socialInfo = null)
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
    }
}