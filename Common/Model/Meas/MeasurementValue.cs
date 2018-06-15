using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{

	[Serializable]
	[DataContract]
	[KnownType(typeof(PowerType))]
	[KnownType(typeof(TypeOfReference))]
	public class MeasurementValue : IdentifiedObject
	{
		private long timestamp; // radi se sa ticks
		private long synchronousMachine = 0;
		private PowerType powerType;
		private int address;

		public MeasurementValue(long globalId)
			: base(globalId)
		{
		}

        [DataMember]
		public long Timestamp
		{
			get
			{
				return timestamp;
			}

			set
			{
				timestamp = value;
			}
		}

        [DataMember]
        public long SynchronousMachine
		{
			get
			{
				return synchronousMachine;
			}

			set
			{
				synchronousMachine = value;
			}
		}

		[DataMember]
		public  PowerType PowerType
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
		public int Address
		{
			get
			{
				return address;
			}

			set
			{
				address = value;
			}
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				MeasurementValue x = (MeasurementValue)obj;
				return (x.timestamp == this.timestamp &&
						x.synchronousMachine == this.synchronousMachine &&
						x.address == this.address && 
						x.powerType == this.powerType);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override IdentifiedObject DeepCopy(IdentifiedObject copy = null)
		{
			if (copy == null)
			{
				copy = new MeasurementValue(this.GlobalId);
			}

			((MeasurementValue)copy).timestamp = this.timestamp;
			((MeasurementValue)copy).synchronousMachine = this.synchronousMachine;
			((MeasurementValue)copy).powerType = this.powerType;
			((MeasurementValue)copy).address = this.address;
			return base.DeepCopy(copy);
		}

		#region IAccess implementation

		public override bool HasProperty(ModelCode t)
		{
			switch (t)
			{
				case ModelCode.MEASUREMENTVALUE_SYNCMACHINE:
				case ModelCode.MEASUREMENTVALUE_TIMESTAMP:
				case ModelCode.MEASUREMENTVALUE_POWERTYPE:
				case ModelCode.MEASUREMENTVALUE_ADDRESS:

					return true;
				default:
					return base.HasProperty(t);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.MEASUREMENTVALUE_SYNCMACHINE:
					property.SetValue(synchronousMachine);
					break;

				case ModelCode.MEASUREMENTVALUE_TIMESTAMP:
					property.SetValue(timestamp);
					break;

				case ModelCode.MEASUREMENTVALUE_POWERTYPE:
					property.SetValue((short)powerType);
					break;

				case ModelCode.MEASUREMENTVALUE_ADDRESS:
					property.SetValue(address);
					break;

				default:
					base.GetProperty(property);
					break;
			}
		}

		public override void SetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.MEASUREMENTVALUE_SYNCMACHINE:
					synchronousMachine = property.AsReference();
					break;

				case ModelCode.MEASUREMENTVALUE_TIMESTAMP:    
					timestamp = property.AsLong(); //  ako ce se citati vrijeme iz longa (.Ticks polje)
					break;

				case ModelCode.MEASUREMENTVALUE_POWERTYPE:
					powerType = (PowerType)property.AsEnum();
					break;

				case ModelCode.MEASUREMENTVALUE_ADDRESS:
					address = property.AsInt();
					break;

				default:
					base.SetProperty(property);
					break;
			}
		}

		#endregion IAccess implementation

		#region IReference implementation	
		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (synchronousMachine != 0 && (refType != TypeOfReference.Reference || refType != TypeOfReference.Both))
			{
				references[ModelCode.MEASUREMENTVALUE_SYNCMACHINE] = new List<long>();
				references[ModelCode.MEASUREMENTVALUE_SYNCMACHINE].Add(synchronousMachine);
			}

			base.GetReferences(references, refType);
		}
		#endregion IReference implementation

	}
}
