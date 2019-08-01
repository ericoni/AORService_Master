using FTN.Common.AORContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.AORModel;
using System.ServiceModel;
using FTN.Common.AORCachedModel;
using ActiveAORCache;

namespace AORService
{
	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class AORViewerComm : IAORViewerCommunication
	{
		//private AORCache aorCache = null;
		private AORXmlHelper aorXmlHelper;

		public AORViewerComm()
		{
			aorXmlHelper = new AORXmlHelper();
			//aorCache = new AORCache();
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

		//public List<AORCachedGroup> GetAORGroups()
		//{
		//	return aorCache.GetAORGroups();
		//}

		//public List<User> GetAllUsers()
		//{
		//	return aorCache.GetAllUsers();
		//}
	
		//public List<string> GetUsernamesForDNA(string name)
		//{
		//	return aorCache.GetUsernamesForDNA(name);
		//}
		//public HashSet<AORCachedArea> GetAORAreas()
		//{
		//	return aorCache.GetAORAreas();
		//}
		//public List<string> GetUsernamesForArea(string name)
		//{
		//	return aorCache.GetUsernamesForArea(name);
		//}
		//public List<string> GetGroupsForArea(string name)
		//{
		//	return aorCache.GetGroupsForArea(name);
		//}
		//public List<string> GetPermissionsForArea(string name)
		//{
		//	return aorCache.GetPermissionsForArea(name);
		//}

		public List<string> GetAORAreasForUser(string username)
		{
			throw new NotImplementedException();
		}

		public List<string> GetUsernamesForDNA(string name)
		{
			throw new NotImplementedException();
		}

		public HashSet<AORCachedArea> GetAORAreas()
		{
			throw new NotImplementedException();
		}

		public List<AORCachedGroup> GetAORGroups()
		{
			throw new NotImplementedException();
		}

		public List<AORCachedUser> GetAllUsers()
		{
			throw new NotImplementedException();
		}

		public List<string> GetPermissionsForArea(string name)
		{
			throw new NotImplementedException();
		}

		public List<string> GetUsernamesForArea(string name)
		{
			throw new NotImplementedException();
		}

		public List<string> GetGroupsForArea(string name)
		{
			throw new NotImplementedException();
		}
	}
}
