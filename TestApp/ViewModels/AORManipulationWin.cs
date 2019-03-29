using AORViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Model;

namespace TestApp.ViewModels
{
	public class AORManipulationWin : ViewModelBase
	{
		ObservableCollection<AORModel> areas;
		public AORManipulationWin()
		{
			areas = new ObservableCollection<AORModel>() { new AORModel("a"), new AORModel("bbbbbbbbbbbbbbb"), new AORModel("cccc") };
			Areas = areas;
			//aorModel.AORAreas = new List<string>(3) { "prva", "druga", "treca" };
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
