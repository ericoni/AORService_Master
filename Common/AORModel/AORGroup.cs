using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.AORModel
{
	[Serializable]
	[DataContract]
	public class AORGroup : IdentifiedObject
	{
		private int id;
		private long gid = 0;
		private List<long> substations = new List<long>();
		private List<long> synchronousMachines = new List<long>();

		public AORGroup(long globalId) : base(globalId)
		{
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}
		[DataMember]
		public long AORAGAggregator { get; set; }
		[DataMember]
		public bool IsCovered { get; set; }
		[DataMember]
		public List<long> Substations
		{
			get { return substations; }
			set { substations = value; }
		}
		[DataMember]
		public List<long> SynchronousMachines
		{
			get { return synchronousMachines; }
			set { synchronousMachines = value; }
		}
		[DataMember]
		public long Gid
		{
			get { return gid; }
			set { gid = value; }
		}

		#region IAccess implementation

		public override bool HasProperty(ModelCode property)
		{
			switch (property)
			{
				case ModelCode.AOR_GROUP_AGGREGATOR:
				case ModelCode.AOR_GROUP_COVERED:
				case ModelCode.AOR_GROUP_SUBSTATIONS:
				case ModelCode.AOR_GROUP_SYNCMACHINES:
					return true;
				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.AOR_GROUP_AGGREGATOR:
					property.SetValue(AORAGAggregator);
					break;

				case ModelCode.AOR_GROUP_COVERED:
					property.SetValue(IsCovered);
					break;

				case ModelCode.AOR_GROUP_SUBSTATIONS:
					property.SetValue(Substations);
					break;

				case ModelCode.AOR_GROUP_SYNCMACHINES:
					property.SetValue(SynchronousMachines);
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
				case ModelCode.AOR_GROUP_AGGREGATOR:
					AORAGAggregator = property.AsReference();
					break;
				case ModelCode.AOR_GROUP_COVERED:
					IsCovered = property.AsBool();
					break;

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
				return SynchronousMachines.Count > 0 || Substations.Count > 0 || base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (Substations != null && Substations.Count > 0 &&
				(refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.AOR_GROUP_SUBSTATIONS] = Substations.GetRange(0, Substations.Count);
			}
			if (SynchronousMachines != null && SynchronousMachines.Count > 0 &&
				(refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.AOR_GROUP_SYNCMACHINES] = Substations.GetRange(0, SynchronousMachines.Count);
			}

			if (AORAGAggregator != 0 && (refType != TypeOfReference.Reference || refType != TypeOfReference.Both))
			{
				references[ModelCode.AOR_GROUP_AGGREGATOR] = new List<long>();
				references[ModelCode.AOR_GROUP_AGGREGATOR].Add(AORAGAggregator);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId) // samo ovde ide kontra 
		{
			switch (referenceId)
			{
				case ModelCode.SUBSTATION_AORGROUP:
					Substations.Add(globalId);
					break;
				case ModelCode.SYNCMACHINE_AORGROUP:
					SynchronousMachines.Add(globalId);
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
				case ModelCode.SUBSTATION_AORGROUP:

					if (Substations.Contains(globalId))
					{
						Substations.Remove(globalId);

					}
					else
					{
						CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
					}
					break;

				case ModelCode.SYNCMACHINE_AORGROUP:

					if (SynchronousMachines.Contains(globalId))
					{
						SynchronousMachines.Remove(globalId);

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

		public AORGroup ConvertFromRD(ResourceDescription rd)
		{
			if (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)) != DMSType.AOR_GROUP)
			{
				return this;
			}

			if (rd.Properties == null)
			{
				return this;
			}

			foreach (Property property in rd.Properties)
			{
				if (property.Id == ModelCode.IDOBJ_GID)
				{
					this.GlobalId = property.AsLong();
				}
				else
				{
					switch (property.Id)
					{
						case ModelCode.AOR_GROUP_SUBSTATIONS:
						case ModelCode.AOR_GROUP_SYNCMACHINES:
							continue;
					}
					this.SetProperty(property);
				}
			}
			return this;
		}
	}
}
