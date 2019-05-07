using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Security;
using FTN.Common.AORContract;
using FTN.Common.Model;

namespace AORManagementProxyNS
{
	public class AORManagementChannel : ClientBase<IAORManagement>, IAORManagement
	{
		public AORManagementChannel()
			: base("AORViewerComm") // vrati se da sredis ovo
		{
		}

		//public List<string> GetAORAreasForUsername(string username) // to do ubaciti ovde ili u neki zajednici interfejs za pristupanje kesu
		//{
		//	return this.Channel.GetAORAreasForUsername(username);
		//}

		public bool Login(string username, string password)
		{
			return this.Channel.Login(username, password);
		}
	}
}
