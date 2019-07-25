using EventCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventCollectorProxyNS
{
	public class EventCollectorChannel : ClientBase<IDERMSEventCollector>, IDERMSEventCollector
	{
		public EventCollectorChannel()
			: base("EventCollectorEndpoint")
		{
		}
		public void SendEvent(Event e)
		{
			this.Channel.SendEvent(e);
		}
	}
}
