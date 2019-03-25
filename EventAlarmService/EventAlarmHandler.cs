using FTN.Common.EventAlarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class EventAlarmHandler // TODO left to be done
	{
		/// <summary>
		/// 
		/// </summary>
		private static volatile EventAlarmHandler instance;

		/// <summary>
		/// Lock object
		/// </summary>
		private static object syncRoot = new Object();

		DuplexChannelFactory<IEventDistribution> factory = null;
		IEventDistribution proxy = null;

		/// <summary>
		/// Singleton method
		/// </summary>
		public static EventAlarmHandler Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new EventAlarmHandler();
					}
				}

				return instance;
			}
		}

		public EventAlarmHandler()
		{
			ConnectToTest();
		}

		public void Refresh()
		{
			int a = 5555;
		}

		private void ConnectToTest()
		{
			factory = new DuplexChannelFactory<IEventDistribution>(
			  new InstanceContext(this),
			  new NetTcpBinding(),
			  new EndpointAddress("net.tcp://localhost:10045/IEventDistribution"));
			proxy = factory.CreateChannel();
			proxy.TestEventDistribution();
		}

	}
}
