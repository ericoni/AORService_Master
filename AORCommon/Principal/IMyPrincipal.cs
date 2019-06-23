using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.Principal
{
    public interface IMyPrincipal : IPrincipal
    {
        string Username { get; set; }

        WindowsImpersonationContext Impersonate();
    }
}
