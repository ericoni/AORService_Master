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
		public List<long> aorGroups { get; set; }
		public List<long> aorAreas { get; set; }

		public AORAGAggregator(long globalId) : base(globalId)
		{
		}

		#region IAccess implementation
		public override bool HasProperty(ModelCode code) // vrati se ovde
		{
			switch (code)
			{
				case ModelCode.aor_:
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
	}
}
