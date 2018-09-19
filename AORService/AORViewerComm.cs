using FTN.Common.AORContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.Model;
using FTN.Common.AORModel;
using ActiveAORCache;
using System.ServiceModel;
using FTN.Common.AORCachedModel;

namespace AORService
{
	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class AORViewerComm : IAORViewerCommunication
	{
		private AORCache aorCache = null;

		/// <summary>
		/// Helper class used for XML (de)serialization
		/// </summary>
		private AORXmlHelper aorXmlHelper;

		public AORViewerComm()
		{
			aorXmlHelper = new AORXmlHelper();
			aorCache = new AORCache();
		}

		public List<Permission> GetAllPermissions()
		{
			return aorXmlHelper.ReadPermXml();
		}

		public void SerializeDNAs()
		{
			aorXmlHelper.CreateXml(true);
		}

		public List<DNAAuthority> GetAllDNAs()
		{
			return aorXmlHelper.ReadDNAFromXml();
		}

		public List<AORCachedArea> GetAORAreas() // vrati se analizirati sta se od ovoga koristi
		{
			return aorCache.GetAORAreas();
		}

		public List<AORCachedGroup> GetAORGroups()
		{
			return aorCache.GetAORGroups();
		}

		public List<User> GetAllUsers()
		{
			return aorCache.GetAllUsers();
		}
		public List<string> GetPermissionsForArea(string name)
		{
			return aorCache.GetPermissionsForArea(name);
		}
		public List<string> GetUsernamesForDNA(string name)
		{
			return aorCache.GetUsernamesForDNA(name);
		}

		public List<string> GetUsernamesForArea(string name)
		{
			return aorCache.GetUsernamesForArea(name);
		}
		public List<string> GetGroupsForArea(string name)
		{
			return aorCache.GetGroupsForArea(name);
		}
	}
}
