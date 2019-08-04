﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FTN.Common.AORCachedModel
{
	[Serializable]
    [DataContract]
	public class Permission
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[XmlIgnore]
		public int PermissionId { get; set; }
		[XmlAttribute]
        [DataMember]
		public string Name { get; set; } //DNAAuthority ima List<Permission> sto tabeli Permissions daje vezu ka DNAAuthority
		[NotMapped]
        [DataMember]
		[XmlAttribute]
        public string Description { get; set; }
		[XmlIgnore]
        [DataMember]
		public List<DNAAuthority> DNAs { get; set; } // ubaceno samo zbog EF many to many
		[XmlIgnore]
		public List<AORCachedModel.AORCachedArea> Areas { get; set; } // ubaceno samo zbog EF many to many

		public Permission()
		{
			this.Name = string.Empty;
			this.Description = string.Empty;
		}

		public Permission(string name)
		{
			this.Name = name;
		}

		public Permission(string name, string desc)
		{
			this.Name = name;
			this.Description = desc;
		}
	}
}
