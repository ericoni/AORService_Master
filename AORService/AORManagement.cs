using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using FTN.Common.AORContract;
using FTN.Common.Model;
using ActiveAORCache;
using AORC.Acess;
using ActiveAORCache.Helpers;

namespace AORService
{
	/// <summary>
	/// TODO: AOR login trenutno poziva aorDatabaseHelper DB, po konstruktoru
	/// </summary>
	public class AORManagement : IAORManagement
	{
		private AORDatabaseHelper aorDatabaseHelper;
		/// <summary>
		/// Singleton instance
		/// </summary>
		private static volatile AORManagement instance;

		/// <summary>
		/// Lock object
		/// </summary>
		private static object syncRoot = new Object();

		/// <summary>
		/// Singleton method
		/// </summary>
		public static AORManagement Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new AORManagement();
					}
				}

				return instance;
			}
		}

		public AORManagement()
		{
			try
			{
				aorDatabaseHelper = new AORDatabaseHelper();
				var c = AORCacheConfigurations.GetPermissionsForArea("West-Area");
			}
			catch (Exception e)
			{
				Trace.Write(e.StackTrace);
				throw e;
			}
		}

		#region IAORManagement

		public bool Login(string username, string password)
		{
			return aorDatabaseHelper.LoginUserAndSetPrincipal(username, password);
		}

		public List<string> GetAORAreasForUsername(string username)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Private methods

		private String SecureStringToString(SecureString value)
		{
			IntPtr valuePtr = IntPtr.Zero;
			try
			{
				valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
				return Marshal.PtrToStringUni(valuePtr);
			}
			catch (Exception e)
			{
				Console.Write(e.StackTrace);
				return string.Empty;
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
			}
		}

		#endregion
	}
}
