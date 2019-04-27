using Smarties.SocialTagMe.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Abstractions.Services
{
    public interface IImageService
    {
        Task AddAsync(int id, IEnumerable<string> imagePaths, bool train = true);
        Task<int?> RecognizeAsync(string imagePath);
        Task TrainAsync();
        Task<IList<DetectedFaceInfo>> DetectFaceAsync(string imagePath);
    }
}