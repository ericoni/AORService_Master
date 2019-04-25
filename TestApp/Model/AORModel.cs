using DERMSApp.Model;
using FTN.Common.AORCachedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Model
{
	/// <summary>
	/// Meant for modeling 1 AOR area
	/// </summary>
	public class AORModel : BindableBase
	{
		private string name;
		private bool isCovered;
		private HashSet<string> usersCoveringArea;
		public AORModel() { }
		public AORModel(string name)
		{
			this.Name = name;
			this.IsCovered = false;
			this.usersCoveringArea = new HashSet<string>();
			this.usersCoveringArea.Add("aaa");
		}

		/// <summary>
		/// Not used, yet.
		/// </summary>
		/// <param name="area"></param>
		public AORModel(AORCachedArea area)
		{
			this.Name = area.Name;
			this.IsCovered = false;
			this.usersCoveringArea = new HashSet<string>();
			this.usersCoveringArea.Add("aaa"); // to do sredi kako treba
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
		public HashSet<string> UsersCoveringArea
		{
			get { return usersCoveringArea; }
			set
			{
				//if (usersControllingArea != value)
				//{
				usersCoveringArea = value;
				OnPropertyChanged("UsersCoveringArea");
				//}
			}
		}

		public bool IsCovered
		{
			get { return isCovered; }
			set
			{
				isCovered = value;
				OnPropertyChanged("IsCovered");
			}
		}
	}
}
