using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.Logging;

namespace Tests
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        string Echo(string message);
    }

    public class TestClient : ClientBase<ITestService>, ITestService
    {
        private readonly Func<string, string> _executor;

        public TestClient(Func<string, string> executor, Binding binding, EndpointAddress endpointAddress) : base(binding, endpointAddress) 
        {
            _executor = executor ?? throw new ArgumentNullException(nameof (executor));
        }

        public string Echo(string message) => _executor(message);
    }

    public class TestClientBaseProxyWrapper : ClientBaseProxyWrapper<ITestService, TestClient>
    {
        public TestClientBaseProxyWrapper(TestClient client, ILogger<TestClientBaseProxyWrapper> logger) : base(client, logger)
        {
        }
    }

    public class TestChannelFactoryProxyWrapper : ChannelFactoryProxyWrapper<ITestService>
    {
        public TestChannelFactoryProxyWrapper(InsightArchitectures.Utilities.ServiceModel.IChannelFactory<ITestService> channelFactory, ILogger<TestChannelFactoryProxyWrapper> logger) : base(channelFactory, logger)
        {
        }
    }
}
