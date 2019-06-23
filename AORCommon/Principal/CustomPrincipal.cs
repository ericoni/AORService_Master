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
        string[] areas;

        public CustomPrincipal(IIdentity identity, string username, string[] roles)
        {
            this.identity = identity;
            this.username = username;
            this.areas = roles;
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

        public string[] Areas
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
