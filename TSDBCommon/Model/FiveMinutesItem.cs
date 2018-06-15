using FTN.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSDB.Model
{
	public class FiveMinutesItem : 
		
		HistorianItem
	{
		public FiveMinutesItem() { }

        public FiveMinutesItem(float averageValue, long timestamp, long globalId, PowerType type, float increase, float decrease)
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
