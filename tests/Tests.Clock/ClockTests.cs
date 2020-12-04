using AutoFixture.NUnit3;
using InsightArchitectures.Utilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ClockTests
    {
        [Test]
        public void SystemClock_is_default_instance()
        {
            Assert.That(Clock.Default, Is.InstanceOf<SystemClock>());
        }

        [Test, AutoData]
        public void Set_replaces_current_default(TestClock testClock)
        {
            Assume.That(Clock.Default, Is.InstanceOf<SystemClock>());

            Clock.Set(testClock);

            Assert.That(Clock.Default, Is.SameAs(testClock));
        }

        [SetUp]
        public void SetUp()
        {
            Clock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            Clock.Reset();
        }
    }
}