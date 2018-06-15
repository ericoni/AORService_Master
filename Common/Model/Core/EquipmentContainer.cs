using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
	public class EquipmentContainer : ConnectivityNodeContainer
	{
		public EquipmentContainer(long globalId)
			: base(globalId) { }

		private List<long> equipments = new List<long>();

		public List<long> Equipments
		{
			get { return equipments; }
			set { equipments = value; }
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				EquipmentContainer x = (EquipmentContainer)obj;
				return ((CompareHelper.CompareLists(x.Equipments, this.Equipments)));
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

		public override IdentifiedObject DeepCopy( IdentifiedObject copy)
		{
			return base.DeepCopy(copy);
		}

		#region IAccess implementation
		public override bool HasProperty(ModelCode code)
		{
			switch (code)
			{
				case ModelCode.EQCONTAINER_EQUIPMENTS:
					return true;

				default:
					return base.HasProperty(code);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.EQCONTAINER_EQUIPMENTS:
					prop.SetValue(equipments);
					break;
                default:
                    base.GetProperty(prop);
                    break;
            }
		}

		public override void SetProperty(Property property)
		{
			base.SetProperty(property);
		}

		#endregion IAccess implementation	

		#region IReference implementation

		public override bool IsReferenced
		{
			get
			{
				return equipments.Count > 0 || base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (equipments != null && equipments.Count > 0 &&
				(refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.EQCONTAINER_EQUIPMENTS] = equipments.GetRange(0, equipments.Count);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				case ModelCode.EQUIPMENT_EQCONTAINER:
					equipments.Add(globalId);
					break;

				default:
					base.AddReference(referenceId, globalId);
					break;
			}
		}

		public override void RemoveReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				case ModelCode.EQUIPMENT_EQCONTAINER:

					if (equipments.Contains(globalId))
					{
						equipments.Remove(globalId);
					}
					else
					{
						//CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
					}

					break;
				
				default:
					base.RemoveReference(referenceId, globalId);
					break;
			}
		}

		#endregion IReference implementation	

	}
}
