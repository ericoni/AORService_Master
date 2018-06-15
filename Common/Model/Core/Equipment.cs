using System.Collections.Generic;
using FTN.Common;
using System.Runtime.Serialization;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
	[DataContract]
	public class Equipment : PowerSystemResource
	{		
		private bool normallyInService;
		private long equipmentContainer;
						
		public Equipment(long globalId) : base(globalId) 
		{
		}
		[DataMember]
	
		public bool NormallyInService
		{
			get { return normallyInService; }
			set { normallyInService = value; }
		}

		[DataMember]
		public long EquipmentContainer
		{
			get { return equipmentContainer; }
			set { equipmentContainer = value; }
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				Equipment x = (Equipment)obj;
				return ((x.NormallyInService == this.NormallyInService) &&
						(x.EquipmentContainer == this.equipmentContainer));
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
				copy = new Equipment(this.GlobalId);
			}
			((Equipment)copy).NormallyInService = this.NormallyInService;
			((Equipment)copy).EquipmentContainer = this.EquipmentContainer;
			return base.DeepCopy(copy);
		}

		#region IAccess implementation

		public override bool HasProperty(ModelCode property)
		{
			switch (property)
			{
				case ModelCode.EQUIPMENT_INSERVICE:
				case ModelCode.EQUIPMENT_EQCONTAINER:
		
					return true;
				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.EQUIPMENT_INSERVICE:
					property.SetValue(NormallyInService);
					break;

				case ModelCode.EQUIPMENT_EQCONTAINER:
					property.SetValue(EquipmentContainer);
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
				case ModelCode.EQUIPMENT_INSERVICE:					
					NormallyInService = property.AsBool();
					break;

				case ModelCode.EQUIPMENT_EQCONTAINER:
					EquipmentContainer = property.AsReference();
					break;
			
				default:
					base.SetProperty(property);
					break;
			}
		}

		#endregion IAccess implementation


		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (EquipmentContainer != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
			{
				references[ModelCode.EQUIPMENT_EQCONTAINER] = new List<long>();
				references[ModelCode.EQUIPMENT_EQCONTAINER].Add(EquipmentContainer);
			}

			base.GetReferences(references, refType);
		}
	}
}
