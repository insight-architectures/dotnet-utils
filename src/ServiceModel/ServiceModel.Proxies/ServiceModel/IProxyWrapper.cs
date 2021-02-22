using System;

namespace InsightArchitectures.Utilities.ServiceModel
{
    /// <summary>
    /// A disposable wrapper around a service proxy.
    /// </summary>
    /// <typeparam name="TContract">The type of the service to wrap.</typeparam>
    public interface IProxyWrapper<out TContract> :
#if NETCOREAPP || NETSTANDARD2_1
        IAsyncDisposable
#elif NETFRAMEWORK || NETSTANDARD2_0
        IDisposable
#endif
        where TContract : class
    {
        /// <summary>
        /// The wrapped service proxy.
        /// </summary>
        TContract Proxy { get; }
    }
}
