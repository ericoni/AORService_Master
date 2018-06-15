using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSDB.Model;

namespace TSDB.Access
{
    public class AccessDB : DbContext
    {
        public AccessDB() : base("historianDB") { }

        public DbSet<CollectItem> CollectItems { get; set; }
		public DbSet<FiveMinutesItem> FiveMinutesItems { get; set; }
		public DbSet<HourlyItem> HourlyItems { get; set; }
		public DbSet<DailyItem> DailyItems { get; set; }
		public DbSet<MonthlyItem> MonthlyItems { get; set; }
		public DbSet<AnnualItem> AnnualItems { get; set; }
    }
}
