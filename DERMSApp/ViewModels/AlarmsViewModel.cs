using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AORCommon.Enumerations;
using DERMSApp.Model;
using GalaSoft.MvvmLight.Command;

namespace DERMSApp.ViewModels
{
	public class AlarmsViewModel : BindableBase
	{
		private ObservableCollection<AlarmModel> alarms = null;
		public ICommand ClearAlarmsCommand { get; private set; }

		public AlarmsViewModel()
		{
			List<AlarmModel> tempList = new List<AlarmModel>(3) {
				new AlarmModel("Area 'Low-Voltage-Zrenjanin-Area' has become uncovered.", "Backa", DateTime.Now.AddMinutes(-6).AddSeconds(-5).AddMilliseconds(-66), SeverityEnumeration.High),
				new AlarmModel("Weather Forecast is not available.", "Backa", DateTime.Now, SeverityEnumeration.Medium)};
			Alarms = new ObservableCollection<AlarmModel>(tempList);
			ClearAlarmsCommand = new RelayCommand(() => ExecuteClearAlarmsCommand());
		}

		public ObservableCollection<AlarmModel> Alarms
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
			Alarms = new ObservableCollection<AlarmModel>();
		}
	}
}
