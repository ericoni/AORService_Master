using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.Common;

namespace CommonCE
{
	[DataContract]
	public class ForecastObject
	{
		private long _derGID;

		private List<AnalogValue> _hourlyP;

		private List<AnalogValue> _hourlyQ;

		public ForecastObject()
		{
			this._derGID = 0;
			this._hourlyP = new List<AnalogValue>();
			this._hourlyQ = new List<AnalogValue>();
		}
		public ForecastObject(long derGID, List<AnalogValue> hourlyP, List<AnalogValue> hourlyQ)
		{
			this._derGID = derGID;
			this._hourlyP = hourlyP;
			this._hourlyQ = hourlyQ;
			
		}

		[DataMember]
		public long DerGID
		{
			get { return _derGID; }
			set { _derGID = value; }
		}

		[DataMember]
		public List<AnalogValue> HourlyP
		{
			get { return _hourlyP; }
			set { _hourlyP = value; }
		}
		

		[DataMember]
		public List<AnalogValue> HourlyQ
		{
			get { return _hourlyQ; }
			set { _hourlyQ = value; }
		}
	}

	//[Serializable]
	//[DataContract]
	//public class PowerHourObject
	//{
	//	private float powValue;
	//	private float powIncrease;
	//	private float powDecrease;
	//	private long time;

	//	public PowerHourObject(long time, float powValue, float powIncrease, float powDecrease)
	//	{
	//		this.Time = time;
	//		this.PowValue = powValue;
	//		this.PowIncrease = powIncrease;
	//		this.PowDecrease = powDecrease;
			
	//	}

	//	[DataMember]
	//	public float PowValue
	//	{
	//		get { return powValue; }
	//		set { powValue = value; }
	//	}

	//	[DataMember]
	//	public float PowIncrease
	//	{
	//		get { return powIncrease; }
	//		set { powIncrease = value; }
	//	}

	//	[DataMember]
	//	public float PowDecrease
	//	{
	//		get { return powDecrease; }
	//		set { powDecrease = value; }
	//	}

	//	[DataMember]
	//	public long Time
	//	{
	//		get { return time; }
	//		set { time = value; }
	//	}
	//}
}
