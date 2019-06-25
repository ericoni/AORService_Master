using ActiveAORCache.Helpers;
using Adapter;
using AORCommon.Principal;
using AORManagementProxyNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsoleApp
{
	class Program
	{
		static void Main(string[] args) //ima neka fora s ovim, on kreira svoju novu bazu "UsersDatabase7xxxxx" umjesto da koristi istu.
		{
			//var a = AORCacheConfigurations.GetAORAreasForUsername("marko.markovic");//ovako radi normalno

			// AORCacheConfigurations.SelectAreaForView("West-Area", false);

			///var c = AORCacheConfigurations.GetPermissionsForArea("West-Area"); 

			var curretn = ServiceSecurityContext.Current;

			AORManagementProxy aorManagementProxy = new AORManagementProxy();
			bool isValidLogin = aorManagementProxy.Proxy.Login("state", "a");

			var curretn2 = ServiceSecurityContext.Current;

			var principal = Thread.CurrentPrincipal.Identity.Name;
			var p2 = (IMyPrincipal)Thread.CurrentPrincipal;


		//RDAdapter rdAdapter = new RDAdapter();
		//var a = rdAdapter.GetSyncMachinesByGids(new List<long>() { 12884901889 });
		}
	}
}
