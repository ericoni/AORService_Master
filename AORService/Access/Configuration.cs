using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORService.Access
{
	class Configuration : DbMigrationsConfiguration<AccessDB>
	{
		public Configuration() 
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
			ContextKey = "UsersDB";
			AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
		}
	}
}
