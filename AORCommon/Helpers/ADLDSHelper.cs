using DERMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.Helpers
{
	public class ADLDSHelper
	{
		public static ADLDSHelper Current;

		//private List<AORArea> areas = null;
		//private List<AORGroup> group = null;

		public void GetADLSDConfiguration(out List<AORArea> areas, out List<AORGroup> groups)
		{
			areas = new List<AORArea>();
			groups = new List<AORGroup>();
		}
	}
}
