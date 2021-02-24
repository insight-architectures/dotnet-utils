using System;
using System.Linq;
using System.Reflection;
using AutoFixture.Idioms;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Tests.DependencyInjection
{
    [TestFixture]
    [TestOf(typeof(ServiceModelServiceCollectionExtensions))]
    public class ServiceModelServiceCollectionExtensionsTests
    {
        [Test]
        [CustomInlineAutoData(new[] { typeof(IServiceCollection), typeof(string) }, new[] { typeof (ITestService) })]
        [CustomInlineAutoData(new[] { typeof(IServiceCollection) }, new[] { typeof(ITestService) })]
        [CustomInlineAutoData(new[] { typeof(IServiceCollection), typeof(string) }, new[] { typeof (ITestService), typeof (TestClient) })]
        [CustomInlineAutoData(new[] { typeof(IServiceCollection) }, new[] { typeof(ITestService), typeof(TestClient) })]
        public void AddServiceModelProxy_is_guarded_against_nulls(Type[] parameters, Type[] genericParameterArgumentTypes, GuardClauseAssertion assertion)
        {
            var type = typeof (ServiceModelServiceCollectionExtensions);

            var method = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                             .First(i => i.GetGenericArguments().Length == genericParameterArgumentTypes.Length && i.GetParameters().Length == parameters.Length)!
                             .MakeGenericMethod(genericParameterArgumentTypes);

            assertion.Verify(method);
        }
    }
}
