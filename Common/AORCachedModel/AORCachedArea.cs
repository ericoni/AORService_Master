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
	[KnownType(typeof(User))]
	public class AORCachedArea : AORCachedEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int AreaId { get; set; }
		[DataMember]
		public string Mrid { get; set; }
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
		public List<User> Users { get; set; }  // ubaceno samo zbog EF many to many, mada mislim da ovako jednostavno mora xD
		//[DataMember] // sa ovim puca
		[NotMapped]
		public string GetPermsInOneLine {
			get
			{
				string tempString = string.Empty;

				foreach (var item in Permissions)
				{
					tempString += item.Name + ",";
				}

				tempString = tempString.Remove(tempString.Length - 1);

				return tempString;
			}

			set
			{
				GetPermsInOneLine = value;
			}
		}

		public AORCachedArea()
		{
			this.Users = new List<User>();
			this.Mrid = Guid.NewGuid().ToString();
			this.Permissions = new List<Permission>();
		}
		public AORCachedArea(List<Permission> perms, List<User> users)
		{
			this.Permissions = perms;
			this.Users = users;
			this.Mrid = Guid.NewGuid().ToString();
		}
		public AORCachedArea(string name, string description, List<Permission> perms, List<AORCachedGroup> groups) : base(description, false)
		{
			this.Name = name;
			this.Permissions = perms;
			this.Groups = groups;
			this.Mrid = Guid.NewGuid().ToString();
		}
	}
}
