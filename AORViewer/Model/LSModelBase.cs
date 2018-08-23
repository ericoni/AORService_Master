using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORViewer.Model
{
	public class LBModelBase
	{
		public string Name { get; set; }
		public string Description { get; set;  }

		public LBModelBase(string name)
		{
			this.Name = name;
		}

		public LBModelBase(string name, string desc)
		{
			this.Name = name;
			this.Description = desc;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public enum LBType
	{
		Permissions,
		DNA_Authorities,
		AOR_Groups,
		AOR_Areas
	}
}
