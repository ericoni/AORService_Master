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
			EventSystem.Publish<long>(-1);
			_ders.Clear();

			// to do ubaciti aor proxy, a ne ovako direktno vezanje. Ne kontam sto je ovakav komentar 20.8.
			List<long> smGids = new List<long>();

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

			List<SynchronousMachine> syncMachines = rdAdapter.GetSyncMachinesByGids(smGids);

			//try
			//{
			//	aorManagementProxy = new AORManagementProxy();
			//	aorManagementProxy.Proxy.
			//}
			//catch (Exception e)
			//{
			//	throw e;
			//}

			foreach (SynchronousMachine der in syncMachines) // old 1.7. rdAdapter.GetAllDERs())
			{// to do 15.8. mislim da je ovde potrebno dodati za setovanje grupe za Table item
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
