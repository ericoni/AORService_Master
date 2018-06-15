using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.CE.Model
{
	public class CAS
	{
		private long gid;

		private CASEnum controlledBy;

		public long Gid
		{
			get
			{
				return gid;
			}

			set
			{
				gid = value;
			}
		}

		public CASEnum ControlledBy
		{
			get
			{
				return controlledBy;
			}

			set
			{
				controlledBy = value;
			}
		}

		public CAS(long gid, CASEnum control)
		{
			this.Gid = gid;
			this.ControlledBy = control;
		}
	}
}
