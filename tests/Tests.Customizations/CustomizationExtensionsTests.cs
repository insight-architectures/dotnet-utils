using System;
using System.Collections.Generic;
using InsightArchitectures.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

// ReSharper disable InvokeAsExtensionMethod

namespace Tests;

[TestFixture]
[TestOf(typeof(CustomizationExtensions))]
public class CustomizationExtensionsTests
{
    [TestFixture]
    [TestOf(nameof(CustomizationExtensions.ApplyTo))]
    public class ApplyTo
    {
        [Test, CustomAutoData]
        public void Throws_if_target_is_null(IEnumerable<ICustomization<TestType>> customizations) => Assert.That(() => CustomizationExtensions.ApplyTo(customizations, null!), Throws.ArgumentNullException);

        [Test, CustomAutoData]
        public void Applies_customization_to_target(ICustomization<TestType> customization, TestType target)
        {
            var customizations = new[] { customization };

            CustomizationExtensions.ApplyTo(customizations, target);
            
            Mock.Get(customization).Verify(p => p.Customize(target), Times.Once());
        }
    }
    
    [TestFixture]
    [TestOf(nameof(CustomizationExtensions.Customize))]
    public class Customize
    {
        [Test, CustomAutoData]
        public void Throws_if_delegate_is_null(IServiceCollection services) => Assert.That(() => CustomizationExtensions.Customize(services, (Action<TestType>) null!), Throws.ArgumentNullException);

        [Test, CustomAutoData]
        public void Registers_a_DelegateCustomization(IServiceCollection services, Action<TestType> customization)
        {
            CustomizationExtensions.Customize(services, customization);
            
            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(ICustomization<TestType>) && sd.ImplementationInstance is DelegateCustomization<TestType>)));
        }

        [Test, CustomAutoData]
        public void Customization_can_be_resolved(Action<TestType> customization)
        {
            var serviceProvider = new ServiceCollection().Customize(customization).BuildServiceProvider();

            var registeredCustomization = serviceProvider.GetService<ICustomization<TestType>>();
            
            Assert.That(registeredCustomization, Is.Not.Null);
        }
    }

    [TestFixture]
    [TestOf(nameof(CustomizationExtensions.GetCustomizedService))]
    public class GetCustomizedService
    {
        [Test, CustomAutoData]
        public void Throws_if_serviceProvider_is_null() => Assert.That(() => CustomizationExtensions.GetCustomizedService<TestType>(null!), Throws.ArgumentNullException);

        [Test, CustomAutoData]
        public void Returns_a_registered_instance_of_target(TestType target)
        {
            var serviceProvider = new ServiceCollection().AddSingleton(target).BuildServiceProvider();

            var result = CustomizationExtensions.GetCustomizedService<TestType>(serviceProvider);
            
            Assert.That(result, Is.SameAs(target));
        }

        [Test, CustomAutoData]
        public void Returns_a_registered_instance_with_customizations_applied(TestType target, Action<TestType> customization)
        {
            var serviceProvider = new ServiceCollection().AddSingleton(target).Customize(customization).BuildServiceProvider();
            
            var result = CustomizationExtensions.GetCustomizedService<TestType>(serviceProvider);
            
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.SameAs(target));
                
                Mock.Get(customization).Verify(p => p(target), Times.Once());
            });
        }
    }
}
