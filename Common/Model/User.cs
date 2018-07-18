using AORCommon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.Model
{
	public class User
	{
		string username;
		string password;
		List<AORArea> aorAreas;

		public string Username
		{
			get { return username; }
			set { username = value; }
		}

		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		public List<AORArea> AorAreas
		{
			get { return aorAreas; }
			set { aorAreas = value; }
		}
	}
}
