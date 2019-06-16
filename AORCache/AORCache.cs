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
    [Obsolete("Vidjeti da se vise nigdje ne koristi nego da se prisupa direktno na modelu", true)]
	public class AORCache : IAORCache, IDisposable
	{
		public AORCacheModel AORCacheModel { get; private set; }

		public AORCache()
		{
			AORCacheModel = AORCacheModel.Instance;
			var a = AORCacheModel.GetNewAORAreas();
		}

		public void SynchronizeAORConfig()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}
		
		public List<AORCachedGroup> GetAORGroups()
		{
			return AORCacheModel.GetModelAORGroups();
		}

		public List<User> GetAllUsers()
		{
			return AORCacheModel.GetAllUsers();
		}

		public List<string> GetPermissionsForArea(string name)
		{
			return AORCacheModel.GetPermissionsForArea(name);
		}
		public List<string> GetUsernamesForDNA(string name)
		{
			return AORCacheModel.GetUsernamesForDNA(name);
		}
		public List<string> GetUsernamesForArea(string name)
		{
			return AORCacheModel.GetUsernamesForArea(name);
		}
		public List<string> GetGroupsForArea(string name)
		{
			return AORCacheModel.GetGroupsForArea(name);
		}

		public HashSet<AORCachedArea> GetAORAreas()
		{
			return AORCacheModel.GetAORAreas();
		}
	}
}
