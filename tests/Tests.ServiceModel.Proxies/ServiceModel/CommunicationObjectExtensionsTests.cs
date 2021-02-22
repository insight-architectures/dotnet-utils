using System;
using System.ServiceModel;
using System.Threading.Tasks;
using InsightArchitectures.Utilities.ServiceModel;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable ConvertClosureToMethodGroup

namespace Tests.ServiceModel
{
    [TestFixture]
    public class CommunicationObjectExtensionsTests
    {
        [Test, CustomAutoData]
        public void OpenAsync_throws_if_communicationObject_is_null() => Assert.That(() => CommunicationObjectExtensions.OpenAsync(null!), Throws.ArgumentNullException);

        [Test, CustomAutoData]
        public async Task OpenAsync_uses_BeginOpen(ICommunicationObject sut)
        {
            await CommunicationObjectExtensions.OpenAsync(sut);

            Mock.Get(sut).Verify(p => p.BeginOpen(It.IsAny<AsyncCallback>(), It.IsAny<object>()));
        }

        [Test, CustomAutoData]
        public void CloseAsync_throws_if_communicationObject_is_null() => Assert.That(() => CommunicationObjectExtensions.CloseAsync(null!), Throws.ArgumentNullException);

        [Test, CustomAutoData]
        public async Task CloseAsync_uses_BeginClose(ICommunicationObject sut)
        {
            await CommunicationObjectExtensions.CloseAsync(sut);

            Mock.Get(sut).Verify(p => p.BeginClose(It.IsAny<AsyncCallback>(), It.IsAny<object>()));
        }

        [Test, CustomAutoData]
        public void EnsureChannelIsOpened_throws_if_communicationObject_is_null() => Assert.That(() => CommunicationObjectExtensions.EnsureChannelIsOpened(null!), Throws.ArgumentNullException);

        [Test, CustomAutoData]
        public async Task EnsureChannelIsOpened_opens_channel_when_created(ICommunicationObject sut)
        {
            Mock.Get(sut).SetupGet(p => p.State).Returns(CommunicationState.Created);

            await CommunicationObjectExtensions.EnsureChannelIsOpened(sut);

            Mock.Get(sut).Verify(p => p.BeginOpen(It.IsAny<AsyncCallback>(), It.IsAny<object>()));
        }

        [Test]
        [CustomInlineAutoData(CommunicationState.Faulted)]
        [CustomInlineAutoData(CommunicationState.Closed)]
        public void EnsureChannelIsOpened_throws_when_unusable(CommunicationState state, ICommunicationObject sut)
        {
            Mock.Get(sut).SetupGet(p => p.State).Returns(state);

            Assert.That(() => CommunicationObjectExtensions.EnsureChannelIsOpened(sut), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        [CustomInlineAutoData(CommunicationState.Opening)]
        [CustomInlineAutoData(CommunicationState.Closing)]
        [CustomInlineAutoData(CommunicationState.Opened)]
        public void EnsureChannelIsOpened_does_nothing_when_ongoing(CommunicationState state, ICommunicationObject sut)
        {
            Mock.Get(sut).SetupGet(p => p.State).Returns(state);

            Mock.Get(sut).Verify(p => p.BeginOpen(It.IsAny<AsyncCallback>(), It.IsAny<object>()), Times.Never());
        }

        [Test, CustomAutoData]
        public void DisposeChannelAsync_throws_if_communicationObject_is_null(ILogger logger) => Assert.That(() => CommunicationObjectExtensions.DisposeChannelAsync(null!, logger), Throws.ArgumentNullException);

        [Test, CustomAutoData]
        public async Task DisposeChannelAsync_invokes_BeginClose_if_channel_is_opened(ICommunicationObject sut, ILogger logger)
        {
            Mock.Get(sut).SetupGet(p => p.State).Returns(CommunicationState.Opened);

            await CommunicationObjectExtensions.DisposeChannelAsync(sut, logger);

            Mock.Get(sut).Verify(p => p.BeginClose(It.IsAny<AsyncCallback>(), It.IsAny<object>()));
        }

        [Test]
        [CustomInlineAutoData(CommunicationState.Faulted)]
        public async Task DisposeChannelAsync_aborts_if_channel_is_not_usable(CommunicationState state, ICommunicationObject sut, ILogger logger)
        {
            Mock.Get(sut).SetupGet(p => p.State).Returns(state);

            await CommunicationObjectExtensions.DisposeChannelAsync(sut, logger);

            Mock.Get(sut).Verify(p => p.Abort());
        }

        [Test, CustomAutoData]
        public async Task DisposeChannelAsync_does_nothing_if_channel_is_created(ICommunicationObject sut, ILogger logger)
        {
            Mock.Get(sut).SetupGet(p => p.State).Returns(CommunicationState.Created);

            await CommunicationObjectExtensions.DisposeChannelAsync(sut, logger);

            Mock.Get(sut).Verify(p => p.BeginClose(It.IsAny<AsyncCallback>(), It.IsAny<object>()), Times.Never());
        }

        [Test, CustomAutoData]
        public async Task DisposeChannelAsync_does_nothing_if_channel_is_closed(ICommunicationObject sut, ILogger logger)
        {
            Mock.Get(sut).SetupGet(p => p.State).Returns(CommunicationState.Closed);

            await CommunicationObjectExtensions.DisposeChannelAsync(sut, logger);

            Mock.Get(sut).Verify(p => p.BeginClose(It.IsAny<AsyncCallback>(), It.IsAny<object>()), Times.Never());
        }

        [Test]
        [CustomInlineAutoData(CommunicationState.Closing)]
        [CustomInlineAutoData(CommunicationState.Opening)]
        public async Task DisposeChannelAsync_does_nothing_if_channel_not_faulted_nor_opened(CommunicationState state, ICommunicationObject sut, ILogger logger)
        {
            Mock.Get(sut).SetupGet(p => p.State).Returns(CommunicationState.Created);

            await CommunicationObjectExtensions.DisposeChannelAsync(sut, logger);

            Mock.Get(sut).Verify(p => p.BeginClose(It.IsAny<AsyncCallback>(), It.IsAny<object>()), Times.Never());
        }
    }
}
