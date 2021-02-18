using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace InsightArchitectures.Utilities.ServiceModel
{
    public static class CommunicationObjectExtensions
    {
        public static Task OpenAsync(this ICommunicationObject communicationObject)
        {
            _ = communicationObject ?? throw new ArgumentNullException(nameof(communicationObject));

            return Task.Factory.FromAsync(communicationObject.BeginOpen(null, null), communicationObject.EndOpen);
        }

        public static Task CloseAsync(this ICommunicationObject communicationObject)
        {
            _ = communicationObject ?? throw new ArgumentNullException(nameof(communicationObject));

            return Task.Factory.FromAsync(communicationObject.BeginClose(null, null), communicationObject.EndClose);
        }
    }
}
