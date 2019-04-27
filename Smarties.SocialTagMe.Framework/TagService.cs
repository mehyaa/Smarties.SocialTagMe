using Newtonsoft.Json;
using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Framework
{
    public class TagService : ITagService
    {
        private string DataFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Data");

        public async Task AddOrUpdateAsync(int id, SocialInfo socialInfo)
        {
            await Task.CompletedTask;

            var files = Directory.GetFiles(DataFolder, "*.image");

            if (files.Any() && socialInfo != null)
            {
                var infoPath = Path.Combine(DataFolder, $"{id.ToString()}.json");

                var infoJson = JsonConvert.SerializeObject(socialInfo);

                File.WriteAllText(infoPath, infoJson);
            }
        }

        public async Task<SocialInfo> GetAsync(int id)
        {
            await Task.CompletedTask;

            var infoPath = Path.Combine(DataFolder, $"{id.ToString()}.json");

            if (File.Exists(infoPath))
            {
                var infoJson = File.ReadAllText(infoPath);

                var socialInfo = JsonConvert.DeserializeObject<SocialInfo>(infoJson);

                return socialInfo;
            }

            return null;
        }
    }
}