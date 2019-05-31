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
	/// Method used to access AOR cache
	/// </summary>
	public class AORCacheConfigurations
	{
		public static Dictionary<string, List<string>> GetAORGroupsForArea(string areaName)
		{
			Dictionary<string, List<string>> aorGroupsForArea = new Dictionary<string, List<string>>();

			using (var access = new AccessDB())
			{
				var area = access.Areas.Include("Groups").Where(a => a.Name.Equals(areaName)).FirstOrDefault();
				var areas = access.Areas.Include("Groups").ToList();

				Debug.Assert(area == null, "Nulcina je GetAORGroupsForArea");
			}
			return aorGroupsForArea;
		}

		public static List<string> GetPermissionsForArea(string areaName)
		{
			List<string> areaPermissions = new List<string>(10);
			AORCachedArea area = null;

			using (var access = new AccessDB())
			{
				try
				{
					area = access.Areas.Include("Permissions").Where(a => a.Name.Equals(areaName)).FirstOrDefault();
				}
				catch (Exception e)
				{
					throw;
				}

				Debug.Assert(area == null, "Nulcina je u GetPermissionsForArea ");
			}
			return areaPermissions;
		}

		public static List<string> GetPermissionsForUser(string username)
		{
			List<string> usersPermissions = new List<string>();

			using (var access = new AccessDB())
			{
				var user = access.Users.Include(x => x.DNAs.Select(y => y.PermissionList)).Where(u => u.Username.Equals(username)).ToList();
				Debug.Assert(user == null, "Nulcina je u GetPermissionsForArea ");
			}

			return usersPermissions;
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
					Trace.WriteLine("Failed to save state in SelectAreaForControl method");
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
					Trace.WriteLine(" areaQuery is empty.");
					return;
				}

				var area = areaQuery[0];
				area.IsViewable = isSelectedForView;

				int i = access.SaveChanges();

				if (i <= 0)
					Trace.WriteLine("Failed to save state in SelectAreaForView method");
			}
		}
	}
}
