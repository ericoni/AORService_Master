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
	/// Already contains "Name" in parent class. This entity will be put in the DB (not NMS entity).
	/// </summary>
	[Serializable]
	[DataContract]
    [KnownType(typeof(AORCachedSyncMachine))]
    [KnownType(typeof(AORCachedArea))]
    public class AORCachedGroup : AORCachedEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int GroupId { get; set; }
		[DataMember]
		public List<AORCachedSyncMachine> SynchronousMachines { get; set; }
        //[DataMember] // ovde je svjesno izbacena areas, da se ne bi slalo kroz mrezu previse podataka
        public List<AORCachedArea> Areas { get; set; }
		[DataMember]
		public string Mrid { get; set; } // TODO: za sta ce mi ovaj
		[DataMember] 
		[NotMapped] //zasto notMapped?
		public long GidFromNms { get; set; }
		public AORCachedGroup()
		{
			SynchronousMachines = new List<AORCachedSyncMachine>();
			Areas = new List<AORCachedArea>();
		}
		public AORCachedGroup(string name, List<AORCachedSyncMachine> syncMachines, List<AORCachedArea> areas) : base(name)
		{
			this.SynchronousMachines = syncMachines;
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
