using FTN.Common.EventAlarm.EventSubscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService.Subscriptions
{
	/// <summary>
	/// Alarm ack or event deletion.
	/// </summary>
	public class EventAlarmSubscriptionService
	{
		ServiceHost host = null;
		NetTcpBinding binding = null;
		string address = "net.tcp://localhost:10046/IEventSubscription";

		EventAlarmSubscriptionHandler eventAlarmSubscriptionHandler = null;

		public EventAlarmSubscriptionService()
		{
			eventAlarmSubscriptionHandler = EventAlarmSubscriptionHandler.Instance;
			binding = new NetTcpBinding();
			InitializeHosts();
		}

		private void InitializeHosts()
		{
			host = new ServiceHost(eventAlarmSubscriptionHandler);
		}

		private void CloseHosts()
		{
			if (host == null)
			{
				throw new Exception("EventAlarmSubscriptionService can not be closed because it is not initialized.");
			}

			host.Close();

			string message = "EventAlarmSubscriptionService is closed.";
			Console.WriteLine("\n\n{0}", message);
		}

		public void Start()
		{
			StartHosts();
		}

		private void StartHosts()
		{
			if (host == null)
			{
				throw new Exception("EventAlarmSubscriptionService can not be opened because it is not initialized.");
			}

			string message = string.Empty;

			try
			{
				host.AddServiceEndpoint(typeof(IEventSubscription), binding, address);

				host.Open();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			message = string.Format("The WCF services {0} is ready.", host.Description.Name);
			Console.WriteLine(message);

			message = "Endpoints:";
			Console.WriteLine(message);
			Console.WriteLine("{0}", address);

			message = "The EventAlarmSubscriptionService is started.";
			Console.WriteLine("{0}", message);
		}
	}
}
