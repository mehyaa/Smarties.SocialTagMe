using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Framework
{
    public class IdService : IIdService
    {
        public async Task<int> GetAsync()
        {
            await Task.CompletedTask;

            var now = DateTime.UtcNow;

            var zeroDate = DateTime.MinValue.AddHours(now.Hour).AddMinutes(now.Minute).AddSeconds(now.Second).AddMilliseconds(now.Millisecond);

            return (int)(zeroDate.Ticks / 10000);
        }
    }
}