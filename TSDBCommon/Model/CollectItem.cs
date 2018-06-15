using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSDB.Model
{
	public class CollectItem : HistorianItem
	{
		public CollectItem() { }

		public CollectItem(float averageValue, long timestamp, long globalId, PowerType type, float increase, float decrease)
		{
			Value = averageValue;
			Timestamp = timestamp;
			GlobalId = globalId;
			Type = type;
            Decrease = decrease;
            Increase = increase;
		}
	}
}
