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
	/// Currently there are 2 ways of saving SyncMachines and AOR group relationships. Maybe get rid of one later.
	/// </summary>
	public class AORCachedSyncMachineGroupNew
	{
		[Key, Column(Order = 0)]
		public int GroupID { get; set; }
		[Key, Column(Order = 1)]
		public int SyncMachineID { get; set; }
		//navigation purposes
		public virtual AORCachedGroup Group { get; set; }
		public virtual AORCachedSyncMachine SyncMachine { get; set; }

		public long SmGidFromNMS { get; set; }
		public string AORGroupName { get; set; }
	}
}
