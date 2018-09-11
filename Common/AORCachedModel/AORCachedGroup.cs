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
	[Serializable]
	[DataContract]
	public class AORCachedGroup : AORCachedEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[DataMember]
		public List<SynchronousMachine> SMachines { get; set; }
		[DataMember]
		public List<AORCachedArea> Areas { get; set; }
		public AORCachedGroup()
		{
			SMachines = new List<SynchronousMachine>();
			Areas = new List<AORCachedArea>();
		}
		public AORCachedGroup(string name, List<SynchronousMachine> sms) : base(name) // vrati se da obrises ovaj
		{
			this.SMachines = sms;
		}

		public AORCachedGroup(string name, List<SynchronousMachine> sms, List<AORCachedArea> areas) : base(name)
		{
			this.SMachines = sms;
			this.Areas = areas;
		}
	}
}
