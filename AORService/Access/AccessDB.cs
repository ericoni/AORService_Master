using FTN.Common.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.AORCachedModel;

namespace AORService.Access
{
	public class AccessDB : DbContext
	{
		public AccessDB() : base("UsersDB") { }

		public DbSet<User> Users { get; set; }

		public DbSet<DNAAuthority> DNAs { get; set; }

		public DbSet<Permission> Perms { get; set; }
		public DbSet<AORCachedGroup> Groups { get; set; }
		public DbSet<AORCachedArea> Areas { get; set; }
	}
}
