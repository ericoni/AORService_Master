using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.Services
{
    [ServiceContract(CallbackContract = typeof(IDeltaNotifyCallback))]
    public interface IDeltaNotify
    {
        [OperationContract]
        void Register();

        [OperationContract]
        void Notify();

        [OperationContract]
        void Unregister();
    }
}
