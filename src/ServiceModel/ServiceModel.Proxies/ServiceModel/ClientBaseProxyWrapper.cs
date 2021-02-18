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
    public class ClientBaseProxyWrapper<TContract, TClient> : IProxyWrapper<TContract>
        where TContract : class
        where TClient : ClientBase<TContract>, TContract
    {
        private readonly TClient _client;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates an instance of the wrapper around an instance of <see cref="ClientBase{TContract}"/>.
        /// </summary>
        /// <param name="client">A client inheriting from <see cref="ClientBase{TChannel}"/> and implementing <typeparamref name="TContract"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger" />.</param>
        protected ClientBaseProxyWrapper(TClient client, ILogger logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates an instance of the wrapper around an instance of <see cref="ClientBase{TContract}"/>.
        /// </summary>
        /// <param name="client">A client inheriting from <see cref="ClientBase{TChannel}"/> and implementing <typeparamref name="TContract"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger" />.</param>
        public ClientBaseProxyWrapper(TClient client, ILogger<ClientBaseProxyWrapper<TContract, TClient>> logger)
            : this(client, logger as ILogger)
        {
        }

        /// <summary>
        /// The proxy client.
        /// </summary>
        public TContract Proxy
        {
            get
            {
                _client.EnsureChannelIsOpened().Wait();

                return _client;
            }
        }

        /// <summary>
        /// Closes the underlying proxy.
        /// </summary>
        /// <inheritdoc />
        public ValueTask DisposeAsync() => _client.DisposeChannelAsync(_logger);
    }
}
