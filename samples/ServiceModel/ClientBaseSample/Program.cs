using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

                var endpointAddress = new EndpointAddress("http://localhost:8080");

                return ActivatorUtilities.CreateInstance<EchoClient>(sp, binding, endpointAddress);
            });

            services.AddTransient<EchoClientWrapper>();

            await using var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            var client = serviceProvider.GetRequiredService<EchoClientWrapper>();

            try
            {
                var result = client.Proxy.Echo("Hello world");

                logger.LogInformation($"Result: {result}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while performing a remote call");
            }

        }
    }

    [ServiceContract]
    public interface IEchoService
    {
        [OperationContract]
        string Echo(string message);
    }

    public class EchoClient : ClientBase<IEchoService>, IEchoService
    {
        public EchoClient(Binding binding, EndpointAddress address) : base(binding, address)
        {
        }

        public string Echo(string message) => Channel.Echo(message);
    }

    public class EchoClientWrapper : ClientBaseProxyWrapper<IEchoService, EchoClient>
    {
        public EchoClientWrapper(EchoClient client, ILogger<EchoClientWrapper> logger) : base(client, logger) {}
    }
}
