using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.Model
{
	public class AORUser
	{
		public string Password { get; set; }

		public string Permissions { get; set; }

		public List<string> Areas { get; set; }

		public List<string> ControlArea { get; set; }

		public List<string> ViewAreas { get; set; }

		public List<string> ControlAreas { get; set; }

		public DateTime? CheckinTime { get; set; }

		public AORUser()
		{
			Areas = new List<string>();
			ViewAreas = new List<string>();
			ControlArea = new List<string>();
		}
	}
}
