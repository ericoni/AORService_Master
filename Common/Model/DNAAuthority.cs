using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.Model
{
	public class DNAAuthority
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public List<Permission> PermissionList { get; set; }         // EF does not support collections of primitive types
		//public Permission PermissionList { get; set; }   
		//public DNAAuthority(Permission permissionList)
		//{
		//	this.PermissionList = permissionList;
		//}
		public DNAAuthority() { }

		public DNAAuthority(List<Permission> permissionList)
		{
			this.PermissionList = permissionList;
		}
	}
}
