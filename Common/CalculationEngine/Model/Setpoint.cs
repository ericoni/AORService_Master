using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.CalculationEngine.Model
{
	[Serializable]
	[DataContract]
	public class Setpoint
	{
		private Dictionary<long, float> pDistributionByAV;

		public Setpoint()
		{
			pDistributionByAV = new Dictionary<long, float>();
		}

		[DataMember]
		
		public Dictionary<long, float> PDistributionByAV
		{
			get
			{
				return pDistributionByAV;
			}

			set
			{
				pDistributionByAV = value;
			}
		}
	}
}
