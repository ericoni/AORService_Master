using EventCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DERMSEventCollector : IDERMSEventCollector
    {
        public void SendEvent(Event e)
        {
            string a = "a";
        }
    }
}
