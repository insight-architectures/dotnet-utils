using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Contracts;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClientBaseSampleFx
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Debug));

            services.AddServiceModelProxy<ITestService, TestClient>()
                    .AddTypedWrapper<ITestClient, ITestService, TestClient, TestProxyWrapper>()
                    .SetBinding(new BasicHttpBinding())
                    .SetEndpointAddress(new Uri("http://localhost:8080/basic"));

            await using var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            var client = serviceProvider.GetRequiredService<ITestClient>();

            try
            {
                for (var i = 0; i < 1_000; i++)
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

    public interface ITestClient : IProxyWrapper<ITestService> { }

    public class TestProxyWrapper : ClientBaseProxyWrapper<ITestService, TestClient>, ITestClient
    {
        public TestProxyWrapper(TestClient client, ILogger<TestProxyWrapper> logger) : base(client, logger) {}
    }
}
