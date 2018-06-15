using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System.Collections.ObjectModel;
using FTN.ESI.SIMES.CIM.CIMAdapter;
using DERMSApp.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Controls;
using Adapter;

namespace DERMSApp.ViewModels
{
    public class GeographicalRegionViewModel : TreeViewItemViewModel
    {
        readonly GeographicalRegion _region;
		private RDAdapter rdAdapter = new RDAdapter();
        private Visibility reactiveVisibility;

        public ICommand ActivePowerCommand { get; private set; }
        public ICommand ReactivePowerCommand { get; private set; }

        ObservableCollection<TableSMItem> _ders;
        public GeographicalRegionViewModel(GeographicalRegion region, NetworkRootViewModel parent, ObservableCollection<TableSMItem> ders) 
            : base(parent, true)
        {
            ReactiveVisibility = Visibility.Collapsed;

            foreach (SynchronousMachine der in rdAdapter.GetDERs(region.GlobalId))
            {
                if (der.FuelType == FTN.Common.FuelType.Wind)
                {
                    ReactiveVisibility = Visibility.Visible;
                    break;
                }
            }

            _region = region;
            ActivePowerCommand = new RelayCommand(() => ExecuteActivePowerCommand());
            ReactivePowerCommand = new RelayCommand(() => ExecuteReactivePowerCommand());
            _ders = ders;
            _ders.Clear();
        }

        public string GeographicalRegionViewModelName
        {
            get { return _region.Name; }
        }

        public Visibility ReactiveVisibility
        {
            get
            {
                return reactiveVisibility;
            }

            set
            {
                reactiveVisibility = value;
                OnPropertyChanged("ReactiveVisibility");
            }
        }


        protected override void LoadChildren()
        {
			foreach (SubGeographicalRegion subGeographicalRegion in rdAdapter.GetSubRegionsForRegion(_region.GlobalId))
				base.Children.Add(new SubGeographicalRegionViewModel(subGeographicalRegion, this, _ders));
            
		}

        protected override void LoadDERS()
        {
            EventSystem.Publish<long>(_region.GlobalId);
            _ders.Clear();
            foreach (SynchronousMachine der in rdAdapter.GetDERs(_region.GlobalId))
            {
                TableSMItem item = new TableSMItem();
                item = (TableSMItem)CacheReceiver.Instance.TableItemList.Where(o => o.Gid.Equals(der.GlobalId)).FirstOrDefault();
                if (item == null)
                {
                    item = new TableSMItem();
                    item.CurrentP = 0;
                    item.TimeStamp = new DateTime();
                    item.CurrentQ = 0;
                    item.PDecrease = 0;
                    item.PIncrease = 0;
                    item.QDecrease = 0;
                    item.QIncrease = 0;
                }
                item.Der = der;
                _ders.Add(item);
            }
        }

        private void ExecuteReactivePowerCommand()
        {
            _ders.Clear();
            foreach (SynchronousMachine der in rdAdapter.GetDERs(_region.GlobalId))
            {
                TableSMItem item = new TableSMItem();
                item = (TableSMItem)CacheReceiver.Instance.TableItemList.Where(o => o.Gid.Equals(der.GlobalId)).FirstOrDefault();
                if (item == null)
                {
                    item = new TableSMItem();
                    item.CurrentP = 0;
                    item.TimeStamp = new DateTime();
                    item.CurrentQ = 0;
                    item.PDecrease = 0;
                    item.PIncrease = 0;
                    item.QDecrease = 0;
                    item.QIncrease = 0;
                }
                item.Der = der;
                _ders.Add(item);
            }
            base.IsSelected = true;
            EventSystem.Publish<ObservableCollection<TableSMItem>>(_ders);
            EventSystem.Publish<ForecastObjData>(new ForecastObjData() { Gid = _region.GlobalId, Power = false, IsGroup = true });
        }

        private void ExecuteActivePowerCommand()
        {
            _ders.Clear();
            foreach (SynchronousMachine der in rdAdapter.GetDERs(_region.GlobalId))
            {
                TableSMItem item = new TableSMItem();
                item = (TableSMItem)CacheReceiver.Instance.TableItemList.Where(o => o.Gid.Equals(der.GlobalId)).FirstOrDefault();
                if (item == null)
                {
                    item = new TableSMItem();
                    item.CurrentP = 0;
                    item.TimeStamp = new DateTime();
                    item.CurrentQ = 0;
                    item.PDecrease = 0;
                    item.PIncrease = 0;
                    item.QDecrease = 0;
                    item.QIncrease = 0;
                }
                item.Der = der;
                _ders.Add(item);
            }

            base.IsSelected = true;
            EventSystem.Publish<ObservableCollection<TableSMItem>>(_ders);
            EventSystem.Publish<ForecastObjData>(new ForecastObjData() { Gid = _region.GlobalId, Power = true, IsGroup = true });
        }
    }
}
