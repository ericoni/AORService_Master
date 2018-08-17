using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FTN.Common.Model
{
	public class DNAAuthority
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[XmlIgnore]
		public int Id { get; set; }
		[XmlAttribute]
		public List<Permission> PermissionList { get; set; }         // EF does not support collections of primitive types
		[XmlAttribute]
		public string Name { get; set; }
		[XmlAttribute]
		public string Description { get; set; }
		//public Permission PermissionList { get; set; }   
		//public DNAAuthority(Permission permissionList)
		//{
		//	this.PermissionList = permissionList;
		//}
		public DNAAuthority() { }

		public DNAAuthority(string name, List<Permission> permissionList)
		{
			this.Name = name;
			this.PermissionList = permissionList;
		}

		public DNAAuthority(string name, string desc, List<Permission> permissionList)
		{
			this.Name = name;
			this.Description = desc;
			this.PermissionList = permissionList;
		}
	}
}
