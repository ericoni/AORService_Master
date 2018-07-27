using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.Model
{
	[DataContract]
	public class AORGroup
	{
		private int id;
		private string name = "";
		private long gid = 0;

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[DataMember]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets or sets the gid.
		/// </summary>
		/// <value>
		/// The gid.
		/// </value>
		[DataMember]
		public long Gid
		{
			get { return gid; }
			set { gid = value; }
		}
	}
}
