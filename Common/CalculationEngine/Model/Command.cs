using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.CalculationEngine.Model
{
    [DataContract]
	public class Command
	{
		private string mrid = "";
		private long globalId;
		private float demandedPower;
		private PowerType powerType;
		private long startTime;
		private long endTime;

		/// <summary>
		/// Trebalo bi da se unese broj sati 1-24
		/// </summary>
		private int duration;
		private OptimizationType optimizationType;

        [DataMember]
		public long GlobalId
		{
			get
			{
				return globalId;
			}

			set
			{
				globalId = value;
			}
		}

        [DataMember]
        public float DemandedPower
		{
			get
			{
				return demandedPower;
			}

			set
			{
				demandedPower = value;
			}
		}

        [DataMember]
        public PowerType PowerType
		{
			get
			{
				return powerType;
			}

			set
			{
				powerType = value;
			}
		}

        [DataMember]
        public long EndTime
		{
			get
			{
				return endTime;
			}

			set
			{
				endTime = value;
			}
		}

        [DataMember]
        public int Duration
		{
			get
			{
				return duration;
			}

			set
			{
				duration = value;
			}
		}

        [DataMember]
        public OptimizationType OptimizationType
		{
			get
			{
				return optimizationType;
			}

			set
			{
				optimizationType = value;
			}
		}

        [DataMember]
        public string Mrid
		{
			get
			{
				return mrid;
			}
			set
			{
				mrid = value;
			}
		}

        [DataMember]
        public long StartTime
		{
			get
			{
				return startTime;
			}

			set
			{
				startTime = value;
			}
		}

        public Command() { }

		public Command(long globalId, float demandedPower, PowerType powerType, int duration, OptimizationType optimizationType)
		{
			this.mrid = Guid.NewGuid().ToString();
			this.GlobalId = globalId;
			this.DemandedPower = demandedPower;
			this.PowerType = powerType;
			this.Duration = duration;
			this.OptimizationType = optimizationType;
		}
	}
}
