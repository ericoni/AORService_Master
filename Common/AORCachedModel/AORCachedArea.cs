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
	[KnownType(typeof(AORCachedUser))]
	[KnownType(typeof(AORCachedArea))]
	[KnownType(typeof(AORCachedGroup))]
	[KnownType(typeof(Permission))]
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
		public List<AORCachedArea> Areas { get; set; }
		[DataMember]
		public List<AORCachedGroup> Groups { get; set; }
		[DataMember]
		public List<Permission> Permissions { get; set; }
		[DataMember]
		public List<AORCachedUser> Users { get; set; }//to do da li ovo izbaciti, mozda nije potrebno. Nego raditi samo sa UserAreaNew combined tabelom... 1.8.
		//[DataMember] // izgleda da nece ovo kad se stavi,svakako ne vidim ulogu koju ima kad moze sve ok da radi.
        //public virtual ICollection<AORCachedUserArea> UserAreas { get; set; }
        [DataMember]
        public int NumberOfUsersControling { get; set; }
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
			this.Areas = new List<AORCachedArea>();
			this.Groups = new List<AORCachedGroup>();
			this.Permissions = new List<Permission>();
			this.Users = new List<AORCachedUser>();
			this.NumberOfUsersControling = 0;
		}

		public AORCachedArea(string name, string description, List<Permission> perms, List<AORCachedGroup> groups) : base(description, false)
		{
			this.Name = name;
			this.Permissions = perms;
			this.Groups = groups;
			this.Mrid = Guid.NewGuid().ToString();
			this.NumberOfUsersControling = 0;
		}

		public AORCachedArea(string name, string description, List<Permission> perms, List<AORCachedGroup> groups, List<AORCachedArea> areas) : base(description, false)
		{
			this.Name = name;
			this.Permissions = perms;
			this.Groups = groups;
			this.NumberOfUsersControling = 0;
			this.Mrid = Guid.NewGuid().ToString();
		}
	}
}
