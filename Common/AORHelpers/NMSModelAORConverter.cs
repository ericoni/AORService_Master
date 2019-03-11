using FTN.Common.AORCachedModel;
using FTN.Common.AORModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.AORHelper
{
	public class NMSModelAORConverter
	{
		public static List<AORCachedGroup> ConvertAORGroupsFromNMS(List<AORGroup> nmsAorGroups)
		{
			List<AORCachedGroup> convertedItems = new List<AORCachedGroup>(nmsAorGroups.Count);
			AORCachedGroup tempCachedGroup = null;

			foreach (var nmsGroup in nmsAorGroups)
			{
				tempCachedGroup = new AORCachedGroup(nmsGroup.Name, nmsGroup.GlobalId);
				convertedItems.Add(tempCachedGroup);
			}
			return convertedItems;
		}
	}
}

