using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using InsightArchitectures.Utilities.Internal;

namespace Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base(FixtureHelpers.CreateFixture)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CustomInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public CustomInlineAutoDataAttribute(params object[] args) : base(FixtureHelpers.CreateFixture, args)
        {
        }
    }

    internal static class FixtureHelpers
    {
        public static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true
            });

            fixture.Register(() => new SelectorEqualityComparer<TypeWithProperty<int>, int>(item => item.Property));

            fixture.Register(() => new SelectorEqualityComparer<TypeWithProperty<string>, string>(item => item.Property));

            return fixture;
        }
    }
}
