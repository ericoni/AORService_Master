﻿using FTN.Common;
using FTN.Common.AORModel;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

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
		List<SynchronousMachine> GetAllDERsWithFullInfo();

		[OperationContract]
        List<long> GetAnalogValuesGidForGidAndPowerType(long gid, PowerType powerType);

        [OperationContract]
        List<long> GetAllAnalogValuesGidForPowerType(PowerType powerType);

		[OperationContract]
		SynchronousMachine GetSyncMachineByGid(long gid);
	}

}
