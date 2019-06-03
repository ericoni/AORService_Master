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
			var a = AORCacheConfigurations.GetAORAreasForUsername("marko.markovic");//kada se ovo izbaci, ono staro punjenje DB radi
			//AORCacheConfigurations.SelectAreaForView("West-Area", true);
			//var c = AORCacheConfigurations.GetPermissionsForArea("West-Area");
		}
	}
}
