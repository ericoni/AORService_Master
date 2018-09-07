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
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public List<DNAAuthority> DNAs { get; set; }
		public List<AORCachedArea> ControlAreas { get; set; }
		public List<AORCachedArea> ViewAreas { get; set; }

		public User() { }

		public User(string username, string password, List<DNAAuthority> dnas, List<AORCachedArea> controlAreas, List<AORCachedArea> viewAreas)
		{
			this.Username = username;
			this.Password = password;
			this.DNAs = dnas;
			this.ControlAreas = controlAreas;
			this.ViewAreas = viewAreas;
		}
	}
}
