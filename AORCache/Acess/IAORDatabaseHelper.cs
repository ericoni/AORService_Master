﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORC.Acess
{
	public interface IAORDatabaseHelper
	{
	 bool LoginUserAndSetPrincipal(string username, string password);
	}
}