using FTN.Common.EventAlarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmServiceNS
{
	public class EventAlarmCollectorService
	{
		EventAlarmCollector eventCollector = null;
		ServiceHost hostAlarm = null;
		ServiceHost hostEvent = null;
		NetTcpBinding binding = null;
		string address = "net.tcp://localhost:10029/IAlarm";
		string addressEvent = "net.tcp://localhost:10029/IEvent";

		public EventAlarmCollectorService()
		{
			eventCollector = EventAlarmCollector.Instance;
			binding = new NetTcpBinding();
			InitializeHosts();
		}

		private void InitializeHosts()
		{
			hostAlarm = new ServiceHost(eventCollector);
			hostEvent = new ServiceHost(eventCollector);
		}

		public void Dispose()
		{
			CloseHosts();
			GC.SuppressFinalize(this);
		}

		private void CloseHosts()
		{
			if (hostAlarm == null || hostEvent == null)
			{
				throw new Exception("EventCollectorService can not be closed because it is not initialized.");
			}

			hostAlarm.Close();
			hostEvent.Close();

			string message = "EventCollectorService is closed.";
			Console.WriteLine("\n\n{0}", message);
		}

		public void Start()
		{
			StartHosts();
		}

		private void StartHosts()
		{
			if (hostAlarm == null || hostEvent == null)
			{
				throw new Exception("EventCollectorService can not be opened because it is not initialized.");
			}

			string message = string.Empty;

			try
			{
				hostAlarm.AddServiceEndpoint(typeof(IAlarm), binding, address);
				hostEvent.AddServiceEndpoint(typeof(IEvent), binding, addressEvent);

				hostAlarm.Open();
				hostEvent.Open();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			message = string.Format("The WCF services {0} and {1} are ready.", hostAlarm.Description.Name, hostEvent.Description.Name);
			Console.WriteLine(message);

			message = "Endpoints:";
			Console.WriteLine(message);
			Console.WriteLine("{0} {1}", address, addressEvent);

			message = "The EventCollectorService is started.";
			Console.WriteLine("{0}", message);
		}
	}
}
