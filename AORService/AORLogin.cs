using AORCommon.AORManagementContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

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

		public bool Login(string username, SecureString password)
		{
			// TO DO
			string a = SecureStringToString(password);
			int b = 0;
			b++;
			return false;
		}

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
	}
}
