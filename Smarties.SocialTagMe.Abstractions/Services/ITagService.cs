using Smarties.SocialTagMe.Abstractions.Models;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Abstractions.Services
{
    public interface ITagService
    {
        Task AddOrUpdateAsync(int id, SocialInfo socialInfo);
        Task<SocialInfo> GetAsync(int id);
    }
}