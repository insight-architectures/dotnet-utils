using System;

namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// A service used to abstract the retrieval of date and time.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Gets the current date and time at UTC.
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }
}