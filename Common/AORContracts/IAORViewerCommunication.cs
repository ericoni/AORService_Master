using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.Model;

namespace FTN.Common.AORContract
{
	[ServiceContract]
	public interface IAORViewerCommunication
	{
		[OperationContract]
		List<Permission> GetAllPermissions();
	}
}
