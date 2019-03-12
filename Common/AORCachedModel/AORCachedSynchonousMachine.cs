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
	public class AORCachedSynchonousMachine : AORCachedEntity
	{
		private int id;
		private float maxQ;
		private float maxP;
		private AORCachedGroup aorGroup;

		public AORCachedSynchonousMachine(long gidFromNms, float maxQ, float maxP, AORCachedGroup aorGroup)
		{
			this.GidFromNms = gidFromNms;
			this.MaxQ = maxQ;
			this.MaxP = maxP;
			this.AORGroup = aorGroup;
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[DataMember]
		[NotMapped]
		public long GidFromNms { get; set; }

		[DataMember]
		public AORCachedGroup AORGroup
		{
			get
			{
				return aorGroup;
			}
			set
			{
				aorGroup = value;
			}
		}

		[DataMember]
		public float MaxQ
		{
			get
			{
				return maxQ;
			}

			set
			{
				maxQ = value;
			}
		}

		[DataMember]
		public float MaxP
		{
			get
			{
				return maxP;
			}

			set
			{
				maxP = value;
			}
		}
	}
}
