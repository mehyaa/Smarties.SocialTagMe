using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Framework
{
    public class TagService : ITagService
    {
        private readonly ConcurrentDictionary<int, byte[]> _faceData = new ConcurrentDictionary<int, byte[]>();

        private readonly ConcurrentDictionary<int, SocialInfo> _socialInfoData = new ConcurrentDictionary<int, SocialInfo>();

        private readonly FaceRecognizer _faceRecognizer;

        private string DataFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Training.data");

        public TagService()
        {
            _faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
        }

        public async Task<int> TagAsync(Stream image, SocialInfo socialInfo = null)
        {
            var existingSocialInfo = await QueryAsync(image);

            if (existingSocialInfo != null)
            {
                return existingSocialInfo.Id;
            }

            using (var memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);

                var bytes = memoryStream.ToArray();

                var id = NextId();

                _faceData.AddOrUpdate(id, bytes, (key, oldValue) => bytes);

                if (socialInfo != null)
                {
                    _socialInfoData.AddOrUpdate(id, socialInfo, (key, oldValue) => socialInfo);
                }

                Train();

                return id;
            }
        }

        public Task UpdateAsync(int id, SocialInfo socialInfo)
        {
            if (socialInfo != null)
            {
                _socialInfoData.AddOrUpdate(id, socialInfo, (key, oldValue) => socialInfo);
            }

            return Task.CompletedTask;
        }

        public Task<SocialInfo> QueryAsync(Stream image)
        {
            _faceRecognizer.Read(DataFolder);

            var faceImage = new Image<Gray, byte>(new Bitmap(image));

            var result = _faceRecognizer.Predict(faceImage.Resize(100, 100, Inter.Cubic));

            var id = result.Label;

            if (id > 0 && _socialInfoData.TryGetValue(id, out var socialInfo))
            {
                return Task.FromResult(socialInfo);
            }

            return Task.FromResult<SocialInfo>(null);
        }

        private void Train()
        {
            if (_faceData != null)
            {
                var count = _faceData.Count;

                var counter = 0;

                var faceImages = new Mat[count];

                var faceLabels = new int[count];

                foreach (var item in _faceData)
                {
                    var stream = new MemoryStream();

                    stream.Write(item.Value, 0, item.Value.Length);

                    var faceImage = new Image<Gray, byte>(new Bitmap(stream));

                    faceImages[counter] = faceImage.Resize(100, 100, Inter.Cubic).Mat;

                    faceLabels[counter] = item.Key;

                    counter++;
                }

                _faceRecognizer.Train(faceImages, faceLabels);

                _faceRecognizer.Write(DataFolder);
            }
        }

        private static int NextId()
        {
            var now = DateTime.UtcNow;

            var zeroDate = DateTime.MinValue.AddHours(now.Hour).AddMinutes(now.Minute).AddSeconds(now.Second).AddMilliseconds(now.Millisecond);

            return (int)(zeroDate.Ticks / 10000);
        }
    }
}