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
	public class AnalogValue : MeasurementValue
	{
		private float value;
		private float powIncrease;
		private float powDecrease;

        public AnalogValue() : base(-1) { }

		public AnalogValue(long globalId)
			: base(globalId)
		{
		}

		public AnalogValue(float value, long timestamp, long globalId, PowerType type, float increase, float decrease) : base(globalId)
		{
			Value = value;
			Timestamp = timestamp;
			GlobalId = globalId;
			PowerType = type;
            powDecrease = decrease;
            powIncrease = increase;
        }

        public AnalogValue(float value, long timestamp, long globalId, long syncMachine, PowerType type, float powIncrease, float powDecrease) : base(globalId)
		{
			Value = value;
			Timestamp = timestamp;
			GlobalId = globalId;
			PowerType = type;
			PowIncrease = powIncrease;
			PowDecrease = powDecrease;
			SynchronousMachine = syncMachine;
		}

		[DataMember]
		public float Value
		{
			get
			{
				return value;
			}

			set
			{
				this.value = value;
			}
		}

		[DataMember]
		public float PowIncrease
		{
			get
			{
				return powIncrease;
			}

			set
			{
				powIncrease = value;
			}
		}
		[DataMember]
		public float PowDecrease
		{
			get
			{
				return powDecrease;
			}

			set
			{
				powDecrease = value;
			}
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				AnalogValue x = (AnalogValue)obj;
				return (x.value == this.value);
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
				copy = new AnalogValue(this.GlobalId);
			}

			((AnalogValue)copy).value = this.value;
			return base.DeepCopy(copy);
		}

		#region IAccess implementation
		public override bool HasProperty(ModelCode t)
		{
			switch (t)
			{
				case ModelCode.ANALOGVALUE_VALUE:

					return true;
				default:
					return base.HasProperty(t);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.ANALOGVALUE_VALUE:
					property.SetValue(value);
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
				case ModelCode.ANALOGVALUE_VALUE:
					value = property.AsFloat();
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
			base.GetReferences(references, refType);
		}

        #endregion IReference implementation

        public AnalogValue ConvertFromRD(ResourceDescription rd)
        {
            if (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)) == DMSType.ANALOGVALUE)
            {
                if (rd.Properties != null)
                {
                    foreach (Property property in rd.Properties)
                    {
                        if (property.Id == ModelCode.IDOBJ_GID)
                        {
                            this.GlobalId = property.AsLong();
                            continue;
                        }
                        else
                        {
                            this.SetProperty(property);
                        }
                    }
                }
            }
            return this;
        }
    }
}
