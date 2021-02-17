using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace InsightArchitectures.Utilities.ServiceModel
{
    /// <summary>
    /// Implementation of <see cref="IProxyWrapper{TContract}"/> based on <see cref="ClientBase{TChannel}"/>.
    /// </summary>
    /// <inheritdoc />
    public abstract class ClientBaseProxyWrapper<TContract, TClient> : IProxyWrapper<TContract>
        where TContract : class
        where TClient : ClientBase<TContract>, TContract
    {
        private readonly TClient _client;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates an instance of the wrapper around an instance of <see cref="ClientBase{TContract}"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        protected ClientBaseProxyWrapper(TClient client, ILogger logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The proxy client.
        /// </summary>
        public TContract Proxy
        {
            get
            {
                switch (_client.State)
                {
                    case CommunicationState.Created:
                        OpenAsync().Wait();
                        break;

                    case CommunicationState.Faulted:
                    case CommunicationState.Closed:
                        throw new InvalidOperationException($"Unable to use the current channel because its state is: {_client.State:G}");
                }

                return _client;
            }
        }

        /// <summary>
        /// Closes the underlying proxy.
        /// </summary>
        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            try
            {
                switch (_client.State)
                {
                    case CommunicationState.Opened:
                        _logger.LogDebug("Closing connection");
                        await CloseAsync().ConfigureAwait(false);
                        break;

                    case CommunicationState.Faulted:
                        _logger.LogDebug("Aborting connection");
                        _client.Abort();
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
                _client.Abort();
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "An error occurred while closing the connection");
                _client.Abort();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while closing the connection");
                _client.Abort();

                throw;
            }
        }

        private Task OpenAsync() => Task.Factory.FromAsync(((ICommunicationObject)_client).BeginOpen(null, null), ((ICommunicationObject)_client).EndOpen);

        private Task CloseAsync() => Task.Factory.FromAsync(((ICommunicationObject)_client).BeginClose(null, null), ((ICommunicationObject)_client).EndClose);
    }
}
