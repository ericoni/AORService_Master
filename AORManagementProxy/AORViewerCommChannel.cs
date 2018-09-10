
using FTN.Common.AORContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.Model;
using FTN.Common.AORModel;
using FTN.Common.AORCachedModel;

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

		public List<AORCachedArea> GetAORAreas()
		{
			return this.Channel.GetAORAreas();
		}

		public List<AORGroup> GetAORGroups()
		{
			return this.Channel.GetAORGroups();
		}

		public void SerializeDNAs()
		{
			this.Channel.SerializeDNAs();
		}

		public List<Permission> GetPermissionsForArea(long areaId)
		{
			return this.Channel.GetPermissionsForArea(areaId);
		}
	}
}
