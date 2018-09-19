using FTN.Common.AORCachedModel;
using FTN.Common.AORModel;
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
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public List<DNAAuthority> DNAs { get; set; }
		public List<AORCachedArea> Areas { get; set; }
		//public List<AORCachedArea> ControlAreas { get; set; } //vratiti se jos na ovo kansije kada se bude pravio AOR management window
		//public List<AORCachedArea> ViewAreas { get; set; }

		public User() { }

		public User(string username, string password, List<DNAAuthority> dnas, List<AORCachedArea> areas)//, List<AORCachedArea> controlAreas, List<AORCachedArea> viewAreas)
		{
			this.Username = username;
			this.Password = password;
			this.DNAs = dnas;
			this.Areas = areas;
			this.DNAs = new List<DNAAuthority>();
			//this.ControlAreas = controlAreas;
			//this.ViewAreas = viewAreas;
		}
	}
}
