using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.Services
{
    /// <summary>
    /// Has a callback <seealso cref="IDeltaNotifyCallback"/> interface. 
    /// Contains methods to register/unregister and notify.
    /// </summary>
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
