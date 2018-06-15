using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
	public class GeographicalRegion : IdentifiedObject
	{
		public GeographicalRegion(long globalId)
			: base(globalId) { }

		private List<long> subRegions = new List<long>();

		public List<long> SubRegions
		{
			get { return subRegions; }
			set { subRegions = value; }
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				GeographicalRegion x = (GeographicalRegion)obj;
				return ((CompareHelper.CompareLists(x.SubRegions, this.SubRegions)));
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
				copy = new GeographicalRegion(this.GlobalId);
			}
			((GeographicalRegion)copy).subRegions.AddRange(subRegions);
			return base.DeepCopy(copy);
		}

		#region IAccess implementation
		public override bool HasProperty(ModelCode code)
		{
			switch (code)
			{
				case ModelCode.REGION_SUBREGIONS:
					return true;

				default:
					return base.HasProperty(code);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.REGION_SUBREGIONS:
					prop.SetValue(SubRegions);
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
				return SubRegions.Count > 0 || base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (SubRegions != null && SubRegions.Count > 0 &&
				(refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.REGION_SUBREGIONS] = SubRegions.GetRange(0, SubRegions.Count);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				case ModelCode.SUBREGION_REGION:
					SubRegions.Add(globalId);
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
				case ModelCode.SUBREGION_REGION:

					if (SubRegions.Contains(globalId))
					{
						SubRegions.Remove(globalId);
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

        public GeographicalRegion ConvertFromRD(ResourceDescription rd)
        {
            if (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)) == DMSType.REGION)
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
                            switch(property.Id)
                            {
                                case ModelCode.REGION_SUBREGIONS:
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
