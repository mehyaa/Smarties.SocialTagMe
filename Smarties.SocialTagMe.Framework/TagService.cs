using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Framework
{
    public class TagService : ITagService
    {
        private const int FaceRecognizerNumberOfComponents = 1;
        private const double FaceRecognizerThreshold = double.MaxValue / 80;

        private const int ResizeImageWidth = 128;
        private const int ResizeImageHeight = 128;

        private string DataFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Data");

        private string FisherTrainingDataPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Training.Fisher.data");
        private string LBPHTrainingDataPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Training.LBPH.data");

        private readonly FisherFaceRecognizer _fisherFaceRecognizer;
        private readonly LBPHFaceRecognizer _lbphFaceRecognizer;

        public TagService()
        {
            _fisherFaceRecognizer = new FisherFaceRecognizer();
            //_fisherFaceRecognizer = new FisherFaceRecognizer(FaceRecognizerNumberOfComponents, FaceRecognizerThreshold);
            _lbphFaceRecognizer = new LBPHFaceRecognizer();
            //_lbphFaceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 123);

            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }

            if (File.Exists(FisherTrainingDataPath))
            {
                _fisherFaceRecognizer.Read(FisherTrainingDataPath);
            }

            if (File.Exists(LBPHTrainingDataPath))
            {
                _lbphFaceRecognizer.Read(LBPHTrainingDataPath);
            }
        }

        public async Task<int> TagAsync(IEnumerable<string> paths, SocialInfo socialInfo = null, bool train = true)
        {
            var pathArray = paths.ToArray();

            var id = NextId();

            for (int i = 0; i < paths.Count(); i++)
            {
                var path = pathArray[i];

                var imagePath = Path.Combine(DataFolder, $"{id.ToString()}.{i}.image");

                File.Copy(path, imagePath);
            }

            if (socialInfo != null)
            {
                socialInfo.Id = id;

                var infoPath = Path.Combine(DataFolder, $"{id.ToString()}.json");

                var infoJson = JsonConvert.SerializeObject(socialInfo);

                File.WriteAllText(infoPath, infoJson);
            }

            if (train)
            {
                await TrainAsync();
            }

            return id;
        }

        public Task UpdateAsync(int id, SocialInfo socialInfo)
        {
            var files = Directory.GetFiles(DataFolder, "*.image");

            if (files.Any() && socialInfo != null)
            {
                var infoPath = Path.Combine(DataFolder, $"{id.ToString()}.json");

                var infoJson = JsonConvert.SerializeObject(socialInfo);

                File.WriteAllText(infoPath, infoJson);
            }

            return Task.CompletedTask;
        }

        public Task<SocialInfo> QueryAsync(string imagePath)
        {
            var faceImage = new Image<Gray, byte>(imagePath);

            faceImage = faceImage.Resize(ResizeImageWidth, ResizeImageHeight, Inter.Cubic);

            var fisherFaceRecognizerResult = _fisherFaceRecognizer.Predict(faceImage);

            var lbphFaceRecognizerResult = _lbphFaceRecognizer.Predict(faceImage);

            var id = fisherFaceRecognizerResult.Label;

            if (id <= 0)
            {
                id = lbphFaceRecognizerResult.Label;
            }

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

        public Task TrainAsync()
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

                    faceImage = faceImage.Resize(ResizeImageWidth, ResizeImageHeight, Inter.Cubic);

                    faceImages[counter] = faceImage.Mat;

                    faceLabels[counter] = int.Parse(Path.GetFileName(file).Split('.').First());

                    counter++;
                }

                _fisherFaceRecognizer.Train(faceImages, faceLabels);

                _fisherFaceRecognizer.Write(FisherTrainingDataPath);

                _lbphFaceRecognizer.Train(faceImages, faceLabels);

                _lbphFaceRecognizer.Write(LBPHTrainingDataPath);
            }

            return Task.CompletedTask;
        }

        private static int NextId()
        {
            var now = DateTime.UtcNow;

            var zeroDate = DateTime.MinValue.AddHours(now.Hour).AddMinutes(now.Minute).AddSeconds(now.Second).AddMilliseconds(now.Millisecond);

            return (int)(zeroDate.Ticks / 10000);
        }
    }
}