using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
	[DataContract]
	public class RotatingMachine : RegulatingCondEq
	{

		private FuelType fuelType;
		private float ratedS;
		private float nominalP;
		private float nominalQ;
		private int derFlexibilityP;
		private int derFlexibilityQ;
		public RotatingMachine(long globalId)
			: base(globalId)
		{
		}

		[DataMember]
		public FuelType FuelType
		{
			get
			{
				return fuelType;
			}

			set
			{
				fuelType = value;
			}
		}

		[DataMember]
		public float RatedS
		{
			get
			{
				return ratedS;
			}

			set
			{
				ratedS = value;
			}
		}

		[DataMember]
		public float NominalP
		{
			get
			{
				return nominalP;
			}

			set
			{
				nominalP = value;
			}
		}

		[DataMember]
		public float NominalQ
		{
			get
			{
				return nominalQ;
			}

			set
			{
				nominalQ = value;
			}
		}

		[DataMember]
		public int DERFlexibilityP
		{
			get
			{
				return derFlexibilityP;
			}

			set
			{
				derFlexibilityP	 = value;
			}
		}

		[DataMember]
		public int DERFlexibilityQ
		{
			get
			{
				return derFlexibilityQ;
			}

			set
			{
				derFlexibilityQ = value;
			}
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				RotatingMachine x = (RotatingMachine)obj;
				return (x.fuelType == this.fuelType && 
					x.ratedS == this.ratedS && 
						x.ratedS == this.ratedS &&
						x.nominalP == this.nominalP &&
						x.derFlexibilityP == this.derFlexibilityP &&
						x.derFlexibilityQ == this.derFlexibilityQ &&
						x.nominalQ == this.nominalQ);
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
				copy = new RotatingMachine(this.GlobalId);
			}

			((RotatingMachine)copy).fuelType = this.fuelType;
			((RotatingMachine)copy).ratedS = this.ratedS;
			((RotatingMachine)copy).nominalP = this.nominalP;
			((RotatingMachine)copy).nominalQ = this.nominalQ;
			((RotatingMachine)copy).derFlexibilityP = this.derFlexibilityP;
			((RotatingMachine)copy).derFlexibilityQ = this.derFlexibilityQ;
			return base.DeepCopy(copy);
		}

		#region IAccess implementation
		public override bool HasProperty(ModelCode t)
		{
			switch (t)
			{
				case ModelCode.ROTATINGMACHINE_FUELTYPE:
				case ModelCode.ROTATINGMACHINE_RATEDS:
				case ModelCode.ROTATINGMACHINE_NOMINALP:
				case ModelCode.ROTATINGMACHINE_NOMINALQ:
				case ModelCode.ROTATINGMACHINE_DERFLEXIBILITYP:
				case ModelCode.ROTATINGMACHINE_DERFLEXIBILITYQ:
					return true;

				default:
					return base.HasProperty(t);

			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.ROTATINGMACHINE_FUELTYPE:
					property.SetValue((short)fuelType);
					break;

				case ModelCode.ROTATINGMACHINE_RATEDS:
					property.SetValue(ratedS);
					break;

				case ModelCode.ROTATINGMACHINE_NOMINALP:
					property.SetValue(nominalP);
					break;

				case ModelCode.ROTATINGMACHINE_NOMINALQ:
					property.SetValue(nominalQ);
					break;

				case ModelCode.ROTATINGMACHINE_DERFLEXIBILITYP:
					property.SetValue(derFlexibilityP);
					break;

				case ModelCode.ROTATINGMACHINE_DERFLEXIBILITYQ:
					property.SetValue(derFlexibilityQ);
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
				case ModelCode.ROTATINGMACHINE_FUELTYPE:
					fuelType = (FuelType)property.AsEnum();
					break;

				case ModelCode.ROTATINGMACHINE_RATEDS:
					ratedS = property.AsFloat();
					break;

				case ModelCode.ROTATINGMACHINE_NOMINALP:
					nominalP = property.AsFloat();
					break;

				case ModelCode.ROTATINGMACHINE_NOMINALQ:
					nominalQ = property.AsFloat();
					break;

				case ModelCode.ROTATINGMACHINE_DERFLEXIBILITYP:
					derFlexibilityP = property.AsInt();
					break;

				case ModelCode.ROTATINGMACHINE_DERFLEXIBILITYQ:
					derFlexibilityQ = property.AsInt();
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
	}
}
