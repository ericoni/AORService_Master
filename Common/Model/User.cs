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
		public DNAAuthority DNAAuthority { get; set; }
		public List<AORCachedArea> ControlAreas { get; set; } 
		public List<AORCachedArea> ViewAreas { get; set; }

		public User() { }

		public User(string username, string password, DNAAuthority dnaAuthority)
		{
			this.Username = username;
			this.Password = password;
			this.DNAAuthority = dnaAuthority;
		}

		public User(string username, string password, DNAAuthority dnaAuthority, List<AORCachedArea> controlAreas, List<AORCachedArea> viewAreas)
		{
			this.Username = username;
			this.Password = password;
			this.DNAAuthority = dnaAuthority;
			this.ControlAreas = controlAreas;
			this.ViewAreas = viewAreas;
		}
	}
}
