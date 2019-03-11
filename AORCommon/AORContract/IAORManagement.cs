using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon.AORContract
{
	/// <summary>
	/// Relationship between AOR service and DERMS app
	/// </summary>
	[ServiceContract]
	public interface IAORManagement
	{
		[OperationContract]
		bool Login(string username, string password);
	}
}
