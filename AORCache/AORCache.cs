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

namespace ActiveAORCache
{
	//[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class AORCache : IAORCache, IDisposable
	{
		/// <summary>
		/// Lock object for Singleton implementation
		/// </summary>
		private static object syncRoot = new Object();

		/// <summary>
		/// Singleton instance
		/// </summary>
		//private static volatile AORCache instance;

		public AORCacheModel AORCacheModel { get; private set; }

		/// <summary>
		/// Singleton method
		/// </summary>
		//public static AORCache Instance
		//{
		//	get
		//	{
		//		if (instance == null)
		//		{
		//			lock (syncRoot)
		//			{
		//				if (instance == null)
		//					instance = new AORCache();
		//			}
		//		}

		//		return instance;
		//	}
		//}

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

		public List<AORArea> GetAORAreas()
		{
			return AORCacheModel.GetModelAORAreas();
		}

		public List<AORGroup> GetAORGroups()
		{
			return AORCacheModel.GetModelAORGroups();
		}

		public List<Permission> GetPermissionsForArea(long areaId)
		{
			return AORCacheModel.GetPermissionsForArea(areaId);
		}
	}
}
