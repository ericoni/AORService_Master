using DERMSApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Model
{
	public class AORModel : BindableBase
	{
		private string name;
		public AORModel() { }
		public AORModel(string name)
		{
			this.Name = name;
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
	}
}
