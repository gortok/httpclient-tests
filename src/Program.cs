using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace HttpClientAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            var times = new Dictionary<long, long>();

            Console.WriteLine("Starting benchmark Run, iterations: 100000");
            using (HttpClientExecutor executor = new HttpClientExecutor())
            {
                var i = 0;
                var max = 0L;
                var run = 0L;
                string endpoint = "http://api-test:8080/webservice/hello";
                while (i < 100000)
                {

                    var localRun = executor.Execute(endpoint);
                    if (!times.ContainsKey(localRun))
                    {
                        times.Add(localRun, 1);
                    }
                    else
                    {
                        times[localRun]++;
                    }
                    if (localRun > max)
                    {
                        max = localRun;
                    }
                    run += localRun;
                    i++;
                }
                Console.WriteLine("Number of Runs: {0} ms", i);
                Console.WriteLine("Average run is {0} ms", i / run);
                Console.WriteLine("High Request was: {0} ms", max);

            }
            Console.WriteLine("End Run");
            foreach (var item in times.OrderBy(k => k.Key))
            {
                Console.WriteLine("Run Time {0} (ms): Occurences: {1}", item.Key, times[item.Key]);
            }
        }
    }

    internal class HttpClientExecutor : IDisposable
    {
        private static Dictionary<string, HttpClient> clients { get; }

        static HttpClientExecutor()
        {
            if (clients == null)
            {
                clients = new Dictionary<string, HttpClient>();
            }
        }


        public long Execute(string endpoint)
        {

            if (clients.TryGetValue(endpoint, out HttpClient client))
            {
                    var sw = new Stopwatch();
                   
                    sw.Start();
                    var returnString = client.GetStringAsync(new Uri(endpoint, UriKind.Absolute)).Result;
                    sw.Stop();
                    return sw.ElapsedMilliseconds;
                    
            }
            else
            {
                clients.Add(endpoint, new HttpClient());
                    var sw = new Stopwatch();
                  
                    sw.Start();
                    var returnString = clients[endpoint].GetStringAsync(new Uri(endpoint, UriKind.Absolute)).Result;
                    sw.Stop();
                   
                    return sw.ElapsedMilliseconds;
            }
        }
        public void Dispose() {
            foreach (var client in clients.Keys) {
              clients[client].Dispose();
            }
        }
    }
    
}
