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
    public class AORCachedSyncMachine : AORCachedEntity
	{
		private int id;
		private float maxQ;
		private float maxP;

		public AORCachedSyncMachine() { }

		public AORCachedSyncMachine(long gidFromNms, float maxQ, float maxP)
		{
			this.GidFromNms = gidFromNms;
			this.MaxQ = maxQ;
			this.MaxP = maxP;
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[DataMember]
		public long GidFromNms { get; set; }
	
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
