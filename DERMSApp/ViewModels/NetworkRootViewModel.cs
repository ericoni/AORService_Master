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

		protected override void LoadChildren()
		{
			foreach (GeographicalRegion geographicalRegion in rdAdapter.GetRegions())
				base.Children.Add(new GeographicalRegionViewModel(geographicalRegion, this, _ders));

		}
		/// <summary>
		/// Poziva se kada treba prikazati sve, bas sve DER-ove. 
		/// Mozda i ovo optimizovati.
		/// </summary>
		protected override void LoadDERS()
		{
			EventSystem.Publish<long>(-1);
			_ders.Clear();

			// to do ubaciti aor proxy, a ne ovako direktno vezanje
			List<long> smGids = new List<long>();

			foreach (var area in aorAreas) // got aorAreas by login
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

			foreach (var der in rdAdapter.GetSyncMachinesByGids(smGids)) // old 1.7. rdAdapter.GetAllDERs())
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
				_ders.Add(item);
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
