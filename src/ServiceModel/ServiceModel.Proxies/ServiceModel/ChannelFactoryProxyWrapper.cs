using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace InsightArchitectures.Utilities.ServiceModel
{
    /// <summary>
    /// Implementation of <see cref="IProxyWrapper{TContract}"/> based on <see cref="ChannelFactory{TChannel}"/>.
    /// </summary>
    /// <inheritdoc />
    public class ChannelFactoryProxyWrapper<TContract> : IProxyWrapper<TContract>
        where TContract : class
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Creates an instance of the wrapper around a proxy created by <see cref="ChannelFactory{TContract}"/>.
        /// </summary>
        /// <param name="channelFactory">A channel factory that can create a proxy for <typeparamref name="TContract"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger" />.</param>
        protected ChannelFactoryProxyWrapper(ChannelFactory<TContract> channelFactory, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _proxy = channelFactory?.CreateChannel() ?? throw new ArgumentNullException(nameof(channelFactory));
        }

        /// <summary>
        /// Creates an instance of the wrapper around a proxy created by <see cref="ChannelFactory{TContract}"/>.
        /// </summary>
        /// <param name="channelFactory">A channel factory that can create a proxy for <typeparamref name="TContract"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger" />.</param>
        public ChannelFactoryProxyWrapper(ChannelFactory<TContract> channelFactory, ILogger<ChannelFactoryProxyWrapper<TContract>> logger)
            : this(channelFactory, logger as ILogger)
        {
        }

        private readonly TContract _proxy;

        private ICommunicationObject Channel => _proxy as ICommunicationObject ?? throw new NotSupportedException($"Proxy produced by {nameof(ChannelFactory<TContract>)} does not implement {nameof(ICommunicationObject)}.");

        /// <inheritdoc />
        public TContract Proxy
        {
            get
            {
                Channel.EnsureChannelIsOpened().Wait();

                return _proxy;
            }
        }

#if NETCOREAPP || NETSTANDARD2_1
        /// <summary>
        /// Closes the underlying proxy.
        /// </summary>
        /// <inheritdoc />
        public async ValueTask DisposeAsync() => await Channel.DisposeChannelAsync(_logger).ConfigureAwait(false);
#elif NETFRAMEWORK || NETSTANDARD2_0
        /// <summary>
        /// Closes the underlying proxy.
        /// </summary>
        /// <inheritdoc />
        public void Dispose() => Channel.DisposeChannelAsync(_logger).Wait();
#endif
    }
}
