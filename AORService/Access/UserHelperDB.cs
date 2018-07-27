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

		public UserHelperDB() // static const. smeta
		{
			using (var access = new AccessDB())
			{
				if (access.Users.Count() == 0)
				{
					for (int i = 1; i < 3; i++)
					{
						//access.Users.Add(new User("admin" + i.ToString(), SecurePasswordManager.Hash("admin")));
						access.Users.Add(new User("admin" + i.ToString(), "admin"));
					}

					int j = access.SaveChanges();

					if (j <= 0)
						throw new Exception("Failed to save changes!");
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
