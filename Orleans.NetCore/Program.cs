﻿using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Hosting.Development;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Orleans.SiloHost
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "ConverterApp";
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureLogging(logging => logging.AddConsole())
                .AddMemoryGrainStorageAsDefault()
                .UseInClusterTransactionManager()
                .UseInMemoryTransactionLog()
                .UseTransactionalState();

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}