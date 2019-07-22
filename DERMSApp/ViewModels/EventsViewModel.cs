using DERMSApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

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
			Events = new ObservableCollection<EventModel>(tempList);

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

        private void ExecuteClearEventsCommand()
        {
            Events = new ObservableCollection<EventModel>();
        }
    }
}
