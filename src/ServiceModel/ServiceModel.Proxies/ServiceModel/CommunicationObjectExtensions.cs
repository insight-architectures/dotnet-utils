using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InsightArchitectures.Utilities.ServiceModel
{
    /// <summary>
    /// A set of extension methods around <see cref="ICommunicationObject"/>.
    /// </summary>
    public static class CommunicationObjectExtensions
    {
        /// <summary>
        /// Asynchronously opens the connection of the proxy.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="communicationObject"/> is null.</exception>
        public static Task OpenAsync(this ICommunicationObject communicationObject)
        {
            _ = communicationObject ?? throw new ArgumentNullException(nameof(communicationObject));

            return Task.Factory.FromAsync(communicationObject.BeginOpen(null, null), communicationObject.EndOpen);
        }

        /// <summary>
        /// Asynchronously closes the connection of the proxy.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="communicationObject"/> is null.</exception>
        public static Task CloseAsync(this ICommunicationObject communicationObject)
        {
            _ = communicationObject ?? throw new ArgumentNullException(nameof(communicationObject));

            return Task.Factory.FromAsync(communicationObject.BeginClose(null, null), communicationObject.EndClose);
        }

        /// <summary>
        /// Safely disposes the channel by either closing or aborting it.
        /// </summary>
        /// <param name="communicationObject">The channel to dispose.</param>
        /// <param name="logger">A logger to be used for logging purposes.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="communicationObject"/> is null.</exception>
        public static async Task DisposeChannelAsync(this ICommunicationObject communicationObject, ILogger? logger = null)
        {
            _ = communicationObject ?? throw new ArgumentNullException(nameof(communicationObject));

            logger ??= NullLogger.Instance;

            try
            {
                switch (communicationObject.State)
                {
                    case CommunicationState.Opened:
                        logger.LogDebug("Closing connection");
                        await communicationObject.CloseAsync().ConfigureAwait(false);
                        break;

                    case CommunicationState.Faulted:
                        logger.LogDebug("Aborting connection");
                        communicationObject.Abort();
                        break;

                    case CommunicationState.Closed:
                    case CommunicationState.Closing:
                    case CommunicationState.Created:
                    case CommunicationState.Opening:
                        break;

                    default:
                        throw new InvalidOperationException($"The channel is in an unexpected state: {communicationObject.State:G}");
                }
            }
            catch (CommunicationException ex)
            {
                logger.LogError(ex, "An error occurred while closing the connection");
                communicationObject.Abort();
            }
            catch (TimeoutException ex)
            {
                logger.LogError(ex, "An error occurred while closing the connection");
                communicationObject.Abort();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while closing the connection");
                communicationObject.Abort();

                throw;
            }
        }

        /// <summary>
        /// Ensures the channel is opened.
        /// </summary>
        /// <param name="communicationObject">The channel to open.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="communicationObject"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The channel is in a state such that cannot be opened.</exception>
        public static async Task EnsureChannelIsOpened(this ICommunicationObject communicationObject)
        {
            _ = communicationObject ?? throw new ArgumentNullException(nameof(communicationObject));

            switch (communicationObject.State)
            {
                case CommunicationState.Created:
                    await communicationObject.OpenAsync().ConfigureAwait(false);
                    break;

                case CommunicationState.Faulted:
                case CommunicationState.Closed:
                    throw new InvalidOperationException($"Unable to use the current channel because its state is: {communicationObject.State:G}");
            }
        }
    }
}
