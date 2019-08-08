using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System.Collections.Generic;
using System.ServiceModel;

namespace FTN.ServiceContracts
{
	[ServiceContract]
	public interface INetworkModelClient
	{
		[OperationContract]
		Substation GetSubstation(long gid);

		[OperationContract]
		List<Substation> GetAllSubstation();

		[OperationContract]
		List<GeographicalRegion> GetRegions();

		[OperationContract]
		List<SubGeographicalRegion> GetSubRegionsForRegion(long regionGid);

		[OperationContract]
		List<Substation> GetSubstationsForSubRegion(long subregionGid);

		[OperationContract]
		List<SynchronousMachine> GetDERs(long gid);

		[OperationContract]
		List<long> GetAnalogValuesGidForGidAndPowerType(long gid, PowerType powerType);

		[OperationContract]
		List<long> GetAllAnalogValuesGidForPowerType(PowerType powerType);

		[OperationContract]
		SynchronousMachine GetSyncMachineByGid(long gid);
		[OperationContract]
		string GetStaticModelResourceNameByGid(long gid);
	}

}
