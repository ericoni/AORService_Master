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

namespace TestApp.ViewModels
{
	public class AORManipulationWinVM : ViewModelBase
	{
		ObservableCollection<AORModel> areas = null;
		AORCacheAccessChannel aorCacheProxy = null;

		public AORManipulationWinVM()
		{
			areas = new ObservableCollection<AORModel>() { new AORModel("a"), new AORModel("bbbbbbbbbbbbbbb"), new AORModel("cccc") };
			Areas = areas;
			aorCacheProxy = new AORCacheAccessChannel();

			var cacheAreas = aorCacheProxy.GetAORAreas();
		}

		public ObservableCollection<AORModel> Areas
		{
			get
			{
				return areas;
			}
			set
			{
				//if (areas != value)
				//{
					areas = value;
					OnPropertyChanged("Areas");
				//}
			}
		}
	}
}
