using AORC.Acess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using FTN.Common.AORCachedModel;
using System.Threading;

namespace ActiveAORCache.Helpers
{
	/// <summary>
	/// Method used to access AOR cache. Method validation has to be improved. Check for optimizations in newest methods.
	/// </summary>
	public class AORCacheConfigurations
	{
		//public static List<long> GetSyncMachinesForUser()
		//{


		//}

		public static Dictionary<string, List<long>> GetSyncMachineGidsForAORGroups(List<string> aorGroups)
		{
			Dictionary<string, List<long>> resultGids = new Dictionary<string, List<long>>();
			List<AORCachedGroup> groups = new List<AORCachedGroup>(12);
			List<long> syncMachines = null;

			using (var access = new AccessDB())
			{
				groups = access.Groups.Include("SynchronousMachines").ToList();
			}

			foreach (var group in groups)
			{
				if (!aorGroups.Contains(group.Name))
				{
					continue;
				}

				syncMachines = new List<long>(group.SynchronousMachines.Count);

				foreach (var sm in group.SynchronousMachines)
				{
					syncMachines.Add(sm.GidFromNms);
				}
				resultGids.Add(group.Name, syncMachines);
			}
			return resultGids;
		}

		public static List<long> GetSyncMachineGidsForAORGroup(string aorGroup)
		{
			List<long> syncMachines = new List<long>();

			using (var access = new AccessDB())
			{
				var group = access.Groups.Include("SynchronousMachines").Where(g => g.Name.Equals(aorGroup)).FirstOrDefault();

				if (group == null)
				{
					Trace.WriteLine("ERROR : Failed to find group in GetSyncMachineGidsForAORGroup method");
					return new List<long>(1) { -1 };
				}

				foreach (var sm in group.SynchronousMachines)
				{
					syncMachines.Add(sm.GidFromNms);
				}
			}
			return syncMachines;
		}

		public static List<string> GetAORGroupsForArea(string areaName)
		{
			List<string> aorGroupsForArea = new List<string>();

			using (var access = new AccessDB())
			{
				var area = access.Areas.Include("Groups").Where(a => a.Name.Equals(areaName)).FirstOrDefault();

				foreach (var group in area.Groups)
				{
					aorGroupsForArea.Add(group.Name);
				}

				Debug.Assert(area == null, "Null in GetAORGroupsForArea()");
			}
			return aorGroupsForArea;
		}

		/// <summary>
		/// Expecting valid area names on the input. 
		/// </summary>
		/// <param name="areaNames"></param>
		/// <returns></returns>
		public static Dictionary<string, List<string>> GetAORGroupsForAreasUnsafe(List<string> areaNames)
		{
			Dictionary<string, List<string>> aorGroupsForAreas = new Dictionary<string, List<string>>();
			List<string> tempGroups = null;

			using (var access = new AccessDB())
			{
				foreach (var areaName in areaNames)
				{
					var area = access.Areas.Include("Groups").Where(a => a.Name.Equals(areaName)).FirstOrDefault();

					tempGroups = new List<string>(area.Groups.Count);

					foreach (var group in area.Groups)
					{
						tempGroups.Add(group.Name);
					}
					aorGroupsForAreas.Add(areaName, tempGroups);
				}
			}
			return aorGroupsForAreas;
		}

		public static List<string> GetPermissionsForArea(string areaName) 
		{
			List<string> areaPermissions = new List<string>(10);

			using (var access = new AccessDB())
			{

				var area = access.Areas.Include("Permissions").Where(a => a.Name.Equals(areaName)).FirstOrDefault();

				foreach (var permission in area.Permissions)
				{
					areaPermissions.Add(permission.Name);
				}
			}
			return areaPermissions;
		}

		public static Dictionary<string, List<string>> GetPermissionsForAreas(List<string> demandedAreaNames)
		{
			List<string> areaPermissions = new List<string>(10);
			Dictionary<string, List<string>> returnValue = new Dictionary<string, List<string>>(10);

			using (var access = new AccessDB())
			{
				var areas = access.Areas.Include("Permissions").ToList();

				foreach (var demandedAreaName in demandedAreaNames)
				{
					foreach (var area in areas)
					{
						if (area.Name.Equals(demandedAreaName))
						{
							foreach (var permission in area.Permissions)
							{
								areaPermissions.Add(permission.Name);
							}
							returnValue.Add(area.Name, areaPermissions);
							areaPermissions = new List<string>(10);
						}
					}
				}
			}
			return returnValue;
		}

		public static List<string> GetPermissionsForUser(string username)
		{
			List<string> permissions = new List<string>();

			using (var access = new AccessDB())
			{
				var user = access.Users.Include(x => x.DNAs.Select(y => y.PermissionList)).Where(u => u.Username.Equals(username)).FirstOrDefault();

				Debug.Assert(user == null, "Null in GetPermissionsForUser ");

				foreach (var dna in user.DNAs)
				{
					foreach (var permission in dna.PermissionList)
					{
						permissions.Add(permission.Name);
					}
				}
			}
			return permissions;
		}

		/// <summary>
		/// Needs to be optimized.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static List<AORCachedArea> GetAORAreaObjectsForUsername(string username)
		{
			List<AORCachedArea> aorAreas = new List<AORCachedArea>(10);
			List<AORCachedArea> aorAreaReturnValue = new List<AORCachedArea>(10);

			using (var access = new AccessDB())
			{
				aorAreas = access.Areas.Include(a => a.Groups.Select(y => y.SynchronousMachines)).Include("Users").Include("Permissions").ToList();
			}

			if (aorAreas.Count == 0)// || user == null)
			{
				return new List<AORCachedArea>();
			}

			foreach (var area in aorAreas)
			{
				foreach (var user in area.Users)
				{
					if (user.Username.Equals(username))
					{
						aorAreaReturnValue.Add(area);
						break;
					}
				}
			}

			return aorAreaReturnValue;
		}

		public static List<string> GetAORAreasForUsername(string username)
		{
			List<string> areaNames = null;
			AORCachedUser user = null;

			using (var access = new AccessDB())
			{
				try
				{
					user = access.Users.Include("Areas").Where(u => u.Username.Equals(username)).FirstOrDefault();
				}
				catch (Exception e)
				{
					throw;
				}

				if (user == null)
				{
					return new List<string>();
				}

				areaNames = new List<string>(user.Areas.Count);

				foreach (var area in user.Areas)
				{
					areaNames.Add(area.Name);
				}
			}

			return areaNames;
		}

		public static Dictionary<long, List<string>> GetAORGroupsForSyncMachines(List<long> smGids)
		{
			//List<AORCachedGroup> aorGroups = null;
			//using (var access = new AccessDB())
			//{
			//	aorGroups = access.Groups.Include("SynchronousMachines").ToList();
			//}

			//foreach (var aorGroup in aorGroups)
			//{

			//}
			using (var access = new AccessDB())
			{
				var sms = access.SynchronousMachines.ToList();
			}

			return new Dictionary<long, List<string>>();
		}

		//public static void SelectAreaForControl(string areaName, bool isSelectedForControl)
		//{
		//	if (areaName == string.Empty)
		//		return;

		//	using (var access = new AccessDB())
		//	{
		//		var area = access.Areas.Where(a => a.Name.Equals(areaName)).FirstOrDefault();
		//		area.IsControllable = isSelectedForControl;

		//		int i = access.SaveChanges();

		//		if (i <= 0)
		//			Trace.WriteLine("ERROR: Failed to save state in SelectAreaForControl method");
		//	}
		//}

		//public static void SelectAreaForView(string areaName, bool isSelectedForView)
		//{

		//	if (areaName == string.Empty)
		//		return;

		//	using (var access = new AccessDB())
		//	{
		//		var areaQuery = access.Areas.Where(a => a.Name.Equals(areaName)).FirstOrDefault();

		//		if (areaQuery == null)
		//		{
		//			Trace.WriteLine("ERROR: areaQuery in SelectAreaForView is empty.");
		//			return;
		//		}

		//		areaQuery.IsViewable = isSelectedForView;

		//		int i = access.SaveChanges();

		//		if (i <= 0)
		//			Trace.WriteLine("ERROR: Failed to save state in SelectAreaForView method");
		//	}
		//}

		/// <summary>
		/// HARCDODED SECTION, potrebno je ispraviti ovo posle faze testiranja.
		/// </summary>
		/// <param name="areaName"></param>
		/// <param name="isSelectedForControl"></param>
		/// <returns></returns>
		public static bool SelectAreaForControl(string areaName, bool isSelectedForControl)
		{
			if (areaName == string.Empty)
				return false;

			//var principal = Thread.CurrentPrincipal;
			//String name = principal.Identity.Name;
			//int backslashLastIndex = name.LastIndexOf('\\');
			//string username = name.Substring(backslashLastIndex + 1);
			string username = "marko.markovic";//to do hardcoded for now

			using (var access = new AccessDB())
			{
				var areaQuery = access.Areas.Where(a => a.Name.Equals(areaName)).FirstOrDefault();
				var userQuery = access.Users.Where(u => u.Username.Equals(username)).FirstOrDefault();

				if (areaQuery == null || userQuery == null)
				{
					Trace.WriteLine("ERROR: areaQuery/userQuery in SelectAreaForControl is empty.");
					return false;
				}

				var x = access.CachedUserAreasNew.ToList();
				var y = access.CachedUserAreasNew.Where(a => a.AreaId == areaQuery.AreaId).FirstOrDefault();

				var userAreasCombined = access.CachedUserAreasNew.Where(a => a.AreaId == areaQuery.AreaId).Where(a=>a.UserId == userQuery.UserId).FirstOrDefault();

				if (userAreasCombined == null)
				{
					Trace.WriteLine("ERROR: userAreasCombined in SelectAreaForControl is empty.");
					return false;
				}

				userAreasCombined.IsSelectedForControl = isSelectedForControl;

				if (isSelectedForControl)
				{
					areaQuery.NumberOfUsersControling++;
				}
				else
				{
					areaQuery.NumberOfUsersControling--;
				}

				int i = access.SaveChanges();

				if (i <= 0)
				{
					Trace.WriteLine("ERROR: Failed to save state in SelectAreaForControl method");
					return false;
				}
				return true;
			}
		}

		public static bool SelectAreaForView(string areaName, bool isSelectedForView)
		{
			if (areaName == string.Empty)
				return false;

			using (var access = new AccessDB())
			{
				var areaQuery = access.Areas.Where(a => a.Name.Equals(areaName)).FirstOrDefault();

				if (areaQuery == null)
				{
					Trace.WriteLine("ERROR: areaQuery in SelectAreaForView is empty.");
					return false;
				}

				var userAreasCombined = access.CachedUserAreasNew.Where(a => a.AreaId == areaQuery.AreaId).FirstOrDefault();

				if (userAreasCombined == null)
				{
					Trace.WriteLine("ERROR: userAreasCombined in SelectAreaForView is empty.");
					return false;
				}

				userAreasCombined.IsSelectedForView = isSelectedForView;

				int i = access.SaveChanges();

				if (i <= 0)
				{
					Trace.WriteLine("ERROR: Failed to save state in SelectAreaForView method");
					return false;
				}
				return true;
			}
		}

	}
}
