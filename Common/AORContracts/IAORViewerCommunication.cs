using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.Model;
using FTN.Common.AORModel;
using FTN.Common.AORCachedModel;

namespace FTN.Common.AORContract
{
	/// <summary>
	/// Relationship between AOR Viewer app and AOR Service
	/// </summary>
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
		List<AORCachedArea> GetAORAreas();
		[OperationContract]
		List<AORCachedGroup> GetAORGroups();
		[OperationContract]
		List<User> GetAllUsers();
		[OperationContract]
		List<string> GetPermissionsForArea(string name);
		[OperationContract]
		List<string> GetUsernamesForDNA(string name);
		[OperationContract]
		List<string> GetUsernamesForArea(string name);
		[OperationContract]
		List<string> GetGroupsForArea(string name);
	}
}
