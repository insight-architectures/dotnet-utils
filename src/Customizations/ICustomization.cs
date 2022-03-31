namespace InsightArchitectures.Utilities;

/// <summary>
/// An actionable customization of instances of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type to customize.</typeparam>
public interface ICustomization<in T>
{
    /// <summary>
    /// Customizes the passed instance of <typeparamref name="T"/>.
    /// </summary>
    /// <param name="item">The item to customize.</param>
    void Customize(T item);
}
