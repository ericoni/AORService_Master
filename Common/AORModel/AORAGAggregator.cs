using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.AORModel
{
	[Serializable]
	[DataContract]
	public class AORAGAggregator : IdentifiedObject
	{
		public List<long> AORGroups { get; set; }
		public List<long> AORAreas { get; set; }

		public AORAGAggregator(long globalId) : base(globalId)
		{
			AORGroups = new List<long>();
			AORAreas = new List<long>();
		}

		#region IAccess implementation
		public override bool HasProperty(ModelCode code) 
		{
			switch (code)
			{
				case ModelCode.AOR_AGAGGREGATOR_AORAREAS:
				case ModelCode.AOR_AGAGGREGATOR_AORGROUPS:
					return true;
				default:
					return base.HasProperty(code);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.AOR_AGAGGREGATOR_AORAREAS:
					prop.SetValue(AORAreas);
					break;
				case ModelCode.AOR_AGAGGREGATOR_AORGROUPS:
					prop.SetValue(AORGroups);
					break;
				default:
					base.GetProperty(prop);
					break;
			}
		}

		public override void SetProperty(Property property)
		{
			switch (property.Id)
			{
				default:
					base.SetProperty(property);
					break;
			}
		}

		#endregion IAccess implementation

		#region IReference implementation

		public override bool IsReferenced
		{
			get
			{
				return AORGroups.Count > 0 || AORAreas.Count > 0 || base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (AORAreas != null && AORAreas.Count > 0 &&
				(refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.AOR_AGAGGREGATOR_AORAREAS] = AORAreas.GetRange(0, AORAreas.Count);
			}
			if (AORGroups != null && AORGroups.Count > 0 &&
				(refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.AOR_AGAGGREGATOR_AORGROUPS] = AORGroups.GetRange(0, AORGroups.Count);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				case ModelCode.AOR_AREA_AGGREGATOR:
					AORAreas.Add(globalId);
					break;

				case ModelCode.AOR_GROUP_AGGREGATOR:
					AORGroups.Add(globalId);
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
				case ModelCode.AOR_AREA_AGGREGATOR:

					if (AORAreas.Contains(globalId))
					{
						AORAreas.Remove(globalId);
					}
					else
					{
						CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
					}

					break;

				case ModelCode.AOR_GROUP_AGGREGATOR:

					if (AORGroups.Contains(globalId))
					{
						AORGroups.Remove(globalId);
					}
					else
					{
						CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
					}

					break;

				default:
					base.RemoveReference(referenceId, globalId);
					break;
			}
		}

		#endregion IReference implementation

		public AORAGAggregator ConvertFromRD(ResourceDescription rd)
		{
			if (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)) == DMSType.AOR_AGAGGREGATOR)
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
							switch (property.Id)
							{
								case ModelCode.AOR_AGAGGREGATOR_AORAREAS:
								case ModelCode.AOR_AGAGGREGATOR_AORGROUPS:
									continue;
							}
							this.SetProperty(property);
						}
					}
				}
			}
			return this;
		}
	}
}
