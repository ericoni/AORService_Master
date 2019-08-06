using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DERMSApp.Model;
using GalaSoft.MvvmLight.Command;

namespace DERMSApp.ViewModels
{
    public class AlarmsViewModel : BindableBase
    {
        private ObservableCollection<Alarm> alarms = null;
        public ICommand ClearAlarmsCommand { get; private set; }

        public AlarmsViewModel()
        {
            List<Alarm> tempList = new List<Alarm>(3) { new Alarm("a", "p"), new Alarm("a", "p2"), new Alarm("a", "p3", AlarmSeverity.High) };
            Alarms = new ObservableCollection<Alarm>(tempList);

            Alarms[0].Severity = AlarmSeverity.Medium;
            ClearAlarmsCommand = new RelayCommand(() => ExecuteClearAlarmsCommand());
        }

        public ObservableCollection<Alarm> Alarms
        {
            get
            {
                return alarms;
            }

            set
            {
                alarms = value;
                OnPropertyChanged("Alarms");
            }
        }

        private void ExecuteClearAlarmsCommand()
        {
            Alarms = new ObservableCollection<Alarm>();
        }
    }
}
