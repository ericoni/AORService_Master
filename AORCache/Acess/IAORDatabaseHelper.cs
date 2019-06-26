using FTN.Common.AORCachedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORC.Acess
{
	public interface IAORDatabaseHelper
	{
		List<AORCachedArea> LoginUser(string username, string password);
	}
}
