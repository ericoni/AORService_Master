using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.Model
{
	[DataContract]
	public class AORGroup
	{
		private string name = "";
		private long gid = 0;

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
