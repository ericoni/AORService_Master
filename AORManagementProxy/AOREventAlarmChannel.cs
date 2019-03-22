using FTN.Common.EventAlarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AORManagementProxyNS
{
	public class AOREventAlarmChannel : ClientBase<IEvent>, IEvent // TODO temp bice samo za evente
	{
		public AOREventAlarmChannel()
			: base("EventAlarmEndpoint")
		{
		}

		public string Test()
		{
			return this.Channel.Test();
		}
	}
}
