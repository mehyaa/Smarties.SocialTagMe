using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Framework
{
    public class TagService : ITagService
    {
        private const int FaceRecognizerNumberOfComponents = 0;
        private const int FaceRecognizerThreshold = 15;

        private const int ResizeImageWidth = 400;
        private const int ResizeImageHeight = 300;

        private string DataFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Data");

        private string TrainingDataPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Training.data");

        private readonly FaceRecognizer _faceRecognizer;

        public TagService()
        {
            _faceRecognizer = new EigenFaceRecognizer(FaceRecognizerNumberOfComponents, FaceRecognizerThreshold);

            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }
        }

        public async Task<int> TagAsync(string path, SocialInfo socialInfo = null)
        {
            var existingSocialInfo = await QueryAsync(path);

            if (existingSocialInfo != null)
            {
                return existingSocialInfo.Id;
            }

            var id = NextId();

            var imagePath = Path.Combine(DataFolder, $"{id.ToString()}.image");

            File.Copy(path, imagePath);

            if (socialInfo != null)
            {
                socialInfo.Id = id;

                var infoPath = Path.Combine(DataFolder, $"{id.ToString()}.json");

                var infoJson = JsonConvert.SerializeObject(socialInfo);

                File.WriteAllText(infoPath, infoJson);
            }

            Train();

            return id;
        }

        public Task UpdateAsync(int id, SocialInfo socialInfo)
        {
            var imagePath = Path.Combine(DataFolder, $"{id.ToString()}.image");

            if (File.Exists(imagePath) && socialInfo != null)
            {
                var infoPath = Path.Combine(DataFolder, $"{id.ToString()}.json");

                var infoJson = JsonConvert.SerializeObject(socialInfo);

                File.WriteAllText(infoPath, infoJson);
            }

            return Task.CompletedTask;
        }

        public Task<SocialInfo> QueryAsync(string imagePath)
        {
            if (!File.Exists(TrainingDataPath))
            {
                return Task.FromResult<SocialInfo>(null);
            }

            _faceRecognizer.Read(TrainingDataPath);

            var faceImage = new Image<Gray, byte>(imagePath);

            var result = _faceRecognizer.Predict(faceImage.Resize(ResizeImageWidth, ResizeImageHeight, Inter.Cubic));
            //var result = _faceRecognizer.Predict(faceImage);

            var id = result.Label;

            if (id > 0)
            {
                var infoPath = Path.Combine(DataFolder, $"{id.ToString()}.json");

                if (File.Exists(infoPath))
                {
                    var infoJson = File.ReadAllText(infoPath);

                    var socialInfo = JsonConvert.DeserializeObject<SocialInfo>(infoJson);

                    return Task.FromResult(socialInfo);
                }
            }

            return Task.FromResult<SocialInfo>(null);
        }

        private void Train()
        {
            var files = Directory.GetFiles(DataFolder, "*.image");

            if (files?.Length > 0)
            {
                var count = files.Length;

                var counter = 0;

                var faceImages = new Mat[count];

                var faceLabels = new int[count];

                foreach (var file in files)
                {
                    var faceImage = new Image<Gray, byte>(file);

                    faceImages[counter] = faceImage.Resize(ResizeImageWidth, ResizeImageHeight, Inter.Cubic).Mat;
                    //faceImages[counter] = faceImage.Mat;

                    faceLabels[counter] = int.Parse(Path.GetFileNameWithoutExtension(file));

                    counter++;
                }

                if (File.Exists(TrainingDataPath))
                {
                    _faceRecognizer.Read(TrainingDataPath);
                }

                _faceRecognizer.Train(faceImages, faceLabels);

                _faceRecognizer.Write(TrainingDataPath);
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