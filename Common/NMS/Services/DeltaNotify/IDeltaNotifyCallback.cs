using System.ServiceModel;

namespace FTN.Common.Services
{
    [ServiceContract]
    public interface IDeltaNotifyCallback
    {
        [OperationContract]
        void Refresh();
    }
}