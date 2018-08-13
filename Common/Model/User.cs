using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.Model
{
	public class User
	{
		//List<AORArea> aorAreas;

		//public List<AORArea> AorAreas
		//{
		//	get { return aorAreas; }
		//	set { aorAreas = value; }
		//}

		private int id;
		public string Username { get; set; }
		public string Password { get; set; }

		public User() { }

		public User(string username, string password)
		{
			this.Username = username;
			this.Password = password;
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}
