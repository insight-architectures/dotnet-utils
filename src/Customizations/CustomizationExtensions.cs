using System;
using System.Collections.Generic;
using InsightArchitectures.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A set of extensions to enable the usage of <see cref="ICustomization{T}"/>.
/// </summary>
public static class CustomizationExtensions
{
    /// <summary>
    /// Applies all the <paramref name="customizations" /> to the <paramref name="target" />.
    /// </summary>
    /// <param name="customizations">The set of <see cref="ICustomization{T}"/> to apply.</param>
    /// <param name="target">The object to customize.</param>
    /// <typeparam name="T">The type of the object to customize.</typeparam>
    /// <returns>A reference to the instance of type <typeparamref name="T"/> after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
    public static T ApplyTo<T>(this IEnumerable<ICustomization<T>> customizations, T target)
    {
        _ = target ?? throw new ArgumentNullException(nameof(target));

        customizations ??= Array.Empty<ICustomization<T>>();

        foreach (var customization in customizations)
        {
            customization.Customize(target);
        }

        return target;
    }

    /// <summary>
    /// Registers a delegate as a customization for the type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="services">An instance of <see cref="IServiceCollection"/> where to register the customization.</param>
    /// <param name="customization">The delegate to be used for customizing an instance of type <typeparamref name="T"/>.</param>
    /// <typeparam name="T">The type of the object to customize.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection Customize<T>(this IServiceCollection services, Action<T> customization) => services.AddSingleton<ICustomization<T>>(new DelegateCustomization<T>(customization));

    /// <summary>
    /// Resolves an instance of type <typeparamref name="T"/> and applies all registered customizations.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to retrieve the target from.</param>
    /// <typeparam name="T">The type of the object to resolve and customize.</typeparam>
    /// <returns>The instance of type <typeparamref name="T"/> with all registered customizations applied, or null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is null.</exception>
    public static T? GetCustomizedService<T>(this IServiceProvider serviceProvider)
        where T : class
    {
        _ = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        var target = serviceProvider.GetService<T>();

        if (target is null)
        {
            return null;
        }

        var customizations = serviceProvider.GetServices<ICustomization<T>>();

        return ApplyTo(customizations, target);
    }
}
