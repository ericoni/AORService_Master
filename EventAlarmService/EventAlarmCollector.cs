using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using FTN.Common.EventAlarm;

namespace EventAlarmServiceNS
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

	public class EventAlarmCollector : IAlarm, IEvent
	{
		/// <summary>
		/// Lock object
		/// </summary>
		private static object syncRoot = new Object();

		/// <summary>
		/// Singleton instance
		/// </summary>
		private static volatile EventAlarmCollector instance;

		public EventAlarmCollector()
		{
		}

		/// <summary>
		/// Singleton method
		/// </summary>
		public static EventAlarmCollector Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new EventAlarmCollector();
					}
				}

				return instance;
			}
		}

		public void Test()
		{
			int a = 5;
		}

		public void TestEvent()
		{
			int a = 5;
			int b = 6;
		}
	}
}
