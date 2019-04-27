using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Web.Services
{
    public class MockIdService : IIdService
    {
        public Task<int> GetAsync()
        {
            throw new NotImplementedException();
        }
    }
}