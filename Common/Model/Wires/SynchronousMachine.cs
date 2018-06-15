using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
	[DataContract]
	public class SynchronousMachine : RotatingMachine
	{
		private float baseQ;
		private float minQ;
		private float maxQ;
		private float minP;
		private float maxP;
		private List<long> measurements = new List<long>();

		public SynchronousMachine(long globalId)
			: base(globalId)
		{
		}

		[DataMember]
		public float BaseQ
		{
			get
			{
				return baseQ;
			}

			set
			{
				baseQ = value;
			}
		}

		[DataMember]
		public float MinQ
		{
			get
			{
				return minQ;
			}

			set
			{
				minQ = value;
			}
		}

		[DataMember]
		public float MaxQ
		{
			get
			{
				return maxQ;
			}

			set
			{
				maxQ = value;
			}
		}

		[DataMember]
		public float MinP
		{
			get
			{
				return minP;
			}

			set
			{
				minP = value;
			}
		}

		[DataMember]
		public float MaxP
		{
			get
			{
				return maxP;
			}

			set
			{
				maxP = value;
			}
		}

		[DataMember]
		public List<long> Measurements
		{
			get { return measurements; }
			set { measurements = value; }
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				SynchronousMachine x = (SynchronousMachine)obj;
				return (x.baseQ == this.baseQ && x.minQ == this.minQ && x.maxQ == this.maxQ  &&
						x.minP == this.minP &&
						x.maxP == this.maxP && 
						CompareHelper.CompareLists(x.measurements, this.measurements));
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
				copy = new SynchronousMachine(this.GlobalId);
			}

			((SynchronousMachine)copy).measurements.AddRange(measurements);
			((SynchronousMachine)copy).baseQ = this.baseQ;
			((SynchronousMachine)copy).minQ = this.minQ;
			((SynchronousMachine)copy).maxQ = this.maxQ;
			((SynchronousMachine)copy).minP = this.minP;
			((SynchronousMachine)copy).maxP = this.maxP;
			return base.DeepCopy(copy);
		}

		#region IAccess implementation
		public override bool HasProperty(ModelCode t)
		{
			switch (t)
			{
				case ModelCode.SYNCMACHINE_BASEQ:
				case ModelCode.SYNCMACHINE_MAXQ:
				case ModelCode.SYNCMACHINE_MINQ:
				case ModelCode.SYNCMACHINE_MEASVALUES:
				case ModelCode.SYNCMACHINE_MAXP:
				case ModelCode.SYNCMACHINE_MINP:
					return true;

				default:
					return base.HasProperty(t);

			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.SYNCMACHINE_BASEQ:
					property.SetValue(baseQ);
					break;

				case ModelCode.SYNCMACHINE_MAXQ:
					property.SetValue(maxQ);
					break;

				case ModelCode.SYNCMACHINE_MINQ:
					property.SetValue(minQ);
					break;

				case ModelCode.SYNCMACHINE_MEASVALUES:
					property.SetValue(measurements);
					break;

				case ModelCode.SYNCMACHINE_MAXP:
					property.SetValue(maxP);
					break;

				case ModelCode.SYNCMACHINE_MINP:
					property.SetValue(minP);
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
				case ModelCode.SYNCMACHINE_BASEQ:
					baseQ = property.AsFloat();
					break;

				case ModelCode.SYNCMACHINE_MINQ:
					minQ = property.AsFloat();
					break;

				case ModelCode.SYNCMACHINE_MAXQ:
					maxQ = property.AsFloat();
					break;

				case ModelCode.SYNCMACHINE_MINP:
					minP = property.AsFloat();
					break;

				case ModelCode.SYNCMACHINE_MAXP:
					maxP = property.AsFloat();
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
				return measurements.Count != 0 || base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			
			if (measurements != null && measurements.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.SYNCMACHINE_MEASVALUES] = measurements.GetRange(0, measurements.Count);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				case ModelCode.MEASUREMENTVALUE_SYNCMACHINE:
					measurements.Add(globalId);
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
				case ModelCode.MEASUREMENTVALUE_SYNCMACHINE:

					if (measurements.Contains(globalId))
					{
						measurements.Remove(globalId);
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

        public SynchronousMachine ConvertFromRD(ResourceDescription rd)
        {
            if (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)) == DMSType.SYNCMACHINE)
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
								case ModelCode.SYNCMACHINE_MEASVALUES:
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
