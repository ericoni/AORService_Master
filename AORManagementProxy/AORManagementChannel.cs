using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using AORCommon.AORManagementContract;
using System.Security;

namespace AORManagement
{
	public class AORManagementChannel : ClientBase<IAORManagement>, IAORManagement
	{
		public AORManagementChannel()
			: base("AORLoginEndpoint")
		{
		}

		public bool Login(string username, SecureString password)
		{
			return this.Channel.Login(username, password);
		}
	}
}
