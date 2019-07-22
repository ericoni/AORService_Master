using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AORManagementProxyNS
{
	public class AOREventAlarmChannel : ClientBase<IAlarm>, IAlarm // TODO dodati i za evente
	{
		public AOREventAlarmChannel()
			: base("EventAlarmEndpoint")
		{
		}

		public void Test()
		{
			//this.Channel.Test();
		}
	}
}
