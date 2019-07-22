using EventAlarmProxyNS;
using EventAlarmService.Subscriptions;
using EventAlarmServiceNS;
using FTN.Common.EventAlarm;
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

				DERMSEventSubscription dermsEvent = new DERMSEventSubscription();

				ServiceHost svc = new ServiceHost(dermsEvent);
				//ServiceHost svc = new ServiceHost(typeof(DERMSEvent));
				svc.AddServiceEndpoint(typeof(IDERMSEventSubscription),
				new NetTcpBinding(),
				new Uri(("net.tcp://localhost:10047/IDERMSEvent")));

				svc.Open();

				Console.WriteLine("Press Enter to send msg to clients...");
				Console.Read();
				dermsEvent.NotifyClients(7, "Notify received!");

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
	}
}
