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
		public DbSet<AORCachedSyncMachine> SynchronousMachines { get; set; }
		//public DbSet<AreasGroupsCombined> AreasGroupsCombined { get; set; } //vrati se ovde
		
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Permission>()
				.HasMany(a => a.DNAs)
				.WithMany(d => d.PermissionList)
				.Map(m =>
					{
						m.MapLeftKey("PermissionId");
						m.MapRightKey("DNAId");
						m.ToTable("DNAPermsCombined");
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

			modelBuilder.Entity<AORCachedArea>()
				.HasMany(p => p.Permissions)
				.WithMany(a => a.Areas)
				.Map(k =>
				{
					k.MapLeftKey("AreaId");
					k.MapRightKey("PermissionId");
					k.ToTable("AreasPermissionsCombined");
				});

			modelBuilder.Entity<AORCachedArea>()
				.HasMany(u => u.Users)
				.WithMany(a => a.Areas)
				.Map(k =>
				{
					k.MapLeftKey("AreaId");
					k.MapRightKey("UserId");
					k.ToTable("AreasUsersCombined");
				});

			modelBuilder.Entity<User>()
				.HasMany(u => u.DNAs)
				.WithMany(a => a.Users)
				.Map(k =>
				{
					k.MapLeftKey("UserId");
					k.MapRightKey("DNAId");
					k.ToTable("UsersDNACombined");
				});

			base.OnModelCreating(modelBuilder);
		}
	}
}
