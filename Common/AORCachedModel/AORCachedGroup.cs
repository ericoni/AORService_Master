using FTN.Services.NetworkModelService.DataModel.Wires;
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
	/// <summary>
	/// Already contains "Name" in parrent class. This entity will be put in the DB. 
	/// </summary>
	[Serializable]
	[DataContract]
	public class AORCachedGroup : AORCachedEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int GroupId { get; set; }
		[DataMember]
		public List<SynchronousMachine> SynchronousMachines { get; set; }
		[DataMember]
		public List<AORCachedArea> Areas { get; set; }
		[DataMember]
		public string Mrid { get; set; } // TODO: za sta ce mi ovaj
		[DataMember] 
		[NotMapped]
		public long GidFromNms { get; set; }
		public AORCachedGroup()
		{
			SynchronousMachines = new List<SynchronousMachine>();
			Areas = new List<AORCachedArea>();
		}
		public AORCachedGroup(string name, List<SynchronousMachine> sms) : base(name) // TODO: vrati se da obrises ovaj
		{
			this.SynchronousMachines = sms;
			this.Mrid = Guid.NewGuid().ToString();
		}

		public AORCachedGroup(string name, List<SynchronousMachine> sms, List<AORCachedArea> areas) : base(name)
		{
			this.SynchronousMachines = sms;
			this.Areas = areas;
			this.Mrid = Guid.NewGuid().ToString();
		}

		public AORCachedGroup(string name, long gidFromNms) : base(name)
		{
			this.GidFromNms = gidFromNms;
			this.Mrid = Guid.NewGuid().ToString();
		}
	}
}
