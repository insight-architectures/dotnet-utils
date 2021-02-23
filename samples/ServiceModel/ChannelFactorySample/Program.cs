﻿using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Contracts;
using InsightArchitectures.Utilities.ServiceModel;

namespace ChannelFactorySample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Debug));

            services.AddServiceModelProxy<ITestService>()
                    .SetBinding(new BasicHttpBinding())
                    .SetEndpointAddress(new Uri("http://localhost:8080/basic"));

            await using var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            var client = serviceProvider.GetRequiredService<IProxyWrapper<ITestService>>();

            try
            {
                for (var i = 0; i < 10_000; i++)
                {
                    var result = client.Proxy.SuccessOperation($"Hello world {i}");

                    logger.LogInformation($"Result: {result}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while performing a remote call");
            }
        }
    }
}
