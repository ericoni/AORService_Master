using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace SmartCacheLibrary.Interfaces
{
    /// <summary>
    /// GPH Quick Message Service Operations
    /// </summary>
    [ServiceContract(CallbackContract = typeof(ICacheServiceCallback))]
    public interface ICacheService
    {
        [OperationContract]
        int Register(string userName);

        [OperationContract(IsOneWay = true)]
        void ReceiveCache(string userName, List<string> addressList);

        [OperationContract]
        int Unregister(string userName);
    }
}
