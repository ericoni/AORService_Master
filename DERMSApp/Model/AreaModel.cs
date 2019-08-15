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
        bool isCheckedForControl;
		bool isCovered;
		bool viewStatus;
		bool controlStatus;
		private ObservableCollection<AreaModel> subareas;
		int numberOfCoveringUsers;
        DateTime timestamp;
		public AreaModel()
		{

		}

        public AreaModel(string name, int numberOfCoveringUsers = 2, bool isCheckedForView = true, bool isCheckedForControl = true, bool isCovered = true)
		{
			this.name = name;
			this.IsCheckedForView = isCheckedForView;
            this.isCheckedForControl = isCheckedForControl;
			this.subareas = new ObservableCollection<AreaModel>();
			this.isCovered = isCovered; //do to vrati se na ovo
			this.NumberOfCoveringUsers = numberOfCoveringUsers;
			this.viewStatus = true;
			this.controlStatus = true;
		}

        /// <summary>
        /// Used for AOR Delegation window.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nubmerOfCoveringUsers"></param>
        public AreaModel(string name, DateTime timestamp, int numberOfCoveringUsers = 0)
        {
            this.name = name;
            this.numberOfCoveringUsers = numberOfCoveringUsers;
            this.timestamp = timestamp;
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

        public int NumberOfCoveringUsers
        {
            get { return numberOfCoveringUsers; }
            set
            {
                if (numberOfCoveringUsers != value)
                {
                    numberOfCoveringUsers = value;
                    OnPropertyChanged("NumberOfCoveringUsers");
                }
            }
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

        public bool IsCheckedForControl
        {
            get { return isCheckedForControl; }
            set
            {
                //if (isCheckedForView != value)
                //{
                isCheckedForControl = value;
                OnPropertyChanged("IsCheckedForControl");

                if (this.SubAreas != null)
                {
                    foreach (var subarea in this.SubAreas)
                    {
                        subarea.IsCheckedForControl = value;
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

        public DateTime Timestamp
        {
            get { return timestamp; }
            set
            {
                //if (areas != value)
                //{
                timestamp = value;
                OnPropertyChanged("Timestamp");
                //}
            }
        }
    }
}
