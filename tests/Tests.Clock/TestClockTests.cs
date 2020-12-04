using System;
using AutoFixture.NUnit3;
using InsightArchitectures.Utilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TestClockTests
    {
        [Test, AutoData]
        public void TestClock_can_be_initialized_with_set_time([Frozen] DateTimeOffset pointInTime, TestClock sut)
        {
            Assert.That(sut.UtcNow, Is.EqualTo(pointInTime.ToUniversalTime()));
        }

        [Test, AutoData]
        public void TestClock_can_be_advanced_by_specified_time_interval([Frozen] DateTimeOffset pointInTime, TestClock sut, TimeSpan advanceBy)
        {
            sut.AdvanceBy(advanceBy);

            Assert.That(sut.UtcNow, Is.EqualTo(pointInTime.ToUniversalTime() + advanceBy));
        }

        [Test, AutoData]
        public void TestClock_can_be_set_to_set_time(TestClock sut, DateTimeOffset pointInTime)
        {
            sut.SetTo(pointInTime);

            Assert.That(sut.UtcNow, Is.EqualTo(pointInTime.ToUniversalTime()));
        }

        [Test, AutoData]
        public void TestClock_can_be_reset_to_current_time(TestClock sut)
        {
            Assume.That(sut.UtcNow, Is.Not.EqualTo(DateTimeOffset.UtcNow.ToUniversalTime()).Within(TimeSpan.FromMilliseconds(100)));

            sut.ResetToNow();

            Assert.That(sut.UtcNow, Is.EqualTo(DateTimeOffset.UtcNow.ToUniversalTime()).Within(TimeSpan.FromMilliseconds(100)));
        }
    }
}