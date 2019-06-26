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
			//var a = AORCacheConfigurations.GetAORAreasForUsername("marko.markovic");

			// AORCacheConfigurations.SelectAreaForView("West-Area", false);

			///var c = AORCacheConfigurations.GetPermissionsForArea("West-Area"); 

			AORManagementProxy aorManagementProxy = new AORManagementProxy();
			var areas = aorManagementProxy.Proxy.Login("state", "a");

			//aorManagementProxy.Proxy.Test();

			//RDAdapter rdAdapter = new RDAdapter();
			//var a = rdAdapter.GetSyncMachinesByGids(new List<long>() { 12884901889 });
		}
	}
}
