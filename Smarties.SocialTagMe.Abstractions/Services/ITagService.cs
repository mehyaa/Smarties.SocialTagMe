using Smarties.SocialTagMe.Abstractions.Models;
using System.IO;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Abstractions.Services
{
    public interface ITagService
    {
        Task<string> TagAsync(Stream image, SocialInfo socialInfo = null);
        Task<string> UpdateAsync(string id, SocialInfo socialInfo);
        Task<SocialInfo> QueryAsync(Stream image);
    }
}