using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.AORModel;
using FTN.Common.AORCachedModel;

namespace FTN.Common.AORContract
{
	/// <summary>
	/// Relationship between AOR Viewer app and AOR Service, SHOULD BE  treba napraviti da se preko jednog te istog proxy-a pristupa AOR cache-u.
	/// </summary>
	[ServiceContract]
	public interface IAORViewerCommunication
	{
		[OperationContract]
		List<Permission> GetAllPermissions();
		[OperationContract]
		List<string> GetUsernamesForDNA(string name);
		[OperationContract]
		void SerializeDNAs();
		[OperationContract]
		List<DNAAuthority> GetAllDNAs();
		[OperationContract]
		HashSet<AORCachedArea> GetAORAreas();
		[OperationContract]
		List<AORCachedGroup> GetAORGroups();
		[OperationContract]
		List<AORCachedUser> GetAllUsers();
		[OperationContract]
		List<string> GetPermissionsForArea(string name);
		[OperationContract]
		List<string> GetUsernamesForArea(string name);
		[OperationContract]
		List<string> GetGroupsForArea(string name);
		[OperationContract]
		List<string> GetAORAreasForUser(string username);
		//[OperationContract]
		//HashSet<AORCachedArea> GetAORGroup(); // to do vrati se mozda ce biti potrebno kasnije 
	}
}
