using System;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using InsightArchitectures.Utilities;
using Moq;
using NUnit.Framework;

namespace Tests;

[TestFixture]
[TestOf(typeof(DelegateCustomization<>))]
public class DelegateCustomizationTests
{
    [Test, CustomAutoData]
    public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion) => assertion.Verify(typeof(DelegateCustomization<>).GetConstructors());

    [Test, CustomAutoData]
    public void Customize_invokes_passed_delegate([Frozen] Action<TestType> configurationDelegate, DelegateCustomization<TestType> sut, TestType target)
    {
        sut.Customize(target);

        Mock.Get(configurationDelegate).Verify(p => p(target), Times.Once());
    }
}

public record TestType;
