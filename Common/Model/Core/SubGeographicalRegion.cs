using FTN.Common;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
	public class SubGeographicalRegion : IdentifiedObject
	{
		public SubGeographicalRegion(long globalId)
			: base(globalId) { }

		private List<long> substations = new List<long>();
		private long region;

		public List<long> Substations
		{
			get { return substations; }
			set { substations = value; }
		}

		public long Region
		{
			get { return region; }
			set { region = value; }
		}
		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				SubGeographicalRegion x = (SubGeographicalRegion)obj;
				return ((CompareHelper.CompareLists(x.Substations, this.Substations)) && this.Region == x.Region);
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
				copy = new SubGeographicalRegion(this.GlobalId);
			}
			((SubGeographicalRegion)copy).substations.AddRange(substations);
			((SubGeographicalRegion)copy).region = this.region;
			return base.DeepCopy(copy);
		}

		#region IAccess implementation
		public override bool HasProperty(ModelCode code)
		{
			switch (code)
			{
				case ModelCode.SUBREGION_SUBSTATIONS:
					return true;
				case ModelCode.SUBREGION_REGION:
					return true;
				default:
					return base.HasProperty(code);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.SUBREGION_SUBSTATIONS:
					prop.SetValue(Substations);
					break;
				case ModelCode.SUBREGION_REGION:
					prop.SetValue(Region);
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
				case ModelCode.SUBREGION_REGION:
					Region = property.AsReference();
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
				return Substations.Count > 0 || base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (Substations != null && Substations.Count > 0 &&
				(refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.SUBREGION_SUBSTATIONS] = Substations.GetRange(0, Substations.Count);
			}


			if (Region != 0 && (refType != TypeOfReference.Reference || refType != TypeOfReference.Both))
			{
				references[ModelCode.SUBREGION_REGION] = new List<long>();
				references[ModelCode.SUBREGION_REGION].Add(Region);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				case ModelCode.SUBSTATION_SUBREGION:
					Substations.Add(globalId);
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

					if (Substations.Contains(globalId))
					{
						Substations.Remove(globalId);
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

        public SubGeographicalRegion ConvertFromRD(ResourceDescription rd)
        {
            if (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)) == DMSType.SUBREGION)
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
								case ModelCode.SUBREGION_SUBSTATIONS:
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
