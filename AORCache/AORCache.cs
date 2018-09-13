using FTN.Common.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FTN.Common.AORModel;
using System.ServiceModel;
using FTN.Common.AORCachedModel;

namespace ActiveAORCache
{
	//[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class AORCache : IAORCache, IDisposable
	{
		public AORCacheModel AORCacheModel { get; private set; }

		public AORCache()
		{
			AORCacheModel = AORCacheModel.Instance;
		}

		public void SynchronizeAORConfig()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}

		public List<AORCachedArea> GetAORAreas()
		{
			return AORCacheModel.GetModelAORAreas();
		}

		public List<AORCachedGroup> GetAORGroups()
		{
			return AORCacheModel.GetModelAORGroups();
		}

		public List<User> GetAllUsers()
		{
			return AORCacheModel.GetAllUsers();
		}

		public List<Permission> GetPermissionsForArea(long areaId)
		{
			return AORCacheModel.GetPermissionsForArea(areaId);
		}
	}
}
