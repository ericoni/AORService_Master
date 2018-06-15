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
	public class DiscreteValue : MeasurementValue
	{
		private float value;

		public DiscreteValue(long globalId)
			: base(globalId)
		{
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

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				DiscreteValue x = (DiscreteValue)obj;
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
				copy = new DiscreteValue(this.GlobalId);
			}

			((DiscreteValue)copy).value = this.value;
			return base.DeepCopy(copy);
		}

		#region IAccess implementation
		public override bool HasProperty(ModelCode t)
		{
			switch (t)
			{
				case ModelCode.DISCRETEVALUE_VALUE:

					return true;
				default:
					return base.HasProperty(t);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.DISCRETEVALUE_VALUE:
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
				case ModelCode.DISCRETEVALUE_VALUE:
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

        public DiscreteValue ConvertFromRD(ResourceDescription rd)
        {
            if (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)) == DMSType.DISCRETEVALUE)
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
