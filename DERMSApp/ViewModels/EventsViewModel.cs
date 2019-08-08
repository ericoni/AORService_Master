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
		private void ExecuteClearEventsCommand()
		{
			Events = new ObservableCollection<EventModel>();
		}

		private void UpdateEventData(Event e)
		{
			EventModel eventModel =  ConvertEventToModel(e);
			Events.Add(eventModel);
		}

		private EventModel ConvertEventToModel(Event e)
		{
			return new EventModel(e.Username, e.Details, e.Region, e.FieldTimestamp, DateTime.Now);
		}
		#endregion Private methods
	}
}
