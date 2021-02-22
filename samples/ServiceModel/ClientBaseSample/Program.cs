using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Contracts;

namespace ClientBaseSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Debug));

            services.AddTransient(sp =>
            {
                var binding = new BasicHttpBinding();

                var endpointAddress = new EndpointAddress("http://localhost:8080/basic");

                return ActivatorUtilities.CreateInstance<TestClient>(sp, binding, endpointAddress);
            });

            services.AddTransient<TestEchoProxyWrapper>();

            await using var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            var client = serviceProvider.GetRequiredService<TestEchoProxyWrapper>();

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

    public class TestClient : ClientBase<ITestService>, ITestService
    {
        public TestClient(Binding binding, EndpointAddress address) : base(binding, address)
        {
        }

        public string SuccessOperation(string message) => Channel.SuccessOperation(message);

        public string FaultyOperation(string message) => Channel.FaultyOperation(message);
    }

    public class TestEchoProxyWrapper : ClientBaseProxyWrapper<ITestService, TestClient>
    {
        public TestEchoProxyWrapper(TestClient client, ILogger<TestEchoProxyWrapper> logger) : base(client, logger) {}
    }
}
