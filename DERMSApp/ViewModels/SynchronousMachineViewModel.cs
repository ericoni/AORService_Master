using DERMSApp.Model;
using FTN.Services.NetworkModelService.DataModel.Wires;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DERMSApp.ViewModels
{
    public class SynchronousMachineViewModel : TreeViewItemViewModel
    {
        readonly SynchronousMachine _synchronousMachine;

        string synchronousMachineIcon;

        private Visibility reactiveVisibility;

        public ICommand ActivePowerCommand { get; private set; }
        public ICommand ReactivePowerCommand { get; private set; }

        public SynchronousMachineViewModel(SynchronousMachine synchronousMachine, SubstationViewModel parentSubstation)
            : base(parentSubstation, false)
        {
            _synchronousMachine = synchronousMachine;
            if (synchronousMachine.FuelType == FTN.Common.FuelType.Sun)
            {
                SynchronousMachineIcon = @"../Images/SolarPanel.png";
            }
            else if (synchronousMachine.FuelType == FTN.Common.FuelType.Wind)
            {
                SynchronousMachineIcon = @"../Images/Windmill.png";
            }

            if(synchronousMachine.FuelType == FTN.Common.FuelType.Sun)
            {
                ReactiveVisibility = Visibility.Collapsed;
            }
            else
            {
                ReactiveVisibility = Visibility.Visible;
            }

            ActivePowerCommand = new RelayCommand(() => ExecuteActivePowerCommand());
            ReactivePowerCommand = new RelayCommand(() => ExecuteReactivePowerCommand());
        }

        public string SynchronousMachineName
        {
            get { return _synchronousMachine.Name; }
        }

        public string SynchronousMachineIcon
        {
            get
            {
                return synchronousMachineIcon;
            }

            set
            {
                synchronousMachineIcon = value;
                OnPropertyChanged("SynchronousMachineIcon");
            }
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

        protected override void LoadDERS()
        {
            EventSystem.Publish<long>(_synchronousMachine.GlobalId);
        }

        private void ExecuteReactivePowerCommand()
        {
            TableSMItem item = new TableSMItem();
            item = (TableSMItem)CacheReceiver.Instance.TableItemList.Where(o => o.Gid.Equals(_synchronousMachine.GlobalId)).FirstOrDefault();
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
            item.Der = _synchronousMachine;
            base.IsSelected = true;
            EventSystem.Publish<TableSMItem>(item);
            EventSystem.Publish<ForecastObjData>(new ForecastObjData() { Gid = _synchronousMachine.GlobalId, Power = false, IsGroup = false });
        }

        private void ExecuteActivePowerCommand()
        {
            TableSMItem item = new TableSMItem();
            item = (TableSMItem)CacheReceiver.Instance.TableItemList.Where(o => o.Gid.Equals(_synchronousMachine.GlobalId)).FirstOrDefault();
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
            item.Der = _synchronousMachine;
            base.IsSelected = true;
            EventSystem.Publish<TableSMItem>(item);
            EventSystem.Publish<ForecastObjData>(new ForecastObjData() { Gid = _synchronousMachine.GlobalId, Power = true, IsGroup = false });
        }
    }
}
