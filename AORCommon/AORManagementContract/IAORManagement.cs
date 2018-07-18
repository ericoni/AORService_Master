using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.AORManagementContract
{
	[ServiceContract]
	public interface IAORManagement
	{
		[OperationContract]
		bool Login(string username, string password);
	}
}
