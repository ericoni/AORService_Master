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
	[DataContract]
	public class AORCachedArea : AORCachedEntity
	{
		//[Key]
		//public string AreaName { get; set; }
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
		//[DataMember]
		//public List<User> Users { get; set; } // covered by those users // vrati se ovde 
		[DataMember]
		public List<User> User { get; set; }

		public AORCachedArea() { }
		public AORCachedArea(List<Permission> perms)
		{
			this.Permissions = perms;
		}
		public AORCachedArea(List<Permission> perms, List<User> users)
		{
			this.Permissions = perms;
			this.User = users;
		}
		public AORCachedArea(string name, string description, List<Permission> perms, List<User> user, List<AORCachedGroup> groups) : base(description, false)
		{
			this.Name = name;
			this.Permissions = perms;
			this.User = user;
			this.Groups = groups;
		}
	}
}
