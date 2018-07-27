using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORService.Access
{
	public interface IUserHelperDB
	{
	 bool UserAuthentication(string username, string password);
	}
}
