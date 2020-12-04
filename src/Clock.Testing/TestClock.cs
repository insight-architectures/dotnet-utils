using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// An implementation of <see cref="IClock" /> to be used when authoring unit tests that need to alter the flow of time.
    /// </summary>
    public class TestClock : IClock
    {
        private DateTimeOffset _utcNow;

        /// <summary>
        /// Creates an instance of <see cref="TestClock" /> whose initial time is set to <paramref name="initialTime"/>.
        /// </summary>
        /// <param name="initialTime">The initial time to set the clock to.</param>
        public TestClock(DateTimeOffset initialTime)
        {
            UtcNow = initialTime;
        }

        /// <summary>
        /// Creates an instance of <see cref="TestClock" /> set to the current date and time.
        /// </summary>
        public TestClock() : this (DateTimeOffset.UtcNow)
        {
        }

        /// <summary>
        /// Gets the current value of the point in time.
        /// </summary>
        public DateTimeOffset UtcNow
        {
            get => _utcNow;
            set => _utcNow = value.ToUniversalTime();
        }

        /// <summary>
        /// Advances the inner clock by the value specified in <paramref name="interval" />.
        /// </summary>
        public void AdvanceBy(TimeSpan interval)
        {
            UtcNow += interval;
        }

        /// <summary>
        /// Sets the inner clock to the value of <paramref name="newValue"/>.
        /// </summary>
        /// <param name="newValue"></param>
        public void SetTo(DateTimeOffset newValue)
        {
            UtcNow = newValue;
        }
    }
}
