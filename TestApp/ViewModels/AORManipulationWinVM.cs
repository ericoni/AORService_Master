using AORViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Model;
using System.Windows.Media;
using AORManagementProxyNS;
using FTN.Common.AORCachedModel;
using System.Diagnostics;
using DERMSApp.Model;

namespace TestApp.ViewModels
{
	public class AORManipulationWinVM : ViewModelBase
	{
		AORCacheAccessChannel aorCacheProxy = null;
		ObservableCollection<AORModel> observableAreas = null;
		ObservableCollection<EventModel> events = null;

		public AORManipulationWinVM()
		{
			FetchAndShowAORAreas();
			var tempEvents = new List<EventModel>(3) { new EventModel("a", "poruka1", "region1"), new EventModel("b", "poruka1", "region1"), new EventModel("c", "poruka1", "region1") };
			Events = new ObservableCollection<EventModel>(tempEvents);
		}

		private void FetchAndShowAORAreas()
		{
			aorCacheProxy = new AORCacheAccessChannel();
			HashSet<AORCachedArea> cachedAreas;
			cachedAreas = new HashSet<AORCachedArea>() { new AORCachedArea() { Name = "prvaTest" }, new AORCachedArea() { Name = "drugaTest" } };

			//try
			//{
			//	cachedAreas = aorCacheProxy.GetAORAreas();
			//}
			//catch 
			//{
			//	cachedAreas = new HashSet<AORCachedArea>() { new AORCachedArea() { Name = "prvaTest" }, new AORCachedArea() { Name = "drugaTest" } };
			//	Trace.Write("FetchAndShowAORAreas exception!");
			//}

			observableAreas = new ObservableCollection<AORModel>();

			foreach (var area in cachedAreas)
			{
				observableAreas.Add(new AORModel(area));
			}
			observableAreas[0].UsersCoveringArea = new HashSet<string>();
		}
		public ObservableCollection<EventModel> Events
		{
			get
			{
				return events;
			}
			set
			{
				//if (areas != value)
				//{
				events = value;
				OnPropertyChanged("Events");
				//}
			}
		}

	}
}
