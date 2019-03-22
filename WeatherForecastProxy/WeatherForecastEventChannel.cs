using FTN.Common.EventAlarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecastProxyNS
{
	public class WeatherForecastEventChannel : ClientBase<IEvent>, IEvent
	{
		public WeatherForecastEventChannel()
			: base("EventAlarmEndpoint")
		{ }

		public void TestEvent()
		{
			this.Channel.TestEvent();
		}
	}
}



