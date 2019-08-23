using Adapter;
using AORManagementProxyNS;
using DERMSApp.Model;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.Helpers
{
	public class DERtoTableSMItemConverter
	{
		/// <summary>
		/// To do: to be done later.
		/// </summary>
		/// <param name="_ders"></param>
		/// <param name="smGids"></param>
		public void ConvertToTableItems(ObservableCollection<TableSMItem> _ders, List<long> smGids)
		{
			RDAdapter rdAdapter = new RDAdapter();
			Dictionary<long, List<string>> syncMachineGroupsPair;
			List<SynchronousMachine> syncMachinesFromNMS = rdAdapter.GetSyncMachinesByGids(smGids);
			AORManagementProxy aorManagementProxy = new AORManagementProxy();

			try
			{
				syncMachineGroupsPair = aorManagementProxy.Proxy.GetAORGroupsForSyncMachines(smGids);
			}
			catch (Exception e)
			{
				throw new Exception("Failed to get AOR groups for SyncMachines.", e);
			}

			foreach (SynchronousMachine der in syncMachinesFromNMS) // old 1.7. rdAdapter.GetAllDERs())
			{
				TableSMItem item = new TableSMItem();
				item = (TableSMItem)CacheReceiver.Instance.TableItemList.Where(o => o.Gid.Equals(der.GlobalId)).FirstOrDefault();
				if (item == null)
				{
					item = new TableSMItem();
					item.CurrentP = 0;
					item.TimeStamp = new DateTime();
					item.CurrentQ = 0;
				}
				item.Der = der;

				List<string> assignedGroups;
				syncMachineGroupsPair.TryGetValue(der.GlobalId, out assignedGroups);

				if (assignedGroups != null)
				{
					item.AorGroup = assignedGroups[0];
				}

				_ders.Add(item);//DERs are tableSMItems
			}
		}
	}
}
