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
	/// Method used to access AOR cache. Method validation has to be improved.
	/// </summary>
	public class AORCacheConfigurations
	{

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

		public static string[] GetAORAreasForUsername(string username)
		{
			string[] areas;
			List<User> user = null;

			using (var access = new AccessDB())
			{
				try
				{
					user = access.Users.Include("Areas").Where(u => u.Username.Equals(username)).ToList();
				}
				catch (Exception e)
				{
					throw;
				}

				if (user.Count == 0)
				{
					return new string[1] { "None" };
				}

				areas = new string[user[0].Areas.Count];

				for (int i = 0; i < user[0].Areas.Count; i++)
				{
					areas[i] = user[0].Areas[i].Name;
				}
			}

			return areas;
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
