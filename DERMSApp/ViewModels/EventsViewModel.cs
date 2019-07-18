using DERMSApp.Model;
using FTN.Common.EventAlarm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.ViewModels
{
	/// <summary>
	/// Samo obicni "Event" ga pozove i prikaze "Event 5" u naslovnoj. WTF?!
	/// </summary>
	public class EventsViewModel : BindableBase
	{
		private ObservableCollection<Event> events = null;

		public EventsViewModel()
		{
			List<Event> tempList = new List<Event>(3) { new Event("a", "p", "r"), new Event("a", "p2", "r"), new Event("a", "p3", "r") };
			Events = new ObservableCollection<Event>(tempList);
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
	}
}
