﻿using ActiveAORCache.Helpers;
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
			//AORCacheConfigurations.GetAORAreasForUsername("marko.markovic");
			AORCacheConfigurations.SelectAreaForView("West-Area", true);

		}
	}
}
