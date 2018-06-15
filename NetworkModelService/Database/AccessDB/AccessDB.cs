using FTN.Common;
using FTN.Services.NetworkModelService.Database.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.Database.Access
{
	public class AccessDB : DbContext
	{
		public AccessDB() : base("NmsDB") { }

		public DbSet<DeltaItem> Deltas { get; set; }

		public DbSet<ResourceDescription> RDescriptions { get; set; }

		public DbSet<Property> Properties { get; set; }

	}
}
