using ActiveAORCache.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var a = AORCacheConfigurations.GetAORAreasForUsername("marko.markovic");//ovako radi normalno
           // AORCacheConfigurations.SelectAreaForView("West-Area", false);

			//var c = AORCacheConfigurations.GetPermissionsForArea("West-Area");
		}
	}
}
