using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.Principal
{
	public class CustomPrincipal : IMyPrincipal
	{
		IIdentity identity;
		string username = string.Empty;
		List<string> areas;

		public CustomPrincipal(IIdentity identity, string username, List<string> areas)
		{
			this.identity = identity;
			this.username = username;
			this.Areas = areas;
		}

		public IIdentity Identity
		{
			get { return this.identity; }
		}

		public string Username
		{
			get { return this.username; }
			set { this.username = value; }
		}

		public List<string> Areas
		{
			get { return this.areas; }
			set { this.areas = value; }
		}

		public bool IsInRole(string role)
		{
			return true;
		}

		public WindowsImpersonationContext Impersonate()
		{
			throw new NotImplementedException();
		}
	}
}
