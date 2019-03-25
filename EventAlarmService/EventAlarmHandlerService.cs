using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService
{
    /// <summary>
    /// Maybe unnecessary. Alarm ack or event deletion.
    /// </summary>
    public class EventAlarmHandlerService //TODO left to be done
	{
        ServiceHost host = null;
        NetTcpBinding binding = null;
        string address = "net.tcp://localhost:10045/IEventDistribution";

        EventAlarmHandler eventAlarmHandler = null;

        public EventAlarmHandlerService()
        {
            eventAlarmHandler = EventAlarmHandler.Instance;
            binding = new NetTcpBinding();
            InitializeHosts();
        }

        private void InitializeHosts()
        {
            host = new ServiceHost(eventAlarmHandler);
        }
    }
}
