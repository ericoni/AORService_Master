using FTN.Common.AORCachedModel;
using FTN.Common.AORModel;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.AORHelpers
{
	public class NMSModelAORConverter
	{
		public static List<AORCachedGroup> ConvertAORGroupsFromNMS(List<AORGroup> nmsAorGroups)
		{
			List<AORCachedGroup> convertedItems = new List<AORCachedGroup>(nmsAorGroups.Count);
			AORCachedGroup cachedGroup = null;

			foreach (var nmsGroup in nmsAorGroups)
			{
				cachedGroup = new AORCachedGroup(nmsGroup.Name, nmsGroup.GlobalId);
				convertedItems.Add(cachedGroup);
			}
			return convertedItems;
		}

		public static List<AORCachedSynchonousMachine> ConvertSyncMachinesFromNMS(List<SynchronousMachine> nmsSyncMachines)
		{
			List<AORCachedSynchonousMachine> convertedItems = new List<AORCachedSynchonousMachine>(nmsSyncMachines.Count);
			AORCachedSynchonousMachine cachedSyncMachine = null;

			foreach (var nmsSMachine in nmsSyncMachines)
			{
				//cachedSyncMachine = new AORCachedSynchonousMachine(nmsSMachine.GlobalId, nmsSMachine.MaxQ, nmsSMachine.MaxP, nmsSMachine.); // TODO: sredi posle dobavljanja full SM info
				convertedItems.Add(cachedSyncMachine);
			}
			return convertedItems;
		}
	}
}

