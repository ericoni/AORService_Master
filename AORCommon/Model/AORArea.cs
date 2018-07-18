using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.Model
{
	[DataContract]
	public class AORArea
	{
		private string name = string.Empty;
		private bool controlAssignable = false;                                
		private bool viewAssignable = false;                                   
		private List<AORGroup> groups = new List<AORGroup>();                   
		private List<string> areas = new List<string>();

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
		/// Gets or sets a value indicating whether [control assignable].
		/// </summary>
		/// <value>
		///   <c>true</c> if [control assignable]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool ControlAssignable
		{
			get { return controlAssignable; }
			set { controlAssignable = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [view assignable].
		/// </summary>
		/// <value>
		///   <c>true</c> if [view assignable]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool ViewAssignable
		{
			get { return viewAssignable; }
			set { viewAssignable = value; }
		}


		/// <summary>
		/// Gets or sets the groups.
		/// </summary>
		/// <value>
		/// The groups.
		/// </value>
		[DataMember]
		public List<AORGroup> Groups
		{
			get { return groups; }
			set { groups = value; }
		}

		/// <summary>
		/// Gets or sets the areas.
		/// </summary>
		/// <value>
		/// Areas.
		/// </value>
		[DataMember]
		public List<string> Areas
		{
			get { return areas; }
			set { areas = value; }
		}
	}
}
