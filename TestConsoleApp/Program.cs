﻿using ActiveAORCache.Helpers;
using AORManagementProxyNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
	class Program
	{
		static void Main(string[] args) //ima neka fora s ovim, on kreira svoju novu bazu "UsersDatabase7xxxxx" umjesto da koristi istu.
		{
			//var a = AORCacheConfigurations.GetAORAreasForUsername("marko.markovic");//ovako radi normalno

			// AORCacheConfigurations.SelectAreaForView("West-Area", false);

			var c = AORCacheConfigurations.GetPermissionsForArea("West-Area");

			//AORManagementProxy aorManagementProxy = new AORManagementProxy();
			//aorManagementProxy.Proxy.Login("marko.markovic", "a");
		}
	}
}
