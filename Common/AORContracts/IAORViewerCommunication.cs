using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.Model;
using FTN.Common.AORModel;

namespace FTN.Common.AORContract
{
	[ServiceContract]
	public interface IAORViewerCommunication
	{
		[OperationContract]
		List<Permission> GetAllPermissions();
		[OperationContract]
		void SerializeDNAs();
		[OperationContract]
		List<DNAAuthority> GetAllDNAs();
		[OperationContract]
		List<AORArea> GetAORAreas();
		[OperationContract]
		List<AORGroup> GetAORGroups();
		[OperationContract]
		List<Permission> GetPermissionsForArea(long areaId);
	}
}
