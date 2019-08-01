using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.AORCachedModel
{
	/// <summary>
	/// Mozda ce mi biti potreban DataMember tag.
	/// </summary>
	public class AORCachedUserArea
	{
		[Key, Column(Order = 0)]
		public int UserId { get; set; }
		[Key, Column(Order = 1)]
		public int AreaId{ get; set; }
		// Add the navigation properties
		public virtual AORCachedUser User { get; set; }
		public virtual AORCachedArea Area { get; set; }
		public bool IsSelectedForView { get; set; }
		public bool IsSelectedForControl { get; set; }
	}
}
