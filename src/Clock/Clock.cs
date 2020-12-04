using System;

namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// A static accessor to <see cref="IClock" />.
    /// </summary>
    public static class Clock
    {
        /// <summary>
        /// Gets the current default instance of <see cref="IClock" />.
        /// </summary>
        public static IClock Default { get; private set; } = SystemClock.Instance;

        /// <summary>
        /// Sets the default instance of <see cref="IClock" /> to <paramref name="clock"/>.
        /// </summary>
        /// <param name="clock">The implementation of <see cref="IClock" /> to be used. Useful when testing time-sensitive components.</param>
        public static void Set(IClock clock)
        {
            Default = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        /// <summary>
        /// Resets the default instance of <see cref="IClock" /> to <see cref="SystemClock" />.
        /// </summary>
        public static void Reset() => Set(SystemClock.Instance);
    }
}