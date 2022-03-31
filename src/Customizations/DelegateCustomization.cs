using System;

namespace InsightArchitectures.Utilities;

/// <summary>
/// An implementation of <see cref="ICustomization{T}"/>.
/// </summary>
/// <inheritdoc />
public class DelegateCustomization<T> : ICustomization<T>
{
    private readonly Action<T> _customization;

    /// <summary>
    /// Creates an instance of <see cref="DelegateCustomization{T}"/>.
    /// </summary>
    /// <param name="customization">A delegate to be used to customize instances of <typeparamref name="T"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="customization"/> is null.</exception>
    public DelegateCustomization(Action<T> customization)
    {
        _customization = customization ?? throw new ArgumentNullException(nameof(customization));
    }

    /// <inheritdoc />
    public void Customize(T item) => _customization(item);
}
