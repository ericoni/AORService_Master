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
using System.ServiceModel;

namespace AORService
{
	/// <summary>
	/// TODO: AOR login trenutno poziva aorDatabaseHelper DB, po konstruktoru, ovaj bi trebao da handle logovanje-ako se dobro sjecam
	/// </summary>
	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class AORManagement : IAORManagement
	{
		private AORDatabaseHelper aorDatabaseHelper = null;

		public AORManagement()
		{
			try
			{
				aorDatabaseHelper = new AORDatabaseHelper();
                //var c = AORCacheConfigurations.GetPermissionsForArea("West-Area");
                //var d = AORCacheConfigurations.GetPermissionsForAreas(new List<string>() { "West-Area", "East-Area" });
                var f = AORCacheConfigurations.GetPermissionsForUser("state");
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
			return aorDatabaseHelper.LoginUser(username, password);
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
