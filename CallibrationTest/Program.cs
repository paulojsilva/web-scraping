using Dasync.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace CallibrationTest
{
    class Program
    {
        static HttpClient httpClient = new HttpClient();
        static ConcurrentBag<int> statusCodeBag = new ConcurrentBag<int>();

        static void Main(string[] args)
        {
            Console.WriteLine("Total requests: ");
            int.TryParse(Console.ReadLine(), out int totalRequests);

            Console.WriteLine("processing in parallel: ");
            int.TryParse(Console.ReadLine(), out int maxDegreeOfParallelism);

            string url = "https://localhost:5001/api/WebScraping?url=https://github.com/microsoft/BeanSpy";
            var requests = new List<string>();
            for (int i = 0; i < totalRequests; i++) requests.Add(url);

            var tasks = requests.ParallelForEachAsync(async request =>
            {
                await ProcessAsync(url);

            }, maxDegreeOfParallelism: maxDegreeOfParallelism);

            tasks.ConfigureAwait(false).GetAwaiter().GetResult();

            Console.WriteLine();
            Console.WriteLine("Its done ");
            Console.ReadLine();
        }

        async static Task ProcessAsync(string url)
        {
            var timer = Stopwatch.StartNew();
            var response = await httpClient.GetAsync(url);
            int statusCode = (int)response.StatusCode;

            timer.Stop();
            statusCodeBag.Add(statusCode);

            Console.WriteLine(response.IsSuccessStatusCode ? $"200 OK ({timer.Elapsed.TotalSeconds:N2}s)" : $"{(int)response.StatusCode} {response.StatusCode} ##############");
        }
    }
}
