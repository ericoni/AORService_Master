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
			List<EventModel> tempList = new List<EventModel>(3) { new EventModel("a", "p", "r"), new EventModel("a", "User admin has logged in.", "r"), new EventModel("a", "p3", "r") };
            //Events = new ObservableCollection<EventModel>(tempList);
            Events = new ObservableCollection<EventModel>();

            EventSystem.Subscribe<Event>(UpdateEventData);//to do vrati se i zavrsi ovu komunikaciju iz jednog u drugi VM

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

        private void UpdateEventData(Event e)//to do prikazi pristigle podatke na data grid-u
        {
            
        }
        #endregion Private methods
    }
}
