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

    public class TestWcfProxyWrapper : ClientBaseProxyWrapper<ITestService, TestClient>
    {
        public TestWcfProxyWrapper(TestClient client, ILogger logger) : base(client, logger)
        {
        }
    }
}
