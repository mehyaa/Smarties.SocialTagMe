using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Services
{
    public class MockTagService : ITagService
    {
        public Task AddOrUpdateAsync(int id, SocialInfo socialInfo)
        {
            throw new NotImplementedException();
        }

        public Task<SocialInfo> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}