using FTN.Common.AORContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AORService
{
	/// <summary>
	/// Service class made for communication with AOR Viewer application.
	/// </summary>
	class AORViewerCommService : IDisposable
	{
		ServiceHost host = null;
		NetTcpBinding binding = null;
		AORViewerComm aorViewerComm = null;

		string address = "net.tcp://localhost:10066/IAORViewerCommunication";

		public AORViewerCommService()
		{
			binding = new NetTcpBinding();
			aorViewerComm = new AORViewerComm();
			host = new ServiceHost(typeof(AORViewerComm));
		}
		
		public void Dispose()
		{
			CloseHosts();
			GC.SuppressFinalize(this);
		}

		public void Start()
		{
			StartHosts();
		}

		private void StartHosts()
		{
			if (host == null)
			{
				throw new Exception("AORViewerCommService can not be opened because it is not initialized.");
			}

			string message = string.Empty;

			try
			{
				host.AddServiceEndpoint(typeof(IAORViewerCommunication), binding, address);
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

			message = "The AORViewerCommService is started.";
			Console.WriteLine("{0}", message);

			Console.WriteLine("************************************************************************AORViewerCommService started", message);
		}

		private void CloseHosts()
		{
			if (host == null)
			{
				throw new Exception("AORViewerCommService can not be closed because it is not initialized.");
			}

			host.Close();

			string message = "AORViewerCommService is closed.";
			Console.WriteLine("\n\n{0}", message);
		}
	}
}
