using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace InsightArchitectures.Utilities.ServiceModel
{
    public abstract class WcfProxyWrapper<TContract, TService> : IProxyWrapper<TContract>
        where TContract : class
        where TService : ClientBase<TContract>, TContract
    {
        private readonly TService _service;
        private readonly ILogger _logger;

        protected WcfProxyWrapper(TService service, ILogger logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TContract Service
        {
            get
            {
                EnsureConnectionIsOpen();

                return _service;
            }
        }

        private void EnsureConnectionIsOpen()
        {
            if (_service.State != CommunicationState.Opened)
            {
                OpenAsync().Wait();
            }
        }

#if NETCOREAPP || NETSTANDARD2_1
        /// <inheritdoc />
        public async ValueTask DisposeAsync() => await InnerDisposeAsync().ConfigureAwait(false);
#elif NETFRAMEWORK || NETSTANDARD2_0
        /// <inheritdoc />
        public void Dispose() => InnerDisposeAsync().Wait();
#endif

        private async Task InnerDisposeAsync()
        {
            try
            {
                switch (_service.State)
                {
                    case CommunicationState.Opened:
                        _logger.LogDebug("Closing connection");
                        await CloseAsync().ConfigureAwait(false);
                        break;

                    case CommunicationState.Faulted:
                        _logger.LogDebug("Aborting connection");
                        _service.Abort();
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
                _service.Abort();
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "An error occurred while closing the connection");
                _service.Abort();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while closing the connection");
                _service.Abort();

                throw;
            }
        }

        private Task OpenAsync() => Task.Factory.FromAsync(((ICommunicationObject) _service).BeginOpen(null, null), ((ICommunicationObject) _service).EndOpen);

        private Task CloseAsync() => Task.Factory.FromAsync(((ICommunicationObject) _service).BeginClose(null, null), ((ICommunicationObject) _service).EndClose);
    }
}
