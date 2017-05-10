using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace HttpClientAsync
{
    class Program
    {
        private static string getEndpoint = "http://webservice/webservice/hello";
        private static string postEndpoint = "http://webservice/webservice/";
        static void Main(string[] args)
        {
            

            Console.WriteLine("Starting benchmark Run, iterations: 100000, 5 times");
            using (HttpClientExecutor executor = new HttpClientExecutor())
            {
                for (var iter = 0; iter < 5; iter++)
                {
                    var i = 0;
                    var max = 0L;
                    var run = 0L;
                    
                    var times = new Dictionary<long, long>();
                    while (i < 10)
                    {
                        long localRun;
                        localRun = args[0] == "GET" ? executor.ExecuteGet(getEndpoint) : executor.ExecutePost(postEndpoint);
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
                    Console.WriteLine("-----------------------");
                    Console.WriteLine("RUN {0}", iter);
                    Console.WriteLine("------");
                    Console.WriteLine("Number of Runs: {0} ms", i);
                    Console.WriteLine("Average run is {0} ms", run / i);
                    Console.WriteLine("High Request was: {0} ms", max);
                    Console.WriteLine("-------      -------");
                    foreach (var item in times)
                    {
                        Console.WriteLine("Run Time {0} (ms): Occurences: {1}", item.Key, times[item.Key]);
                    }
                    Console.WriteLine("-----------------------");
                }
            }
            Console.WriteLine("End Runs");
           
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

        private HttpClient GetHttpClient(Uri endpoint)
        {
            if (clients.TryGetValue(endpoint.Host, out HttpClient client))
            {
                return client;
            }
            else
            {
                clients.Add(endpoint.Host, new HttpClient());
                return clients[endpoint.Host];
            }
        }
        public long ExecuteGet(string endpoint)
        {
            var sw = new Stopwatch();    
            sw.Start();
            var returnResponse = GetHttpClient(new Uri(endpoint)).GetAsync(endpoint).Result;
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public void Dispose() {
            foreach (var client in clients.Keys) {
              clients[client].Dispose();
            }
        }

        public long ExecutePost(string postEndpoint)
        {
            var url = new Uri(postEndpoint);
            var content = new StringContent(JsonConvert.SerializeObject(new {value = "value"}), Encoding.UTF8, "application/json");
            var sw = new Stopwatch();
            sw.Start();
            var returnString = GetHttpClient(url).PostAsync(url, content).Result;
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
    
}
