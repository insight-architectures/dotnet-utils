using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using Castle.Core.Logging;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests.ServiceModel
{
    [TestFixture]
    [TestOf(typeof (ClientBaseProxyWrapper<,>))]
    public class ClientBaseProxyWrapperTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion) => assertion.Verify(typeof (TestClientBaseProxyWrapper).GetConstructors());

        [Test, CustomAutoData]
        public void TestFixture_can_reproduce_behavior([Frozen] Func<string, string> func, TestClientBaseProxyWrapper sut, string message)
        {
            Mock.Get(func).Setup(p => p(It.IsAny<string>())).Returns((string arg) => arg);

            var result = sut.Proxy.Echo(message);

            Assert.That(result, Is.EqualTo(message));
        }

        [Test, CustomAutoData]
        public void TestFixture_can_inject_exception([Frozen] Func<string, string> func, TestClientBaseProxyWrapper sut, string message, Exception exception)
        {
            Mock.Get(func).Setup(p => p(It.IsAny<string>())).Throws(exception);

            Assert.That(() => sut.Proxy.Echo(message), Throws.Exception);
        }

        [Test, CustomAutoData]
        public void Client_exposes_inner_client([Frozen] TestClient client, TestClientBaseProxyWrapper sut) => Assert.That(sut.Proxy, Is.SameAs(client));

#if NETCOREAPP3_1 || NET5_0
        [Test, CustomAutoData]
        public async Task Connection_is_closed_when_disposed([Frozen] TestClient client, TestClientBaseProxyWrapper sut, string message)
        {
            Assume.That(client.State, Is.EqualTo(CommunicationState.Created));

            await using (sut)
            {
                _ = sut.Proxy.Echo(message);

                Assume.That(client.State, Is.EqualTo(CommunicationState.Opened));
            }
            
            Assert.That(client.State, Is.EqualTo(CommunicationState.Closed));
        }

        [Test, CustomAutoData]
        public async Task Connection_is_closed_when_disposed_after_failure([Frozen] Func<string, string> func, [Frozen] TestClient client, TestClientBaseProxyWrapper sut, string message, Exception exception)
        {
            Mock.Get(func).Setup(p => p(It.IsAny<string>())).Throws(exception);

            await using (sut)
            {
                try
                {
                    _ = sut.Proxy.Echo(message);

                    Assume.That(client.State, Is.EqualTo(CommunicationState.Opened));

                    _ = sut.Proxy.Echo(message);
                }
                catch (Exception) { }
            }

            Assert.That(client.State, Is.EqualTo(CommunicationState.Closed));

            Mock.Get(func).Verify(p => p(message), Times.Once);
        }
#elif NETFRAMEWORK || NETCOREAPP2_1
        [Test, CustomAutoData]
        public void Connection_is_closed_when_disposed([Frozen] TestClient client, TestClientBaseProxyWrapper sut, string message)
        {
            Assume.That(client.State, Is.EqualTo(CommunicationState.Created));

            using (sut)
            {
                _ = sut.Proxy.Echo(message);

                Assume.That(client.State, Is.EqualTo(CommunicationState.Opened));
            }

            Assert.That(client.State, Is.EqualTo(CommunicationState.Closed));
        }

        [Test, CustomAutoData]
        public void Connection_is_closed_when_disposed_after_failure([Frozen] Func<string, string> func, [Frozen] TestClient client, TestClientBaseProxyWrapper sut, string message, Exception exception)
        {
            Mock.Get(func).Setup(p => p(It.IsAny<string>())).Throws(exception);

            using (sut)
            {
                try
                {
                    _ = sut.Proxy.Echo(message);

                    Assume.That(client.State, Is.EqualTo(CommunicationState.Opened));

                    _ = sut.Proxy.Echo(message);
                }
                catch (Exception) {}
            }

            Assert.That(client.State, Is.EqualTo(CommunicationState.Closed));

            Mock.Get(func).Verify(p => p(message), Times.Once);
        }
#endif
    }
}
