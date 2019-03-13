using FTN.Common.AORCachedModel;
using FTN.Common.AORModel;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;

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

		public static AORCachedGroup ConvertAORGroupFromNMS(AORGroup nmsAorGroup)
		{
			return new AORCachedGroup(nmsAorGroup.Name, nmsAorGroup.GlobalId);
		}

		public static List<AORCachedSyncMachine> ConvertSyncMachinesFromNMS(List<SynchronousMachine> nmsSyncMachines)
		{
			List<AORCachedSyncMachine> convertedItems = new List<AORCachedSyncMachine>(nmsSyncMachines.Count);
			AORCachedSyncMachine cachedSyncMachine = null;

			foreach (var nmsSMachine in nmsSyncMachines)
			{
				cachedSyncMachine = new AORCachedSyncMachine(nmsSMachine.GlobalId, nmsSMachine.MaxQ, nmsSMachine.MaxP);
				convertedItems.Add(cachedSyncMachine);
			}
			return convertedItems;
		}
	}
}

