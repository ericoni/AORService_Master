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

namespace TestApp.ViewModels
{
	public class AORManipulationWinVM : ViewModelBase
	{
		AORCacheAccessChannel aorCacheProxy = null;
		ObservableCollection<AORModel> observableAreas = null;

		public AORManipulationWinVM()
		{
			FetchAndShowAORAreas();
		}

		private void FetchAndShowAORAreas()
		{
			aorCacheProxy = new AORCacheAccessChannel();
			HashSet<AORCachedArea> cachedAreas;

			try
			{
				cachedAreas = aorCacheProxy.GetAORAreas();
			}
			catch 
			{
				cachedAreas = new HashSet<AORCachedArea>() { new AORCachedArea() { Name = "prvaTest" }, new AORCachedArea() { Name = "drugaTest" } };
				Trace.Write("FetchAndShowAORAreas exception!");
			}

			observableAreas = new ObservableCollection<AORModel>();

			foreach (var area in cachedAreas)
			{
				observableAreas.Add(new AORModel(area));
			}
			observableAreas[0].UsersCoveringArea = new HashSet<string>();
		}
		public ObservableCollection<AORModel> Areas
		{
			get
			{
				return observableAreas;
			}
			set
			{
				//if (areas != value)
				//{
					observableAreas = value;
					OnPropertyChanged("Areas");
				//}
			}
		}
	}
}
