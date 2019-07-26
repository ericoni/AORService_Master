using EventAlarmProxyNS;
using EventCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				DERMSEventCollector eventCollector = new DERMSEventCollector();

				ServiceHost svc = new ServiceHost(DERMSEventSubscription.Instance);
				//ServiceHost svc = new ServiceHost(dermsEvent);
				//ServiceHost svc = new ServiceHost(typeof(DERMSEvent));
				svc.AddServiceEndpoint(typeof(IDERMSEventSubscription),
				new NetTcpBinding(),
				new Uri(("net.tcp://localhost:10047/IDERMSEvent")));

				ServiceHost svc2 = new ServiceHost(eventCollector);
				svc2.AddServiceEndpoint(typeof(IDERMSEventCollector),
				new NetTcpBinding(),
				new Uri(("net.tcp://localhost:10048/IDERMSEventCollector")));

				svc.Open();
				svc2.Open();

				Console.WriteLine("Press <Enter> to stop the Event service.");
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.WriteLine("Event Service startup failed.");
				Console.ReadLine();
			}
		}

		private void RandomKod()
		{
			//string message = "Starting EventAlarmCollectorService...\n";
			//Console.WriteLine("\n{0}\n", message);
			//EventAlarmCollectorService eventAlarmService = new EventAlarmCollectorService();
			//eventAlarmService.Start();

			//EventAlarmSubscriptionService eventAlarmSubService = new EventAlarmSubscriptionService();
			//eventAlarmSubService.Start();

			/// ovo je stajalo i prije 22.07. comment
			//EventSubscriberCallback eventSubCallback = new EventSubscriberCallback();
			//EventSubscriberProxy eventSubProxy = new EventSubscriberProxy(eventSubCallback);
			//eventSubProxy.SubscribeToAORAreas(new HashSet<long>());
		}
	}
}
