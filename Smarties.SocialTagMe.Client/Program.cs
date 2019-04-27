﻿using System;
using System.IO;
using System.Net.Http;

namespace Smarties.SocialTagMe.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;

            Console.WriteLine("Enter your imageurl: ");

            path = Console.ReadLine();

            byte[] fileBytes = File.ReadAllBytes(path);

            var stream = new MemoryStream(fileBytes);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000");

                var content = new StreamContent(stream);

                var result = client.PostAsync("/api/tag/query", content, default);

                result.Wait();
            }
        }
    }
}
