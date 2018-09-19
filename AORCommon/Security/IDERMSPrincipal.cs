using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.Security
{
	public interface IDERMSPrincipal : IPrincipal, IDisposable
	{
		/// <summary>
		/// Indicates if principal is a member of supplied Security Identifier object
		/// </summary>
		/// <param name="sid"></param>
		/// <returns></returns>
		bool IsInRole(SecurityIdentifier sid);

		/// <summary>
		/// Gets/sets principal's Client Host Name
		/// </summary>
		string ClientHostName { get; set; }

		/// <summary>
		/// Gets principal's Calling Computer
		/// </summary>
		string CallingComputer { get; }

		/// <summary>
		/// Gets/sets indication if permission checks are forced for principal
		/// </summary>
		bool PermissionChecks { get; set; }

		/// <summary>
		/// Gets a list of AOR groups with supplied permissions that the principal is a member of. Filter specifies if AND or OR condition is forced for supplied AOR permissions
		/// </summary>
		/// <param name="specifiedPermissions"></param>
		/// <param name="specifiedPermissionFilter"></param>
		/// <returns></returns>
		List<string> GetAORGroups(List<string> specifiedPermissions);//, AreaPermissionFilter specifiedPermissionFilter = AreaPermissionFilter.Or);

		/// <summary>
		/// Represents logged in user
		/// </summary>
		string Username { get; set; }
	}
}
