using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORC.Acess
{
	/// <summary>
	/// Ovo se automatski poziva kada se odradi novo instanciranje accessDb-a. Ovo bi trebala biti samo configuration za cache.
	/// </summary>
	class Configuration : DbMigrationsConfiguration<AccessDB>
	{
		public Configuration() 
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
			ContextKey = "UsersDatabase7";
			AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
			//System.IO.Directory.GetCurrentDirectory())
			// "D:\\mrtvaBaza" "C:\\Users\\admin\\Source\\Repos\\AORService_Master\\AORService\\bin\\x64\\Debug" -kada se stavi full path, pise da je db u upotrebi
		}
	}
}
