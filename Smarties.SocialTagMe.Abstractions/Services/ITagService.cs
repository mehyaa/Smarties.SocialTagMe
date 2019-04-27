using Smarties.SocialTagMe.Abstractions.Models;
using System.IO;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Abstractions.Services
{
    public interface ITagService
    {
        Task<int> TagAsync(Stream image, SocialInfo socialInfo = null);
        Task UpdateAsync(int id, SocialInfo socialInfo);
        Task<SocialInfo> QueryAsync(Stream image);
    }
}