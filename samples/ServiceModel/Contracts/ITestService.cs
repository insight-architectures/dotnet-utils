using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        string SuccessOperation(string message);

        [OperationContract]
        string FaultyOperation(string message);
    }
}
