using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORC.Acess
{
	/// <summary>
	/// Ovo se automatski poziva kada se odradi novo instanciranje accessDb-a.
	/// </summary>
	class Configuration : DbMigrationsConfiguration<AccessDB>
	{
		public Configuration() 
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
			ContextKey = "UsersDatabase6";
			AppDomain.CurrentDomain.SetData("DataDirectory", "D:\\mrtvaBaza");
			//System.IO.Directory.GetCurrentDirectory())
			//
		}
	}
}
