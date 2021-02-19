using System;
using System.ServiceModel;

namespace InsightArchitectures.Utilities.ServiceModel
{
    /// <summary>
    /// A wrapper interface around <see cref="ChannelFactory{TChannel}"/>.
    /// </summary>
    /// <typeparam name="TContract">The type of the proxy to create.</typeparam>
    public interface IChannelFactory<out TContract>
        where TContract : class
    {
        /// <summary>
        /// Creates a proxy to a service of type <typeparamref name="TContract"/>.
        /// </summary>
        TContract CreateChannel();
    }

    /// <summary>
    /// A factory of WCF proxies to services of type <typeparamref name="TContract"/>.
    /// </summary>
    /// <typeparam name="TContract">The type of the proxy to create.</typeparam>
    public class ChannelFactoryWrapper<TContract> : IChannelFactory<TContract>
        where TContract : class
    {
        private readonly ChannelFactory<TContract> _channelFactory;

        /// <summary>
        /// Creates an instance of <see cref="ChannelFactoryWrapper{TContract}"/>.
        /// </summary>
        /// <param name="channelFactory">An instance of <see cref="ChannelFactory{TChannel}"/> to create proxies.</param>
        public ChannelFactoryWrapper(ChannelFactory<TContract> channelFactory)
        {
            _channelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
        }

        /// <inheritdoc />
        public TContract CreateChannel() => _channelFactory.CreateChannel();
    }
}
