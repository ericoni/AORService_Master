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
	public class AORArea : IdentifiedObject
	{
		private int id;
		private long aorUser = 0;
		private long aorAggregator = 0;
		private string coveredBy = string.Empty;

		[DataMember]
		public bool IsControllable { get; set; }
		[DataMember]
		public bool IsViewable { get; set; }

		[DataMember]
		public string CoveredBy
		{
			get { return coveredBy; }
			set { coveredBy = value; }
		}

		public AORArea(long globalId) : base(globalId)
		{
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)] // vrati se (izbaciti ovo posle, jer se upisuje CachedAORArea
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [control assignable].
		/// </summary>
		/// <value>
		///   <c>true</c> if [control assignable]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public long AORUser
		{
			get { return aorUser; }
			set { aorUser = value; }
		}
		/// <summary>
		/// Gets or sets a value indicating whether [control assignable].
		/// </summary>
		/// <value>
		///   <c>true</c> if [control assignable]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public long AORAGAggregator
		{
			get { return aorAggregator; }
			set { aorAggregator = value; }
		}

		#region IAccess implementation

		public override bool HasProperty(ModelCode property)
		{
			switch (property)
			{
				case ModelCode.AOR_AREA_AGGREGATOR:
				case ModelCode.AOR_AREA_VIEWABLE:
				case ModelCode.AOR_AREA_COVEREDBY:
				case ModelCode.AOR_AREA_CONTROLLABLE:
				case ModelCode.AOR_AREA_USER:
					return true;
				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.AOR_AREA_AGGREGATOR:
					property.SetValue(AORAGAggregator);
					break;

				case ModelCode.AOR_AREA_VIEWABLE:
					property.SetValue(IsViewable);
					break;

				case ModelCode.AOR_AREA_CONTROLLABLE:
					property.SetValue(IsControllable);
					break;

				case ModelCode.AOR_AREA_USER:
					property.SetValue(AORUser);
					break;

				case ModelCode.AOR_AREA_COVEREDBY:
					property.SetValue(CoveredBy);
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
				case ModelCode.AOR_AREA_AGGREGATOR:
					AORAGAggregator = property.AsReference();
					break;
				case ModelCode.AOR_AREA_USER:
					AORUser = property.AsReference();
					break;
				case ModelCode.AOR_AREA_VIEWABLE:
					IsViewable = property.AsBool();
					break;
				case ModelCode.AOR_AREA_CONTROLLABLE:
					IsControllable = property.AsBool();
					break;
				case ModelCode.AOR_AREA_COVEREDBY:
					CoveredBy = property.AsString();
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
				return base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (AORAGAggregator != 0 && (refType != TypeOfReference.Reference || refType != TypeOfReference.Both))
			{
				references[ModelCode.AOR_AREA_AGGREGATOR] = new List<long>();
				references[ModelCode.AOR_AREA_AGGREGATOR].Add(AORAGAggregator);
			}

			if (AORUser != 0 && (refType != TypeOfReference.Reference || refType != TypeOfReference.Both))
			{
				references[ModelCode.AOR_AREA_USER] = new List<long>();
				references[ModelCode.AOR_AREA_USER].Add(AORUser);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				default:
					base.AddReference(referenceId, globalId);
					break;
			}
		}

		public override void RemoveReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				default:
					base.RemoveReference(referenceId, globalId);
					break;
			}
		}

		#endregion IReference implementation

		public AORArea ConvertFromRD(ResourceDescription rd)
		{
			if (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)) != DMSType.AOR_AREA)
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
					
					this.SetProperty(property);
				}
			}

			return this;
		}
	}
}
