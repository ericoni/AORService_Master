using DERMSApp.Model;
using FTN.Common.EventAlarm;
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
		private ObservableCollection<Event> events = null;
        public ICommand ClearEventsCommand { get; private set; }

        public EventsViewModel()
		{
			List<Event> tempList = new List<Event>(3) { new Event("a", "p", "r"), new Event("a", "User admin has logged in.", "r"), new Event("a", "p3", "r") };
			Events = new ObservableCollection<Event>(tempList);

            ClearEventsCommand = new RelayCommand(() => ExecuteClearEventsCommand());
        }

        public ObservableCollection<Event> Events
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
            Events = new ObservableCollection<Event>();
        }
    }
}
