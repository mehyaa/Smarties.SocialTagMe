using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Smarties.SocialTagMe.Abstractions.Models;
using Smarties.SocialTagMe.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Smarties.SocialTagMe.Framework
{
    public class ImageService : IImageService
    {
        private const int FaceRecognizerNumberOfComponents = 1;
        private const double FaceRecognizerThreshold = double.MaxValue / 80;

        private const int ResizeImageWidth = 128;
        private const int ResizeImageHeight = 128;

        private string DataFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Data");

        private string FisherTrainingDataPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Training.Fisher.data");
        private string LBPHTrainingDataPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Training.LBPH.data");
        private string CascadeClassifierConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties", "haarcascade_frontalface_default.xml");

        private readonly FisherFaceRecognizer _fisherFaceRecognizer;
        private readonly LBPHFaceRecognizer _lbphFaceRecognizer;

        private readonly CascadeClassifier _cascadeClassifier;

        public ImageService()
        {
            _fisherFaceRecognizer = new FisherFaceRecognizer();
            //_fisherFaceRecognizer = new FisherFaceRecognizer(FaceRecognizerNumberOfComponents, FaceRecognizerThreshold);

            _lbphFaceRecognizer = new LBPHFaceRecognizer();
            //_lbphFaceRecognizer = new LBPHFaceRecognizer(1, 8, 32, 32, 256);

            _cascadeClassifier = new CascadeClassifier(CascadeClassifierConfigPath);

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

        public async Task AddAsync(int id, IEnumerable<string> paths, bool train = true)
        {
            var pathArray = paths.ToArray();

            for (int i = 0; i < paths.Count(); i++)
            {
                var path = pathArray[i];

                var imagePath = Path.Combine(DataFolder, $"{id.ToString()}.{i}.image");

                File.Copy(path, imagePath);
            }

            if (train)
            {
                await TrainAsync();
            }
        }

        public async Task<int?> RecognizeAsync(string imagePath)
        {
            await Task.CompletedTask;

            var faceImage = new Image<Gray, byte>(imagePath);

            faceImage = ResizeImage(faceImage);

            //var fisherFaceRecognizerResult = _fisherFaceRecognizer.Predict(faceImage);

            var lbphFaceRecognizerResult = _lbphFaceRecognizer.Predict(faceImage);

            //var id = fisherFaceRecognizerResult.Label;

            //if (id <= 0)
            //{
            //    id = lbphFaceRecognizerResult.Label;
            //}

            var id = lbphFaceRecognizerResult.Label;

            if (id > 0)
            {
                return id;
            }

            return null;
        }

        public async Task TrainAsync()
        {
            await Task.CompletedTask;

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

                    faceImage = ResizeImage(faceImage);

                    faceImages[counter] = faceImage.Mat;

                    faceLabels[counter] = int.Parse(Path.GetFileName(file).Split('.').First());

                    counter++;
                }

                //_fisherFaceRecognizer.Train(faceImages, faceLabels);

                //_fisherFaceRecognizer.Write(FisherTrainingDataPath);

                _lbphFaceRecognizer.Train(faceImages, faceLabels);

                _lbphFaceRecognizer.Write(LBPHTrainingDataPath);
            }
        }

        public async Task<IList<DetectedFaceInfo>> DetectFaceAsync(string imagePath)
        {
            await Task.CompletedTask;

            var extension = Path.GetExtension(imagePath);

            var grayImage = new Image<Gray, byte>(imagePath);

            var detectedFaces = _cascadeClassifier.DetectMultiScale(grayImage);

            var detectedFaceImages = new List<DetectedFaceInfo>();

            foreach (var detectedFace in detectedFaces)
            {
                var detectedFaceImage = grayImage.Copy(detectedFace);

                var path = $"{Path.GetTempFileName()}{extension}";

                detectedFaceImage.Save(path);

                detectedFaceImages.Add(new DetectedFaceInfo
                {
                    Width = detectedFace.Width,
                    Height = detectedFace.Height,
                    Path = path
                });
            }

            return detectedFaceImages;
        }

        private static Image<Gray, byte> ResizeImage(Image<Gray, byte> image)
        {
            if (image.Width > ResizeImageWidth || image.Height > ResizeImageHeight)
            {
                //var widthRatio = (double)ResizeImageWidth / image.Width;

                //var heightRatilo = (double)ResizeImageHeight / image.Height;

                //var newWidth = ResizeImageWidth;

                //var newHeight = ResizeImageHeight;

                //if (widthRatio > heightRatilo)
                //{
                //    newHeight = (int)(image.Height * widthRatio);
                //}
                //else
                //{
                //    newWidth = (int)(image.Width * heightRatilo);
                //}

                //return image.Resize(newWidth, newHeight, Inter.Cubic);

                return image.Resize(ResizeImageWidth, ResizeImageHeight, Inter.Cubic);
            }

            return image;
        }
    }
}