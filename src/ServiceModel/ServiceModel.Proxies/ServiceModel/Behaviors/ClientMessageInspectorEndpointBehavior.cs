using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace InsightArchitectures.Utilities.ServiceModel.Behaviors
{
    public class ClientMessageInspectorEndpointBehavior : IEndpointBehavior
    {
        private readonly IServiceProvider _serviceProvider;

        public ClientMessageInspectorEndpointBehavior(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var inspectors = _serviceProvider.GetServices<IClientMessageInspector>();

            foreach (var inspector in inspectors)
            {
                clientRuntime.ClientMessageInspectors.Add(inspector);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

        public void Validate(ServiceEndpoint endpoint) { }
    }
}
