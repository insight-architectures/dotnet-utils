using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using InsightArchitectures.Utilities.ServiceModel;

namespace InsightArchitectures.Utilities.DependencyInjection
{
    /// <summary>
    /// An options class for configuring instances of <see cref="IProxyWrapper{TContract}"/>.
    /// </summary>
    public class ServiceModelProxyOptions
    {
        /// <summary>
        /// Gets or sets a factory method to create a <see cref="Binding"/> given a <see cref="IServiceProvider"/>.
        /// </summary>
        public Func<IServiceProvider, Binding> BindingFactory { get; set; } = default!;

        /// <summary>
        /// Gets or sets a factory method to create a <see cref="EndpointAddress"/> given a <see cref="IServiceProvider"/>.
        /// </summary>
        public Func<IServiceProvider, EndpointAddress> EndpointAddressFactory { get; set; } = default!;

        /// <summary>
        /// Gets a list of operations used to configure a <see cref="ServiceEndpoint"/>.
        /// </summary>
        public IList<Action<IServiceProvider, ServiceEndpoint>> EndpointConfigurations { get; } = new List<Action<IServiceProvider, ServiceEndpoint>>();
    }
}
