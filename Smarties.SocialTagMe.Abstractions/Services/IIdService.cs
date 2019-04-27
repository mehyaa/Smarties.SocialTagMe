using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Abstractions.Services
{
    public interface IIdService
    {
        Task<int> GetAsync();
    }
}