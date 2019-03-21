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
		ServiceHost host = null;
		NetTcpBinding binding = null;
		string address = "net.tcp://localhost:10029/IEvent";

		public EventAlarmCollectorService()
		{
			eventCollector = EventAlarmCollector.Instance;
			binding = new NetTcpBinding();
			InitializeHosts();
		}

		private void InitializeHosts()
		{
			host = new ServiceHost(eventCollector);
		}

		public void Dispose()
		{
			CloseHosts();
			GC.SuppressFinalize(this);
		}

		private void CloseHosts()
		{
			if (host == null)
			{
				throw new Exception("EventCollectorService can not be closed because it is not initialized.");
			}

			host.Close();

			string message = "EventCollectorService is closed.";
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
				throw new Exception("EventCollectorService can not be opened because it is not initialized.");
			}

			string message = string.Empty;

			try
			{
				host.AddServiceEndpoint(typeof(IEvent), binding, address);
				host.Open();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			message = string.Format("The WCF service {0} is ready.", host.Description.Name);
			Console.WriteLine(message);

			message = "Endpoints:";
			Console.WriteLine(message);
			Console.WriteLine(address);

			message = "The EventCollectorService is started.";
			Console.WriteLine("{0}", message);

			Console.WriteLine("************************************************************************EventCollectorService started", message);
		}
	}
}
