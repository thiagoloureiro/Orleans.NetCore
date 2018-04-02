using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.ConverterContracts;
using Orleans.Runtime;

namespace Orleans.Client
{
    internal class Program
    {
        private static IClusterClient _client;

        private static int Main()
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                _client = await StartClientWithRetries();

                await DoClientWork(_client);
                CreateTimer();

                Console.ReadKey();

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        private static void CreateTimer()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += ATimer_Elapsed1; ;
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
        }

        private static async void ATimer_Elapsed1(object sender, System.Timers.ElapsedEventArgs e)
        {
            await DoClientWork(_client);
        }

        private static async Task<IClusterClient> StartClientWithRetries(int initializeAttemptsBeforeFailing = 5)
        {
            int attempt = 0;
            IClusterClient client;
            while (true)
            {
                try
                {
                    int gatewayPort = 30000;
                    var siloAddress = IPAddress.Loopback;
                    var gateway = new IPEndPoint(siloAddress, gatewayPort);

                    client = new ClientBuilder()
                        .UseLocalhostClustering()
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "AccountTransferApp";
                        })
                        .ConfigureLogging(logging => logging.AddConsole())
                        .Build();

                    await client.Connect();
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(4));
                }
            }

            return client;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            Random rnd = new Random();

            var converterGrain = client.GetGrain<IConverter>(new Guid());

            int value1 = rnd.Next(1000);
            int value2 = rnd.Next(1000);

            var result1 = await converterGrain.ConvertToKm(value1);
            var result2 = await converterGrain.ConvertToMile(value2);

            Console.WriteLine($"Original Value: {value1} Miles Converted Value: {result1} Km");
            Console.WriteLine($"Original Value: {value2} Km Converted Value: {result2} Miles");
        }
    }
}