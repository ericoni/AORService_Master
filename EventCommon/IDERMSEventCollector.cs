using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventCommon
{
    [ServiceContract]
    public interface IDERMSEventCollector
    {
        [OperationContract]
        void SendEvent(Event e);
    }
}
