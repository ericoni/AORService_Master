using FTN.Common.AORCachedModel;
using FTN.Common.AORModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveAORCache
{
	public interface IAORCache
	{
		void SynchronizeAORConfig();
		List<AORGroup> GetAORGroups();
		List<AORCachedArea> GetAORAreas();
	}
}
