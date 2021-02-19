using System;
using System.ServiceModel;

namespace InsightArchitectures.Utilities.ServiceModel
{
    public interface IChannelFactory<out TContract>
        where TContract : class
    {
        TContract CreateChannel();
    }

    public class ChannelFactoryWrapper<TContract> : IChannelFactory<TContract>
        where TContract : class
    {
        private readonly ChannelFactory<TContract> _channelFactory;

        public ChannelFactoryWrapper(ChannelFactory<TContract> channelFactory)
        {
            _channelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
        }

        public TContract CreateChannel() => _channelFactory.CreateChannel();
    }
}
