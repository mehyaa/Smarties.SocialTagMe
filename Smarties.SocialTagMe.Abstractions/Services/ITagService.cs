using Smarties.SocialTagMe.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Abstractions.Services
{
    public interface ITagService
    {
        Task<int> TagAsync(IEnumerable<string> imagePaths, SocialInfo socialInfo = null, bool train = true);
        Task UpdateAsync(int id, SocialInfo socialInfo);
        Task<SocialInfo> QueryAsync(string imagePath);
        Task TrainAsync();
    }
}