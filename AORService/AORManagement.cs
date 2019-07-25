using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using FTN.Common.AORContract;
using ActiveAORCache;
using AORC.Acess;
using ActiveAORCache.Helpers;
using System.ServiceModel;
using System.Threading;
using AORCommon.Principal;
using FTN.Common.AORCachedModel;
using EventCollectorProxyNS;
using EventCommon;

namespace AORService
{
	/// <summary>
	/// TODO: AOR login trenutno poziva aorDatabaseHelper DB, po konstruktoru, ovaj bi trebao da handle logovanje-ako se dobro sjecam
	/// </summary>
	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class AORManagement : IAORManagement
	{
		private AORDatabaseHelper aorDatabaseHelper = null;
		private EventCollectorProxy eventCollectorProxy = null;

		public AORManagement()
		{
			try
			{
				aorDatabaseHelper = new AORDatabaseHelper();
				eventCollectorProxy = new EventCollectorProxy();
				//var a = AORCacheConfigurations.GetAORAreaObjectsForUsername("admin");
				//var c = AORCacheConfigurations.GetPermissionsForArea("West-Area");
				//var d = AORCacheConfigurations.GetPermissionsForAreas(new List<string>() { "West-Area", "East-Area" });
				//var f = AORCacheConfigurations.GetPermissionsForUser("state");
				//var g = AORCacheConfigurations.GetAORGroupsForArea("West-Area");
				//var h = AORCacheConfigurations.GetAORGroupsForAreasUnsafe(new List<string>() { "West-Area", "East-Area" });
				//var j = AORCacheConfigurations.GetSyncMachineGidsForAORGroups(new List<string>(2) { "group_2", "group_3" });
			}
			catch (Exception e)
			{
				Trace.Write(e.StackTrace);
				throw e;
			}
		}

		public List<string> GetAORAreasForUsername(string username)
		{
			var aorCachedAreas =  AORCacheConfigurations.GetAORAreaObjectsForUsername(username);
			List<string> aorAreas = new List<string>(aorCachedAreas.Count);

			foreach (var cachedArea in aorCachedAreas)
			{
				aorAreas.Add(cachedArea.Name);
			}

			return aorAreas;
		}

		public List<long> GetUsersSynchronousMachines()
		{
			throw new NotImplementedException();
		}

		#region IAORManagement

		public List<AORCachedArea> Login(string username, string password)
		{
			List<AORCachedArea> aorCachedAreas = aorDatabaseHelper.LoginUser(username, password);
			string areasCombinedString = AreasCombinedString(aorCachedAreas);

			var p = Thread.CurrentPrincipal;
			eventCollectorProxy = new EventCollectorProxy(); //to do izbaciti ga posle u konstruktor
			eventCollectorProxy.Proxy.SendEvent(new Event(username, "User logged in with specified AOR areas: " + areasCombinedString, "region1", DateTime.Now));

			return aorCachedAreas;
		}

		/// <summary>
		/// Ne znam za sta je ovo ubaceno.
		/// </summary>
		public void Test()
		{
			string a = "sarma";
			var principal = Thread.CurrentPrincipal;
			var p2 = ServiceSecurityContext.Current;
			var p3 = Thread.CurrentPrincipal as IMyPrincipal;
		}

		#endregion

		#region Private methods

		private string AreasCombinedString(List<AORCachedArea> areas)
		{
			string combinedString = string.Empty;
			List<string> areasStringList = new List<string>(areas.Count);

			foreach (var area in areas)
			{
				areasStringList.Add(area.Name);
			}
			combinedString = string.Join(",", areasStringList);
			return combinedString;
		}

		private String SecureStringToString(SecureString value)
		{
			IntPtr valuePtr = IntPtr.Zero;
			try
			{
				valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
				return Marshal.PtrToStringUni(valuePtr);
			}
			catch (Exception e)
			{
				Console.Write(e.StackTrace);
				return string.Empty;
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
			}
		}
		#endregion
	}
}
