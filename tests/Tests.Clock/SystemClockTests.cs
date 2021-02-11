using System;
using AutoFixture.NUnit3;
using InsightArchitectures.Utilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class SystemClockTests
    {
        [Test, AutoData]
        public void UtcNow_returns_current_time()
        {
            Assert.That(SystemClock.Instance.UtcNow, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
        }
    }
}
