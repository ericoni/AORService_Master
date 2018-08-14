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
	public class AORUser : IdentifiedObject
	{
		public AORUser(long globalId) : base(globalId)
		{
			AORAreas = new List<long>();
		}

		[DataMember]
		public string Password { get; set; }
		[DataMember]
		public string Permissions { get; set; }
		[DataMember]
		public string ControlAreas { get; set; } // vrati se kasnije, pitanje je za sta ce se koristiti
		[DataMember]
		public string ViewAreas { get; set; }
		[DataMember]
		public List<long> AORAreas { get; set; }
		//[DataMember]
		//public DateTime? CheckinTime { get; set; }

		#region IAccess implementation
		public override bool HasProperty(ModelCode code)
		{
			switch (code)
			{
				case ModelCode.AOR_USER_AREAS:
				case ModelCode.AOR_USER_VIEWAREAS:
				case ModelCode.AOR_USER_CONTROLAREAS:
					return true;
				default:
					return base.HasProperty(code);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.AOR_USER_CONTROLAREAS:
					prop.SetValue(ControlAreas);
					break;
				case ModelCode.AOR_USER_VIEWAREAS:
					prop.SetValue(ViewAreas);
					break;
				case ModelCode.AOR_USER_AREAS:
					prop.SetValue(AORAreas);
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
				case ModelCode.AOR_USER_CONTROLAREAS:
					ControlAreas = property.AsString();
					break;
				case ModelCode.AOR_USER_VIEWAREAS:
					ViewAreas = property.AsString();
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
				return AORAreas.Count > 0 || base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (AORAreas != null && AORAreas.Count > 0 &&
				(refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.AOR_USER_AREAS] = AORAreas.GetRange(0, AORAreas.Count);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				case ModelCode.AOR_AREA_USER:
					AORAreas.Add(globalId);
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
				case ModelCode.SUBSTATION_SUBREGION:

					if (AORAreas.Contains(globalId))
					{
						AORAreas.Remove(globalId);
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

		public AORUser ConvertFromRD(ResourceDescription rd)
		{
			if (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)) == DMSType.AOR_USER)
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
								case ModelCode.AOR_USER_AREAS:
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
