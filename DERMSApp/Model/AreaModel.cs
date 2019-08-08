using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.Model
{
    /// <summary>
    /// To be documented.
    /// </summary>
	public class AreaModel : BindableBase
	{
		private string name;
		bool isCheckedForView;//to do redefinisati ime (staviti fino da se zna sta je view, i sta je control
		bool isCovered;

		bool viewStatus;
		bool controlStatus;
		private ObservableCollection<AreaModel> subareas;
		int userCoveringArea;
		public AreaModel()
		{

		}

		public AreaModel(string name)
		{
			this.name = name;
			this.IsCheckedForView = true;
			this.subareas = new ObservableCollection<AreaModel>();
			this.isCovered = false; //do to vrati se na ovo
			this.userCoveringArea = 5;
			this.viewStatus = true;
			this.controlStatus = false;
		}
		public string Name
		{
			get { return name; }
			set
			{
				if (name != value)
				{
					name = value;
					OnPropertyChanged("Name");
				}
			}
		}
		public bool IsCovered
		{
			get { return isCovered; }
			set
			{
				if (isCovered != value)
				{
					isCovered = value;
					OnPropertyChanged("IsCovered");
				}
			}
		}

		public bool ViewStatus
		{
			get { return viewStatus; }
			set
			{
				if (viewStatus != value)
				{
					viewStatus = value;
					OnPropertyChanged("ViewStatus");
				}
			}
		}

		public bool ControlStatus
		{
			get { return controlStatus; }
			set
			{
				if (controlStatus != value)
				{
					controlStatus = value;
					OnPropertyChanged("ControlStatus");
				}
			}
		}

		public int UsersCoveringArea
		{
			get { return 6; }
			//set
			//{
			//    if (isCovered != value)
			//    {
			//        isCovered = value;
			//        OnPropertyChanged("UsersCoveringArea");
			//    }
			//}
		}
		public bool IsCheckedForView
		{
			get { return isCheckedForView; }
			set
			{
				//if (isCheckedForView != value)
				//{
				isCheckedForView = value;
				OnPropertyChanged("IsCheckedForView");

				if (this.SubAreas != null)
				{
					foreach (var subarea in this.SubAreas)
					{
						subarea.IsCheckedForView = value;
					}
				}
				//}
			}
		}

		public ObservableCollection<AreaModel> SubAreas
		{
			get { return subareas; }
			set
			{
				//if (areas != value)
				//{
				subareas = value;
					OnPropertyChanged("SubAreas");
				//}
			}
		}
	}
}
