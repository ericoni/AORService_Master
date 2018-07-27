﻿using FTN.Common.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORService.Access
{
	public class AccessDB : DbContext
	{
		public AccessDB() : base("UsersDB") { }

		public DbSet<User> Users { get; set; }
	}
}
