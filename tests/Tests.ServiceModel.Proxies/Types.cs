using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
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

    public interface ITestClient : IProxyWrapper<ITestService>
    {
    }

    public class TestClientBaseProxyWrapper : ClientBaseProxyWrapper<ITestService, TestClient>, ITestClient
    {
        public TestClientBaseProxyWrapper(TestClient client, ILogger<TestClientBaseProxyWrapper> logger) : base(client, logger)
        {
        }
    }

    public class TestChannelFactoryProxyWrapper : ChannelFactoryProxyWrapper<ITestService>, ITestClient
    {
        public TestChannelFactoryProxyWrapper(ChannelFactory<ITestService> channelFactory, ILogger<TestChannelFactoryProxyWrapper> logger) : base(channelFactory, logger)
        {
        }
    }

    public class TestClientMessageInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState) => throw new NotImplementedException();

        public object BeforeSendRequest(ref Message request, IClientChannel channel) => throw new NotImplementedException();
    }
}
