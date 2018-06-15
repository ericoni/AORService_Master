using FTN.Common;
using FTN.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace FTN.Services.NetworkModelService
{
	public class SmartContainerService : IDisposable
	{
		SmartContainer smartContainer = null;
		ServiceHost host = null;
		NetTcpBinding binding = null;
		string address = "net.tcp://localhost:10010/INMSSubscriber";

		public SmartContainerService()
		{
			smartContainer = SmartContainer.Instance;
			binding = new NetTcpBinding();
			InitializeHosts();
		}

		private void InitializeHosts()
		{
			host = new ServiceHost(smartContainer);
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
				throw new Exception("Smart Container Services can not be closed because it is not initialized.");
			}

			host.Close();

			string message = "The Smart Container Service is closed.";
			CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
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
				throw new Exception("Smart container can not be opend because it is not initialized.");
			}

			string message = string.Empty;

			try
			{
				host.AddServiceEndpoint(typeof(INMSSubscriber), binding, address);
				host.Open();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			message = string.Format("The WCF service {0} is ready.", host.Description.Name);
			Console.WriteLine(message);
			CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

			message = "Endpoints:";
			Console.WriteLine(message);
			Console.WriteLine(address);
			CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

			message = "The Smart Container Service is started.";
			Console.WriteLine("{0}", message);
			CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

			Console.WriteLine("************************************************************************Smart Container sterted", message);
		}
	}
}
