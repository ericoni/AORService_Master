using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Security;
using FTN.Common.AORContract;
using FTN.Common.AORCachedModel;

namespace AORManagementProxyNS
{
	/// <summary>
	/// Logovanje korisnika i AOR management prozor
	/// </summary>
	public class AORManagementChannel : ClientBase<IAORManagement>, IAORManagement
	{
		public AORManagementChannel()
			: base("AORManagement") // 8.5.19' dodat iskoristen je jedan ednpoint za login i za setovanje koji od dodjeljnih AOR-a hocu da koristim!
		{
		}

		public List<long> GetUsersSynchronousMachines()
		{
			throw new NotImplementedException(); //
		}

		public List<string> GetAORAreasForUsername(string username) // to do ubaciti ovde ili u neki zajednicki interfejs za pristupanje kesu
		{
			return this.Channel.GetAORAreasForUsername(username);
		}

		public List<AORCachedArea> Login(string username, string password)
		{
			return this.Channel.Login(username, password);
		}

		public void Test()
		{
			this.Channel.Test();
		}
	}
}
