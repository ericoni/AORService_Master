using AORCommon.AORContract;
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

namespace AORService
{
	public class AORLogin : IAORManagement // veza DERMS app i servisa, a drugi je za vezu izmedju AORViewera i servisa
	{
		private UserHelperDB userHelper;
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

		public AORLogin()
		{
			try
			{
				userHelper = new UserHelperDB();
				

			}
			catch (Exception e)
			{
				Trace.Write(e.StackTrace);
			}
		}

		public bool Login(string username, string password)
		{
			return userHelper.UserAuthentication(username, password);
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
