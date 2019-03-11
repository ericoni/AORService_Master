using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.AORModel;
using FTN.Services.NetworkModelService.DataModel.Wires;

namespace FTN.Common.AORContract
{
	[ServiceContract]
	public interface IAORGDAContract
	{
		[OperationContract]
		List<AORArea> GetAORAreas();
		[OperationContract]
		List<SynchronousMachine> GetSyncMachinesForAreaGroupGid(List<long> areaGids);
		[OperationContract]
		List<AORGroup> GetGroupsForAgr(long groupGid);
		[OperationContract]
		List<AORArea> GetAreasForAgr(long areaGid);
	}
}
