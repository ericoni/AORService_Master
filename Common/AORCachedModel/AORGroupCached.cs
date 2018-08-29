using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.AORCachedModel
{
	[Serializable]
	public class AORGroupCached : AORCachedEntity
	{
		public List<SynchronousMachine> sMachines { get; set; }

		public AORGroupCached()
		{
			sMachines = new List<SynchronousMachine>();
		}

		public AORGroupCached(string name, List<SynchronousMachine> sms) : base(name)
		{
			this.sMachines = sms;
		}
	}
}
