using FTN.Common.Model;
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
	[Serializable]
	public class AORCachedArea : AORCachedEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[DataMember]
		public bool IsControllable { get; set; }
		[DataMember]
		public bool IsViewable { get; set; }
		[DataMember]
		public List<AORCachedArea> Areas { get; set;  }
		[DataMember]
		public List<AORCachedGroup> Groups { get; set; }
		[DataMember]
		public List<Permission> Permissions { get; set; }
		[DataMember]
		public List<User> Users { get; set; } // covered by those users
		
		public AORCachedArea() { }
		public AORCachedArea(List<Permission> perms)
		{
			this.Permissions = perms;
		}
		public AORCachedArea(List<Permission> perms, List<User> users)
		{
			this.Permissions = perms;
			this.Users = users;
		}
		public AORCachedArea(List<Permission> perms, List<User> users, List<AORCachedGroup> groups)
		{
			this.Permissions = perms;
			this.Users = users;
			this.Groups = groups;
		}
	}
}
