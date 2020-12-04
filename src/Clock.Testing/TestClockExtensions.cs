using System;

namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// A set of extension methods for <see cref="TestClock" />.
    /// </summary>
    public static class TestClockExtensions
    {
        /// <summary>
        /// Resets the <see cref="TestClock" /> to the current point in time.
        /// </summary>
        /// <param name="clock"></param>
        public static void ResetToNow(this TestClock clock)
        {
            _ = clock ?? throw new ArgumentNullException(nameof(clock));

            clock.SetTo(DateTimeOffset.UtcNow);
        }
    }
}
