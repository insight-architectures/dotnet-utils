using System;
using AutoFixture.Idioms;
using InsightArchitectures.Utilities.Internal;
using NUnit.Framework;

namespace Tests.Internal
{
    [TestFixture]
    public class SelectorEqualityComparerTests
    {
        [Test]
        [CustomInlineAutoData(typeof(SelectorEqualityComparer<TypeWithProperty<int>, int>))]
        [CustomInlineAutoData(typeof(SelectorEqualityComparer<TypeWithProperty<string>, string>))]
        public void Constructors_are_guarded(Type type, GuardClauseAssertion assertion) => assertion.Verify(type.GetConstructors());

        [Test]
        [CustomInlineAutoData(typeof(SelectorEqualityComparer<TypeWithProperty<int>, int>))]
        [CustomInlineAutoData(typeof(SelectorEqualityComparer<TypeWithProperty<string>, string>))]
        public void EqualityComparer_is_correctly_implemented(Type type, EqualityComparerAssertion assertion) => assertion.Verify(type);
    }
}
