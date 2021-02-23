using System;
using Microsoft.Extensions.DependencyInjection;

namespace InsightArchitectures.Utilities.DependencyInjection
{
    public interface IServiceModelProxyBuilder
    {
        string Name { get; }

        IServiceCollection Services { get; }
    }

    public class DefaultServiceModelProxyBuilder : IServiceModelProxyBuilder
    {
        public DefaultServiceModelProxyBuilder(string name, IServiceCollection services)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IServiceCollection Services { get; }
    }
}
