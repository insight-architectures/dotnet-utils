using System;

namespace InsightArchitectures.Utilities.ServiceModel
{
    /// <summary>
    /// A disposable wrapper around a service proxy.
    /// </summary>
    /// <typeparam name="TContract">The type of the service to wrap.</typeparam>
    public interface IProxyWrapper<out TContract> : IAsyncDisposable
        where TContract : class
    {
        /// <summary>
        /// The wrapped service proxy.
        /// </summary>
        TContract Proxy { get; }
    }
}
