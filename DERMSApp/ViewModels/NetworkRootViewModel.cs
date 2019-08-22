using Adapter;
using DERMSApp.Model;
using FTN.ESI.SIMES.CIM.CIMAdapter;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AORManagementProxyNS;
using System.Threading;
using AORCommon.Principal;
using System.ServiceModel;
using ActiveAORCache.Helpers;
using FTN.Common.AORCachedModel;

namespace DERMSApp.ViewModels
{
	/// <summary>
	/// Contains regions. 
	/// </summary>
	public class NetworkRootViewModel : TreeViewItemViewModel
	{
		private RDAdapter rdAdapter = new RDAdapter();
		ObservableCollection<TableSMItem> _ders;
		List<AORCachedArea> aorAreas = new List<AORCachedArea>();

		public ICommand ActivePowerCommand { get; private set; }
		public ICommand ReactivePowerCommand { get; private set; }

		public NetworkRootViewModel(ObservableCollection<TableSMItem> ders) 
			: base(null, true)
		{
			ActivePowerCommand = new RelayCommand(() => ExecuteActivePowerCommand());
			ReactivePowerCommand = new RelayCommand(() => ExecuteReactivePowerCommand());
			_ders = ders;

		}
		public NetworkRootViewModel(ObservableCollection<TableSMItem> ders, List<AORCachedArea> aorAreas)
	   : base(null, true)
		{
			ActivePowerCommand = new RelayCommand(() => ExecuteActivePowerCommand());
			ReactivePowerCommand = new RelayCommand(() => ExecuteReactivePowerCommand());
			_ders = ders;
			this.aorAreas = aorAreas;
		}

		public string NetworkRootViewModelName
		{
			get { return "Entire network"; }
		}

		/// <summary>
		/// Activates when tree view expander on "Entire network" has been clicked on.
		/// </summary>
		protected override void LoadChildren()
		{
			foreach (GeographicalRegion geographicalRegion in rdAdapter.GetRegions())
				base.Children.Add(new GeographicalRegionViewModel(geographicalRegion, this, _ders));
		}
		/// <summary>
		/// Activates when tree view "Entire network" selection has been made.
		/// Poziva se kada treba prikazati sve, bas sve DER-ove. 
		/// To be optimized.
		/// </summary>
		protected override void LoadDERS()
		{
			AORManagementProxy aorManagementProxy;
			List<long> smGids = new List<long>();
			EventSystem.Publish<long>(-1);
			_ders.Clear();

			foreach (var area in aorAreas) // got aorAreas by login, hardcoded for the moment
			{
				foreach (var group in area.Groups)
				{
					foreach (var sm in group.SynchronousMachines)
					{
						if (!smGids.Contains(sm.GidFromNms))
						{
							smGids.Add(sm.GidFromNms);
						}
					}
				}
			}

			List<SynchronousMachine> syncMachinesFromNMS = rdAdapter.GetSyncMachinesByGids(smGids);
			Dictionary<long, List<string>> syncMachineGroupsPair;

			try
			{
				aorManagementProxy = new AORManagementProxy();
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
				if(item == null)
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

		/// <summary>
		/// To do: This method has to be moved somewhere else to be accessed by multiple view models. 22.8.
		/// </summary>
		/// <param name="syncMachineGroupPair"></param>
		/// <param name="tableSmItem"></param>
		private void SetGroupToTableSmItem(Dictionary<long, List<string>> syncMachineGroupPair, TableSMItem tableSmItem, long derGlobalID)
		{
			List<string> assignedGroups;
			syncMachineGroupPair.TryGetValue(derGlobalID, out assignedGroups);// u kom slucaju se nece poklapati derGlobalID i item.Gid? Posto sam nasao da su isti.

			if (assignedGroups != null)
			{
				tableSmItem.AorGroup = assignedGroups[0];
			}
		}

		private void ExecuteReactivePowerCommand()
		{
			EventSystem.Publish<ForecastObjData>(new ForecastObjData() { Gid = -1, Power = false });
		}

		private void ExecuteActivePowerCommand()
		{
			EventSystem.Publish<ForecastObjData>(new ForecastObjData() { Gid = -1, Power = true });
		}
	}
}
