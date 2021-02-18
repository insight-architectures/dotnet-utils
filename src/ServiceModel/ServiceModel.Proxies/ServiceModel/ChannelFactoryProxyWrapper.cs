using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace InsightArchitectures.Utilities.ServiceModel
{
    public abstract class ChannelFactoryProxyWrapper<TContract> : IProxyWrapper<TContract>
        where TContract : class
    {
        private readonly ChannelFactory<TContract> _channelFactory;
        private readonly ILogger _logger;

        protected ChannelFactoryProxyWrapper(ChannelFactory<TContract> channelFactory, ILogger logger)
        {
            _channelFactory = channelFactory ?? throw new ArgumentNullException(nameof (channelFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof (logger));
        }

        private TContract _proxy;

        private ICommunicationObject Channel => _proxy as ICommunicationObject;

        public TContract Proxy
        {
            get
            {
                _proxy ??= _channelFactory.CreateChannel();

                ensureChannelIsOpened(Channel).Wait();

                return _proxy;

                static async Task ensureChannelIsOpened(ICommunicationObject communicationObject)
                {
                    switch (communicationObject?.State)
                    {
                        case null:
                            throw new NotSupportedException();

                        case CommunicationState.Created:
                            await communicationObject.OpenAsync().ConfigureAwait(false);
                            break;

                        case CommunicationState.Faulted:
                        case CommunicationState.Closed:
                            throw new InvalidOperationException($"Unable to use the current channel because its state is: {communicationObject.State:G}");
                    }
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                switch (Channel.State)
                {
                    case CommunicationState.Opened:
                        _logger.LogDebug("Closing connection");
                        await Channel.CloseAsync().ConfigureAwait(false);
                        break;

                    case CommunicationState.Faulted:
                        _logger.LogDebug("Aborting connection");
                        Channel.Abort();
                        break;

                    case CommunicationState.Closed:
                    case CommunicationState.Closing:
                    case CommunicationState.Created:
                    case CommunicationState.Opening:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (CommunicationException ex)
            {
                _logger.LogError(ex, "An error occurred while closing the connection");
                Channel.Abort();
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "An error occurred while closing the connection");
                Channel.Abort();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while closing the connection");
                Channel.Abort();

                throw;
            }
        }
    }
}
