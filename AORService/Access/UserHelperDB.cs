using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.Model;
using AORCommon;
using System.Diagnostics;

namespace AORService.Access
{
	public class UserHelperDB : IUserHelperDB
	{
		private static IUserHelperDB myDB;

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
			using (var access = new AccessDB())
			{
				if (access.Users.Count() == 0)
				{
					#region perms
					Permission p1 = new Permission("DNA_AuthorityDispatcher");
					Permission p2 = new Permission("DNAAuthorityDBAdmin");
					Permission p3 = new Permission("DNA_AuthorityOperator");
					Permission p4 = new Permission("DNA_AuthorityWebManager");
					Permission p5 = new Permission("DNA_AuthorityNetworkEditor");

					//Permission p1 = new Permission(new List<string>() { "DNA_AuthorityDispatcher" });
					//Permission p2 = new Permission(new List<string>() { "DNAAuthorityDBAdmin", "DNA_AuthorityOperator" }); // vrati se ovde

					IList<Permission> perms = new List<Permission>() { p1, p2 };
					access.Perms.AddRange(perms);

					int k = access.SaveChanges();

					if (k <= 0)
						throw new Exception("Failed to save permissions.");
					#endregion

					#region DNAs
					DNAAuthority dna1 = new DNAAuthority(new List<Permission>() { p1, p2 }); //new List<Permission>() { p1 }
					DNAAuthority dna2 = new DNAAuthority(new List<Permission>() { p3, p4, p5 });

					IList<DNAAuthority> dnaAuthorities = new List<DNAAuthority>() { dna1, dna2 };
					access.DNAs.AddRange(dnaAuthorities);

					int l = access.SaveChanges();

					if (l <= 0)
						throw new Exception("Failed to save DNAs in UserHelperDB");
					#endregion

					for (int i = 1; i < 3; i++)
					{
						//access.Users.Add(new User("admin" + i.ToString(), SecurePasswordManager.Hash("admin")));
						if (i == 2)
						{
							access.Users.Add(new User("a" + i.ToString(), "a", dna1));
							continue;
						}
						access.Users.Add(new User("a" + i.ToString(), "a", 
							dna2)); // database fill
					}

					int j = access.SaveChanges();

					if (j <= 0)
						throw new Exception("Failed to save user changes!");
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
