using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Contracts;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChannelFactorySampleFx
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Debug));

            services.AddSingleton(sp =>
            {
                var binding = new BasicHttpBinding();

                var endpointAddress = new EndpointAddress("http://localhost:8080/basic");

                return ActivatorUtilities.CreateInstance<ChannelFactory<ITestService>>(sp, binding, endpointAddress);
            });

            services.AddTransient<TestEchoProxyWrapper>();

            await using var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            using var client = serviceProvider.GetRequiredService<TestEchoProxyWrapper>();

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

    public class TestEchoProxyWrapper : ChannelFactoryProxyWrapper<ITestService>
    {
        public TestEchoProxyWrapper(ChannelFactory<ITestService> channelFactory, ILogger<TestEchoProxyWrapper> logger) : base(channelFactory, logger) { }
    }
}
