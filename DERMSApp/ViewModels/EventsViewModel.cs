using DERMSApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using EventCommon;

namespace DERMSApp.ViewModels
{
	/// <summary>
	/// Viewmodel for event handling.
	/// </summary>
	public class EventsViewModel : BindableBase
	{
		private ObservableCollection<EventModel> events = null;
		public ICommand ClearEventsCommand { get; private set; }

		public EventsViewModel()
		{
			//List<EventModel> tempList = new List<EventModel>(3) { new EventModel("a", "p", "r", DateTime.Now, DateTime.Now, AORCommon.Enumerations.SeverityEnumeration.High), new EventModel("a", "p3", "r", DateTime.Now, DateTime.Now) };
			//Events = new ObservableCollection<EventModel>(tempList);
			Events = new ObservableCollection<EventModel>();// to do hardcoded for the moment

			EventSystem.Subscribe<Event>(UpdateEventData);

			ClearEventsCommand = new RelayCommand(() => ExecuteClearEventsCommand());
		}

		public ObservableCollection<EventModel> Events
		{
			get
			{
				return events;
			}

			set
			{
				events = value;
				OnPropertyChanged("Events");
			}
		}

		#region Private methods
		/// <summary>
		/// To do vrati se ovde i obrisi hardcode. 
		/// </summary>
		private void ExecuteClearEventsCommand()
		{
			Events = new ObservableCollection<EventModel>();
			EventModel e1 = new EventModel(null, "User 'petar.petrovic' logged in with specified AOR areas: Backa-Area, Zrenjanin-Area, NoviBecej-Area", null, DateTime.Now, DateTime.Now);
			EventModel e2 = new EventModel(null, "New setpoint for Zrenjanin, demanded power: 10, power type: Active and duration: 2h", "Backa", DateTime.Now.AddMinutes(1).AddSeconds(31), DateTime.Now.AddMinutes(1).AddSeconds(31), AORCommon.Enumerations.SeverityEnumeration.Medium);
			EventModel e3 = new EventModel(null, "Setpoint removed for Zrenjanin", "Backa", DateTime.Now.AddMinutes(2).AddSeconds(12), DateTime.Now.AddMinutes(2).AddSeconds(12), AORCommon.Enumerations.SeverityEnumeration.Medium);

			Events = new ObservableCollection<EventModel>(new List<EventModel>(3) { e1, e2, e3 });
		}

		private void UpdateEventData(Event e)
		{
			EventModel eventModel =  ConvertEventToModel(e);
			Events.Add(eventModel);
		}

		private EventModel ConvertEventToModel(Event e)
		{
			return new EventModel(e.Username, e.Details, e.Region, e.FieldTimestamp, DateTime.Now, e.Severity);
		}
		#endregion Private methods
	}
}
