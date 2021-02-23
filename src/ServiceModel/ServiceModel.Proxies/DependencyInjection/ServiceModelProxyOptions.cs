using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace InsightArchitectures.Utilities.DependencyInjection
{
    public class ServiceModelProxyOptions
    {
        public Func<IServiceProvider, Binding> BindingFactory { get; set; }

        public Func<IServiceProvider, EndpointAddress> EndpointAddressFactory { get; set; }

        public IList<Action<IServiceProvider, ServiceEndpoint>> EndpointConfigurations { get; } = new List<Action<IServiceProvider, ServiceEndpoint>>();
    }
}
