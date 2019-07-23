using AORC.Acess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using FTN.Common.Model;
using FTN.Common.AORCachedModel;

namespace ActiveAORCache.Helpers
{
	/// <summary>
	/// Method used to access AOR cache. Method validation has to be improved. Check for optimizations in newest methods.
	/// </summary>
	public class AORCacheConfigurations
	{
		// to do jun
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

		public static HashSet<string> GetPermissionsForUser(string username)
		{
			HashSet<string> permissions = new HashSet<string>();

			using (var access = new AccessDB())
			{
				var user = access.Users.Include(x => x.DNAs.Select(y => y.PermissionList)).Where(u => u.Username.Equals(username)).ToList();

				Debug.Assert(user == null || user.Count == 0, "Null in GetPermissionsForUser ");

				foreach (var dna in user[0].DNAs)
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
				aorAreas = access.Areas.Include(a => a.Groups.Select(y => y.SynchronousMachines)).Include("Users").ToList();
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
			User user = null;

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

		public static void SelectAreaForControl(string areaName, bool isSelectedForControl)
		{
			using (var access = new AccessDB())
			{
				if (areaName == string.Empty)
					return;

				var area = access.Areas.Where(a => a.Name.Equals(areaName)).ToList()[0];
				area.IsControllable = isSelectedForControl;

				int i = access.SaveChanges();

				if (i <= 0)
					Trace.WriteLine("ERROR: Failed to save state in SelectAreaForControl method");
			}
		}
	
		public static void SelectAreaForView(string areaName, bool isSelectedForView)
		{
			using (var access = new AccessDB())
			{
				if (areaName == string.Empty)
					return;

				var areaQuery = access.Areas.Where(a => a.Name.Equals(areaName)).ToList();

				if (areaQuery.Count == 0)
				{
					Trace.WriteLine("ERROR: areaQuery in SelectAreaForView is empty.");
					return;
				}

				var area = areaQuery[0];
				area.IsViewable = isSelectedForView;

				int i = access.SaveChanges();

				if (i <= 0)
					Trace.WriteLine("ERROR: Failed to save state in SelectAreaForView method");
			}
		}
	}
}
