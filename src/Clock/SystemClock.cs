using System;

namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// An implementation of <see cref="IClock" /> that abstracts the access to <see cref="DateTimeOffset.UtcNow" />.
    /// </summary>
    public class SystemClock : IClock
    {
        private SystemClock()
        {
        }

        /// <summary>
        /// The default instance of <see cref="SystemClock" />.
        /// </summary>
        public static readonly IClock Instance = new SystemClock();

        /// <inheritdoc/>
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}