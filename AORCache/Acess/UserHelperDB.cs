using System;
using System.Collections.Generic;
using System.Linq;
using FTN.Common.Model;
using Adapter;
using FTN.Common.AORCachedModel;

namespace AORC.Acess
{
	public class UserHelperDB : IUserHelperDB
	{
		private static IUserHelperDB myDB;
		private RDAdapter rdAdapter = null;
		private List<AORCachedGroup> aorGroups = null;

		public static IUserHelperDB Instance
		{
			get
			{
				if (myDB == null)
					myDB = new UserHelperDB();

				return myDB;
			}
			set
			{
				if (myDB == null)
					myDB = value;
			}
		}

		public UserHelperDB() 
		{
			rdAdapter = new RDAdapter();
			aorGroups = rdAdapter.GetAORGroupsWithSmInfo(); // aorGroups with sm info

			using (var access = new AccessDB())
			{
				if (access.Users.Count() == 0)
				{
					#region perms
					Permission p1 = new Permission("DNA_PermissionControlSCADA", "Permission to issue commands towards SCADA system.");
					Permission p2 = new Permission("DNA_PermissionUpdateNetworkModel", "Permission to apply delta (model changes)- update current network model within their assigned AOR");
					Permission p3 = new Permission("DNA_PermissionViewSystem", "Permission to view content of AORViewer");
					Permission p4 = new Permission("DNA_PermissionSystemAdministration", "Permission to view system settings in AORViewer");
					Permission p5 = new Permission("DNA_PermissionViewSecurity", "Permission to view security content of AORViewer");
					Permission p6 = new Permission("DNA_PermissionSecurityAdministration", "Permission to edit security content of AORViewer");
					Permission p7 = new Permission("DNA_PermissionViewAdministration", "Permission to edit security content of AORViewer");
					Permission p8 = new Permission("DNA_PermissionViewSCADA", "Permission to view content operating under SCADA system.");

					IList<Permission> perms = new List<Permission>() { p1, p2, p3, p4, p5, p6, p7, p8 };
					access.Permissions.AddRange(perms);

					int k = access.SaveChanges();

					if (k <= 0)
						throw new Exception("Failed to save permissions.");
					#endregion
					IList<Permission> halfPerms = new List<Permission> { p1, p2, p3, p4 };
					IList<Permission> almostFullPerms = new List<Permission> { p1, p2, p3, p4, p5, p8 };

					#region DNAs
					DNAAuthority dna1 = new DNAAuthority("DNA_AuthorityDispatcher", new List<Permission>() { p1, p8, p5, p7 }); 
					DNAAuthority dna2 = new DNAAuthority("DNA_AuthorityNetworkEditor", new List<Permission>() { p2 });
					DNAAuthority dna3 = new DNAAuthority("DNA_SCADAAdmin", "Provides complete control to all aspects of the SCADA system.", new List<Permission>() { p1, p8 });
					DNAAuthority dna4 = new DNAAuthority("DNA_Viewer", "Required for a user to access the SCADA system.  Provides non-interactive access to data according to AOR.", new List<Permission>() { p3, p5, p7, p8 });
					DNAAuthority dna5 = new DNAAuthority("DNA_DMSAdmin", new List<Permission>() {p3, p5, p7 });
					DNAAuthority dna6 = new DNAAuthority("DNA_Operator", new List<Permission>() { p1, p2, p8 }); 
					IList<DNAAuthority> dnaAuthorities = new List<DNAAuthority>() { dna1, dna2, dna3, dna4, dna5, dna6 };
					access.DNAs.AddRange(dnaAuthorities);

					int l = access.SaveChanges();

					if (l <= 0)
						throw new Exception("Failed to save DNAs in UserHelperDB");
					#endregion DNAs

					User u1 = null;
					User u2 = null;

					AORCachedArea area1 = new AORCachedArea("West-Area", "", (List<Permission>)halfPerms, new List<User>() { u1 } , aorGroups); // dodati im usera naknadno
					AORCachedArea area2 = new AORCachedArea("East-Area", "", (List<Permission>)almostFullPerms, new List<User>() { u2 }, aorGroups);


					#region Users
					for (int i = 1; i < 3; i++)
					{
						//access.Users.Add(new User("admin" + i.ToString(), SecurePasswordManager.Hash("admin")));
						if (i == 2)
						{
							u1 = new User("a" + i.ToString(), "a", new List<DNAAuthority>() { dna1, dna4, dna6 }, new List<AORCachedArea>() { area1 }, new List<AORCachedArea>() { area1 });
							//access.Users.Add(new User("a" + i.ToString(), "a", new List<DNAAuthority>() { dna1, dna4, dna6 }, new List<AORCachedArea>() { area1 }, new List<AORCachedArea>() { area1 }));
							access.Users.Add(u1);
							continue;
						}
						u2 = new User("a" + i.ToString(), "a", new List<DNAAuthority>() { dna2, dna4, dna6 }, new List<AORCachedArea>() { area2 }, new List<AORCachedArea>() { area2 });
						//access.Users.Add(new User("a" + i.ToString(), "a", new List<DNAAuthority>() { dna2, dna4, dna6 }, new List<AORCachedArea>() { area2 }, new List<AORCachedArea>() { area2 }));
						access.Users.Add(u2);
					}
					
					
					//area1.Users.Add(u1);
					//area2.Users.Add(u2);

					int j = access.SaveChanges();

					if (j <= 0)
						throw new Exception("Failed to save user changes!");

					// problem kod dodavanja nove area, na vec postojeceg usera. Ne doda se novi area entitet u bazu!

	 //               var user1 = access.Users.Where(u => u.Username.Equals("a1")).ToList()[0]; // vrati se ovde da uljudis kod
					//var user2 = access.Users.Where(u => u.Username.Equals("a2")).ToList()[0]; //izvlacenje usera

	 //               //var a1 = access.Areas.Where(a => a.Name.Equals("West-Area")).ToList()[0]; //dodjela tog usera na AREA-a
	 //               //a1.User.Add(user1);
	 //               //a1.User.Add(user2);

	 //               //var a2 = access.Areas.Where(a => a.Name.Equals("East-Area")).ToList()[0];
	 //               //a2.User.Add(user1);
	 //               //a2.User.Add(user2);
	 //               area1.User.Add(user1);
	 //               area1.User.Add(user2);

	 //               area2.User.Add(user1);
	 //               area2.User.Add(user2);

	 //               AORCachedArea area3 = new AORCachedArea("Najnovija-Area", "", (List<Permission>)almostFullPerms, new List<User>() { user1, user2 }, aorGroups);
	 //               access.Areas.Add(area1);
	 //               access.Areas.Add(area2);
	 //               access.Areas.Add(area3);

					//j = access.SaveChanges();
	 //               if(j <= 0)
	 //                   throw new Exception("Failed to save new area Najnovija area changes!");

					access.Groups.AddRange(aorGroups);
					j = access.SaveChanges();
					if (j <= 0)
						throw new Exception("Failed to save aorGroups!");

					#endregion
				}
			}
		}

		public bool UserAuthentication(string username, string password)
		{
			using (var access = new AccessDB())
			{
				var myUser = access.Users.Where(u => u.Username.Equals(username)).ToList();

				//return myUser[0].Password.Equals(SecurePasswordManager.Hash(password));
				return myUser[0].Password.Equals(password);
				/* 
					 int i = access.SaveChanges();

				if (i > 0)
					return true;
				return false;*/
			}
		}
	}
}
