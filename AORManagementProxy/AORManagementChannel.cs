using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Security;
using FTN.Common.AORContract;
using FTN.Common.AORCachedModel;
using AORCommon.Principal;
using System.Security.Principal;

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

		public Dictionary<string, List<string>> GetAORGroupsAreasMapping(List<string> areaNames)
		{
			return this.Channel.GetAORGroupsAreasMapping(areaNames);
		}

		public List<AORCachedArea> Login(string username, string password)
		{
			return this.Channel.Login(username, password);
		}

		public void Test(IPrincipal p)
		{
			this.Channel.Test(p);
		}

		public bool SelectAreaForView(string areaName, bool isSelectedForView)
		{
			return this.Channel.SelectAreaForView(areaName, isSelectedForView);
		}

		public bool SelectAreaForControl(string areaName, bool isSelectedForControl)
		{
			return this.Channel.SelectAreaForControl(areaName, isSelectedForControl);
		}

		public List<string> GetPermissionsForUser(string username)
		{
			return this.Channel.GetPermissionsForUser(username);
		}

		public Dictionary<long, List<string>> GetAORGroupsForSyncMachines(List<long> smGids)
		{
			return this.Channel.GetAORGroupsForSyncMachines(smGids);
		}

		public List<AORCachedArea> GetAORAreaObjectsForUsername(string username)
		{
			return this.Channel.GetAORAreaObjectsForUsername(username);
		}
	}
}
