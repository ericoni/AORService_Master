using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.AORCachedModel
{
    /// <summary>
    /// Zasto koristim ovaj User, a ne AORUser?
    /// </summary>
    [Serializable]
    [DataContract]
    public class AORCachedUser
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public List<DNAAuthority> DNAs { get; set; }
		public List<AORCachedArea> Areas { get; set; }
        public ICollection<AORCachedUserArea> UserAreas { get; set; }

        //public List<AORCachedArea> ControlAreas { get; set; } // TODO: vratiti se jos na ovo kansije kada se bude pravio AOR management window
        //public List<AORCachedArea> ViewAreas { get; set; }

        public AORCachedUser() { }

		public AORCachedUser(string username, string password, List<DNAAuthority> dnas, List<AORCachedArea> areas)//, List<AORCachedArea> controlAreas, List<AORCachedArea> viewAreas)
		{
			this.Username = username;
			this.Password = password;
			this.DNAs = dnas;
			this.Areas = areas;
			//this.ControlAreas = controlAreas;
			//this.ViewAreas = viewAreas;
		}
	}
}
