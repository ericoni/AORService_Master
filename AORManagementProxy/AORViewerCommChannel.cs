
using FTN.Common.AORContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.Model;

namespace AORManagementProxy
{
	public class AORViewerCommChannel : ClientBase<IAORViewerCommunication>, IAORViewerCommunication
	{
		public AORViewerCommChannel()
			: base("AORViewerCommEndpoint")
		{
		}

		public List<Permission> GetAllPermissions()
		{
			return this.Channel.GetAllPermissions();
		}

		public List<DNAAuthority> GetAllDNAs()
		{
			return this.Channel.GetAllDNAs();
		}

		public void SerializeDNAs()
		{
			this.Channel.SerializeDNAs();
		}
	}
}
