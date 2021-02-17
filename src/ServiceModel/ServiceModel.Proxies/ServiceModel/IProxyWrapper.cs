using System;

namespace InsightArchitectures.Utilities.ServiceModel
{

    public interface IProxyWrapper<out TContract> :
#if NETCOREAPP || NETSTANDARD2_1
        IAsyncDisposable
#elif NETFRAMEWORK || NETSTANDARD2_0
        IDisposable
#endif
        where TContract : class
    {
        TContract Service { get; }
    }
}
