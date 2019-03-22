using AORCommon.AORContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AORManagementProxyNS;

namespace AORService
{ 
	// [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

	public class AORLoginService : IDisposable
	{
		AORLogin aorLogin = null;
		ServiceHost host = null;
		NetTcpBinding binding = null;
		AOREventAlarmChannel eventProxy = null;
		string address = "net.tcp://localhost:10038/IAORManagement";  

		public AORLoginService()
		{
			aorLogin = AORLogin.Instance;
			binding = new NetTcpBinding();

			InitializeHosts();
		}
		private void InitializeHosts()
		{
			host = new ServiceHost(typeof(AORLogin));
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
				throw new Exception("AOR LoginService can not be opened because it is not initialized.");
			}

			string message = string.Empty;

			try
			{
				host.AddServiceEndpoint(typeof(IAORManagement), binding, address); 
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

			message = "The AOR LoginService is started.";
			Console.WriteLine("{0}", message);

			Console.WriteLine("************************************************************************AOR Login started", message);
		}

		private void CloseHosts()
		{
			if (host == null)
			{
				throw new Exception("AOR LoginService can not be closed because it is not initialized.");
			}

			host.Close();

			string message = "AOR LoginService is closed.";
			Console.WriteLine("\n\n{0}", message);
		}
	}
}
