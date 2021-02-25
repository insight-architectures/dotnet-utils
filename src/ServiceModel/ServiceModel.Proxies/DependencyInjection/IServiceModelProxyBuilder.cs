using System;
using System.ServiceModel;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.DependencyInjection;

namespace InsightArchitectures.Utilities.DependencyInjection
{
    /// <summary>
    /// A builder for configuring named instances of <see cref="IProxyWrapper{TContract}"/>.
    /// </summary>
    public interface IServiceModelProxyBuilder
    {
        /// <summary>
        /// Gets the name of the client configured by this builder.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the application service collection.
        /// </summary>
        IServiceCollection Services { get; }
    }

    /// <summary>
    /// A builder for configuring named instances of <see cref="ChannelFactoryProxyWrapper{TContract}" />.
    /// </summary>
    /// <typeparam name="TContract">The type of the service contract to wrap.</typeparam>
    public interface IServiceModelProxyBuilder<TContract> : IServiceModelProxyBuilder
        where TContract : class
    {
    }

    /// <summary>
    /// A builder for configuring named instances of <see cref="ClientBaseProxyWrapper{TContract,TClient}" />.
    /// </summary>
    /// <typeparam name="TContract">The type of the service contract to wrap.</typeparam>
    /// <typeparam name="TProxy">The type of the proxy built from <see cref="ClientBase{TChannel}" />.</typeparam>
    public interface IServiceModelProxyBuilder<TContract, TProxy> : IServiceModelProxyBuilder
        where TContract : class
        where TProxy : ClientBase<TContract>, TContract
    {
    }

    /// <summary>
    /// The default implementation of <see cref="IServiceModelProxyBuilder{TContract}" />.
    /// </summary>
    /// <typeparam name="TContract">The type of the service contract to wrap.</typeparam>
    public class ChannelFactoryServiceModelProxyBuilder<TContract> : IServiceModelProxyBuilder<TContract>
        where TContract : class
    {
        /// <summary>
        /// Creates an instance of <see cref="ChannelFactoryServiceModelProxyBuilder{TContract}" />.
        /// </summary>
        /// <param name="name">The name of the client configured by this builder.</param>
        /// <param name="services">The application service collection.</param>
        public ChannelFactoryServiceModelProxyBuilder(string name, IServiceCollection services)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IServiceCollection Services { get; }
    }

    /// <summary>
    /// The default implementation of <see cref="IServiceModelProxyBuilder{TContract, TProxy}" />.
    /// </summary>
    /// <typeparam name="TContract">The type of the service contract to wrap.</typeparam>
    /// <typeparam name="TProxy">The type of the proxy built from <see cref="ClientBase{TChannel}" />.</typeparam>
    public class ClientBaseServiceModelProxyBuilder<TContract, TProxy> : IServiceModelProxyBuilder<TContract, TProxy>
        where TContract : class
        where TProxy : ClientBase<TContract>, TContract
    {
        /// <summary>
        /// Creates an instance of <see cref="ClientBaseServiceModelProxyBuilder{TContract,TProxy}" />.
        /// </summary>
        /// <param name="name">The name of the client configured by this builder.</param>
        /// <param name="services">The application service collection.</param>
        public ClientBaseServiceModelProxyBuilder(string name, IServiceCollection services)
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
