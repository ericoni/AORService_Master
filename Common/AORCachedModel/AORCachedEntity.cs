using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.AORCachedModel
{
	public class AORCachedEntity
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public AORCachedEntity() { }
		public AORCachedEntity(string name)
		{
			this.Name = name;
		}
	}
}
