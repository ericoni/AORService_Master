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

namespace DERMSApp.ViewModels
{
    public class NetworkRootViewModel : TreeViewItemViewModel
    {
		private RDAdapter rdAdapter = new RDAdapter();
		ObservableCollection<TableSMItem> _ders;

        public ICommand ActivePowerCommand { get; private set; }
        public ICommand ReactivePowerCommand { get; private set; }

        public NetworkRootViewModel(ObservableCollection<TableSMItem> ders) 
            : base(null, true)
        {
            ActivePowerCommand = new RelayCommand(() => ExecuteActivePowerCommand());
            ReactivePowerCommand = new RelayCommand(() => ExecuteReactivePowerCommand());
            _ders = ders;

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

        protected override void LoadDERS()
        {
            EventSystem.Publish<long>(-1);
            _ders.Clear();

            // to do ubaciti aor proxy

			foreach(SynchronousMachine der in rdAdapter.GetAllDERs())
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
