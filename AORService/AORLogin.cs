using AORCommon.AORManagementContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORService
{
	public class AORLogin : IAORManagement
	{
		/// <summary>
		/// Singleton instance
		/// </summary>
		private static volatile AORLogin instance;

		/// <summary>
		/// Lock object
		/// </summary>
		private static object syncRoot = new Object();

		/// <summary>
		/// Singleton method
		/// </summary>
		public static AORLogin Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new AORLogin();
					}
				}

				return instance;
			}
		}

		public bool Login(string username, string password)
		{
			// TO DO
			int b = 0;
			b++;
			return false;
		}
	}
}
