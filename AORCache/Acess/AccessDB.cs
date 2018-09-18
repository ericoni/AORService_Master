using FTN.Common.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.AORCachedModel;
using FTN.Services.NetworkModelService.DataModel.Wires;

namespace AORC.Acess
{
	public class AccessDB : DbContext
	{
		public AccessDB() : base("UsersDB") { }

		public DbSet<User> Users { get; set; }

		public DbSet<DNAAuthority> DNAs { get; set; }

		public DbSet<Permission> Permissions { get; set; }
		public DbSet<AORCachedGroup> Groups { get; set; }
		public DbSet<AORCachedArea> Areas { get; set; }
		public DbSet<SynchronousMachine> SyncMachines { get; set; }
		
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Permission>()
				.HasMany(a => a.DNAs)
				.WithMany(d => d.PermissionList)
				.Map(m =>
					{
						m.MapLeftKey("PermissionId");
						m.MapRightKey("DNAId");
						m.ToTable("DNAPerms");
					});

			modelBuilder.Entity<AORCachedArea>()
				.HasMany(g => g.Groups)
				.WithMany(a => a.Areas)
				.Map(k => 
					{
						k.MapLeftKey("AreaId");
						k.MapRightKey("GroupId");
						k.ToTable("AreasGroupsCombined");
					});

			base.OnModelCreating(modelBuilder);
		}
	}
}
