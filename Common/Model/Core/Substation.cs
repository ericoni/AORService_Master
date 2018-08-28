using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
	public class Substation : EquipmentContainer
	{
		public Substation(long globalId)
			: base(globalId) { }

		private long subRegion;
		//private long aorGroup;
		private float latitude;
		private float longitude;

		public long SubRegion
		{
			get { return this.subRegion; }
			set { this.subRegion = value; }
		}

		//public long AORGroup
		//{
		//	get { return this.aorGroup; }
		//	set { this.aorGroup = value; }
		//}

		public float Latitude
		{
			get { return this.latitude; }
			set { this.latitude = value; }
		}

		public float Longitude
		{
			get { return this.longitude; }
			set { this.longitude = value; }
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				Substation x = (Substation)obj;
				return ((x.SubRegion == this.SubRegion) && 
					x.Longitude == this.Longitude &&
				//	x.aorGroup == this.aorGroup && 
					x.Latitude == this.Latitude);
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
				copy = new Substation(this.GlobalId);
			}
			((Substation)copy).subRegion = this.subRegion;
			((Substation)copy).latitude = this.latitude;
			((Substation)copy).longitude = this.longitude;
			//((Substation)copy).aorGroup = this.aorGroup;
			return base.DeepCopy(copy);
		}

		#region IAccess implementation

		public override bool HasProperty(ModelCode property)
		{
			switch (property)
			{
				case ModelCode.SUBSTATION_SUBREGION:		
				case ModelCode.SUBSTATION_LATITUDE:
				case ModelCode.SUBSTATION_LONGITUDE:
				//case ModelCode.SUBSTATION_AORGROUP:
					return true;
				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.SUBSTATION_SUBREGION:
					property.SetValue(SubRegion);
					break;

				case ModelCode.SUBSTATION_LATITUDE:
					property.SetValue(Latitude);
					break;

				case ModelCode.SUBSTATION_LONGITUDE:
					property.SetValue(Longitude);
					break;

				//case ModelCode.SUBSTATION_AORGROUP:
				//	property.SetValue(AORGroup);
				//	break;

				default:
					base.GetProperty(property);
					break;
			}
		}

		public override void SetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.SUBSTATION_SUBREGION:
					SubRegion = property.AsReference();
					break;
				case ModelCode.SUBSTATION_LATITUDE:
					Latitude = property.AsFloat();
					break;
				case ModelCode.SUBSTATION_LONGITUDE:
					Longitude = property.AsFloat();
					break;
				//case ModelCode.SUBSTATION_AORGROUP:
				//	AORGroup = property.AsReference();
				//	break;

				default:
					base.SetProperty(property);
					break;
			}
		}

		#endregion IAccess implementation

		public override bool IsReferenced
		{
			get
			{
				return base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (SubRegion != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
			{
				references[ModelCode.SUBSTATION_SUBREGION] = new List<long>();
				references[ModelCode.SUBSTATION_SUBREGION].Add(SubRegion);
			}

			//if (AORGroup != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
			//{
			//	references[ModelCode.SUBSTATION_AORGROUP] = new List<long>();
			//	references[ModelCode.SUBSTATION_AORGROUP].Add(AORGroup);
			//}

			base.GetReferences(references, refType);
		}

		public Substation ConvertFromRD(ResourceDescription rd)
		{
			if(((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id))==DMSType.SUBSTATION)
			{
				if(rd.Properties!=null)
				{
					foreach(Property property in rd.Properties)
					{
						if(property.Id == ModelCode.IDOBJ_GID)
						{
							this.GlobalId = property.AsLong();
							continue;
						}
						else
						{
							switch (property.Id)
							{
								case ModelCode.EQCONTAINER_EQUIPMENTS:
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
