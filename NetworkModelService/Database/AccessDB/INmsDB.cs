using FTN.Common;
using FTN.Services.NetworkModelService.Database.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.Database.Access
{
	public interface INmsDB
	{
		/// <summary>
		/// Add new delta entry
		/// </summary>
		/// <param name="delta"> Delta to be added</param>
		/// <returns></returns>
		bool AddDelta(Delta delta);

		List<Delta> ReadDeltas();

	}
}
