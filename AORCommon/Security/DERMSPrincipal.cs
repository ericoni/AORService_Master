using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.Security
{
	public class DERMSPrincipal : IDERMSPrincipal
	{
		public string CallingComputer
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string ClientHostName
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public IIdentity Identity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool PermissionChecks
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string Username
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public List<string> GetAORGroups(List<string> specifiedPermissions)
		{
			throw new NotImplementedException();
		}

		public bool IsInRole(string role)
		{
			throw new NotImplementedException();
		}

		public bool IsInRole(SecurityIdentifier sid)
		{
			throw new NotImplementedException();
		}
	}
}
