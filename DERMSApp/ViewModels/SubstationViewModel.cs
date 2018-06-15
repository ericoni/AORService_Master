using FTN.ESI.SIMES.CIM.CIMAdapter;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DERMSApp.Model;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows;
using Adapter;

namespace DERMSApp.ViewModels
{
    public class SubstationViewModel : TreeViewItemViewModel
    {
        readonly Substation _substation;
		private RDAdapter rdAdapter = new RDAdapter();
		ObservableCollection<TableSMItem> _ders;
        private Visibility reactiveVisibility;

        public ICommand ActivePowerCommand { get; private set; }
        public ICommand ReactivePowerCommand { get; private set; }

        public SubstationViewModel(Substation substation, SubGeographicalRegionViewModel parentSubGeographicalRegion, ObservableCollection<TableSMItem> ders)
            : base(parentSubGeographicalRegion, true)
        {

            ReactiveVisibility = Visibility.Collapsed;

            foreach (SynchronousMachine der in rdAdapter.GetDERs(substation.GlobalId))
            {
                if (der.FuelType == FTN.Common.FuelType.Wind)
                {
                    ReactiveVisibility = Visibility.Visible;
                    break;
                }
            }

            _substation = substation;
            ActivePowerCommand = new RelayCommand(() => ExecuteActivePowerCommand());
            ReactivePowerCommand = new RelayCommand(() => ExecuteReactivePowerCommand());
            _ders = ders;
            _ders.Clear();
        }

        public string SubstationName
        {
            get { return _substation.Name; }
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
			foreach (SynchronousMachine synchronousMachine in rdAdapter.GetDERs(_substation.GlobalId))
				base.Children.Add(new SynchronousMachineViewModel(synchronousMachine, this));
		
		}

        protected override void LoadDERS()
        {
            EventSystem.Publish<long>(_substation.GlobalId);
            _ders.Clear();
            foreach (SynchronousMachine der in rdAdapter.GetDERs(_substation.GlobalId))
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
            foreach (SynchronousMachine der in rdAdapter.GetDERs(_substation.GlobalId))
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
            EventSystem.Publish<ForecastObjData>(new ForecastObjData() { Gid = _substation.GlobalId, Power = false, IsGroup = true });
        }

        private void ExecuteActivePowerCommand()
        {
            _ders.Clear();
            foreach (SynchronousMachine der in rdAdapter.GetDERs(_substation.GlobalId))
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
            EventSystem.Publish<ForecastObjData>(new ForecastObjData() { Gid = _substation.GlobalId, Power = true, IsGroup = true });
        }
    }
}
