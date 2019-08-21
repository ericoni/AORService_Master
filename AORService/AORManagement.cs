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
using System.Security.Principal;

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

				//var aorAreaNames = aorManagementProxy.Proxy.GetAORAreasForUsername("admin");
				//var areasGroupsMapping = aorManagementProxy.Proxy.GetAORGroupsAreasMapping(aorAreaNames);
				//var x = AORCacheConfigurations.GetAORGroupsForSyncMachines(new List<long>() { 1 });
				//eventCollectorProxy = new EventCollectorProxy();//to do vrati se ovde za evente, nakon sto prodje db initialization

				//var a = AORCacheConfigurations.GetAORAreaObjectsForUsername("admin");//samo se ovako testiraj, a ne iz console app iz drugog proj!!! BITNO
				//var c = AORCacheConfigurations.GetPermissionsForArea("West-Area");
				//var d = AORCacheConfigurations.GetPermissionsForAreas(new List<string>() { "West-Area", "East-Area" });
				//var g = AORCacheConfigurations.GetPermissionsForUser("admin");

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

			eventCollectorProxy = new EventCollectorProxy();
			//eventCollectorProxy.Proxy.SendEvent(new Event(username, "User logged in with specified AOR areas: " + areasCombinedString, null, DateTime.Now));//to do hardcoded for a moment
			eventCollectorProxy.Proxy.SendEvent(new Event(username, "User 'petar.petrovic' logged in with specified AOR areas:  Backa-Area, Zrenjanin-Area, NoviBecej-Area", null, DateTime.Now));//to do hardcoded for a moment

			return aorCachedAreas;
		}

		public bool SelectAreaForControl(string areaName, bool isSelectedForControl)
		{
			return AORCacheConfigurations.SelectAreaForControl(areaName, isSelectedForControl);
		}

		public bool SelectAreaForView(string areaName, bool isSelectedForView)
		{
			return AORCacheConfigurations.SelectAreaForView(areaName, isSelectedForView);
		}
		public List<string> GetPermissionsForUser(string username)
		{
			return AORCacheConfigurations.GetPermissionsForUser(username);
		}

		public Dictionary<string, List<string>> GetAORGroupsAreasMapping(List<string> areaNames)
		{
			return AORCacheConfigurations.GetAORGroupsForAreasUnsafe(areaNames);
		}

		public Dictionary<long, List<string>> GetAORGroupsForSyncMachines(List<long> smGids)
		{
			return AORCacheConfigurations.GetAORGroupsForSyncMachines(smGids);
		}
		public List<AORCachedArea> GetAORAreaObjectsForUsername(string username)
		{
			return AORCacheConfigurations.GetAORAreaObjectsForUsername(username);
		}
		/// <summary>
		/// Ne znam za sta je ovo ubaceno.
		/// </summary>
		public void Test(IPrincipal p)
		{
			string a = "sarma";
			var principal = Thread.CurrentPrincipal;
			var p2 = ServiceSecurityContext.Current;
			var p3 = Thread.CurrentPrincipal as IMyPrincipal;//bitna napomena sve 3 su ekvivalentni.
			Console.WriteLine(principal.Identity.Name);
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
