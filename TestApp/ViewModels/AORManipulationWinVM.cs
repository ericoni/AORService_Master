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
		ObservableCollection<AORModel> obervableAreas = null;

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
				cachedAreas = new HashSet<AORCachedArea>() { new AORCachedArea() { Name = "prva" }, new AORCachedArea() { Name = "druga" } };
				Trace.Write("FetchAndShowAORAreas exception!");
			}

			obervableAreas = new ObservableCollection<AORModel>();

			foreach (var area in cachedAreas)
			{
				obervableAreas.Add(new AORModel(area));
			}
		}
		public ObservableCollection<AORModel> Areas
		{
			get
			{
				return obervableAreas;
			}
			set
			{
				//if (areas != value)
				//{
					obervableAreas = value;
					OnPropertyChanged("Areas");
				//}
			}
		}
	}
}
