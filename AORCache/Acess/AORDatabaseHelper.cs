using System;
using System.Collections.Generic;
using System.Linq;
using Adapter;
using FTN.Common.AORCachedModel;
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.Common.AORHelpers;
using FTN.Common.AORModel;
using System.Diagnostics;
using FTN.Common.Logger;
using ActiveAORCache.Helpers;
using ActiveAORCache;
using EventCollectorProxyNS;
using System.Threading;

namespace AORC.Acess
{
	/// <summary>
	/// Implements <seealso cref="IAORDatabaseHelper"/> interface. 
	/// Class for DB population. Kobajagi vise safe varijanta, da ne ide dodjela AOR area iz static neke klase.
	/// mislim da mi ne treba singleton za instance ove klase.
	/// </summary>
	public class AORDatabaseHelper : IAORDatabaseHelper
	{
		private static IAORDatabaseHelper myDB;
		private RDAdapter rdAdapter = null;
		private AORCachedGroup aorGroup = null;
		private List<AORCachedGroup> aorCachedGroups = null;
		HashSet<AORCachedSyncMachine> aorSMachinesHash = new HashSet<AORCachedSyncMachine>();
		private List<SynchronousMachine> syncMachines = null;

		public AORDatabaseHelper()
		{
			InitializeAORCacheDB();
		}

		public List<AORCachedArea> LoginUser(string username, string password)
		{
			if (AORCacheModel.Instance.AuthenticateUser(username, password))
			{
				return AORCacheConfigurations.GetAORAreaObjectsForUsername(username);
			}
			else
			{
				return new List<AORCachedArea>();
			}
		}

		private void InitializeAORCacheDB()
		{
			using (var access = new AccessDB())
			{
				if (access.Users.Count() != 0)
				{
					string message = "Skipping user database fill (cache database is already populated).";
					Trace.Write(message);
					LogHelper.Log(LogTarget.Database, LogService.AORManagement, message);
				}
				else
				{
					rdAdapter = new RDAdapter();
					aorCachedGroups = new List<AORCachedGroup>();

					string message = "Database is empty. InitializeAORCacheDB() will proceed.";
					Trace.Write(message);
					LogHelper.Log(LogTarget.Database, LogService.AORManagement, message);

					#region Permissions
					Permission p1 = new Permission("PermissionControlSCADA", "Permission to issue commands towards SCADA system.");
					Permission p2 = new Permission("PermissionUpdateNetworkModel", "Permission to apply delta (model changes)- update current network model within their assigned AOR");
					Permission p3 = new Permission("PermissionViewSystem", "Permission to view content of AORSupervisor");
					Permission p4 = new Permission("PermissionSystemAdministration", "Permission to view system settings in AORSupervisor");
					Permission p5 = new Permission("PermissionViewSecurity", "Permission to view security content of AORSupervisor");
					Permission p6 = new Permission("PermissionSecurityAdministration", "Permission to edit security content of AORSupervisor");
					Permission p7 = new Permission("PermissionViewAdministration", "Permission to edit security content of AORSupervisor");
					Permission p8 = new Permission("PermissionViewSCADA", "Permission to view content operating under SCADA system.");

					IList<Permission> perms = new List<Permission>() { p1, p2, p3, p4, p5, p6, p7, p8 };
					access.Permissions.AddRange(perms);

					int k = access.SaveChanges();

					if (k <= 0)
						throw new Exception("Failed to save permissions.");
					#endregion

					#region DNAs
					DNAAuthority dna1 = new DNAAuthority("AuthorityDispatcher", new List<Permission>() { p1, p8, p5, p7 });
					DNAAuthority dna2 = new DNAAuthority("AuthorityNetworkEditor", new List<Permission>() { p2 });
					DNAAuthority dna3 = new DNAAuthority("SCADAAdmin", "Provides complete control to all aspects of the SCADA system.", new List<Permission>() { p1, p8 });
					DNAAuthority dna4 = new DNAAuthority("Viewer", "Required for a user to access the SCADA system.  Provides non-interactive access to data according to AOR.", new List<Permission>() { p3, p5, p7, p8 });
					DNAAuthority dna5 = new DNAAuthority("DMSAdmin", new List<Permission>() { p3, p5, p7 });
					DNAAuthority dna6 = new DNAAuthority("Operator", new List<Permission>() { p1, p2, p8 });

					access.DNAs.AddRange(new List<DNAAuthority>() { dna1, dna2, dna3, dna4, dna5, dna6 });

					int l = access.SaveChanges();

					if (l <= 0)
						throw new Exception("Failed to save DNAs in UserHelperDB");
					#endregion DNAs

					#region AOR Groups
					var nmsAorGroups = rdAdapter.GetAORGroups();

					if (nmsAorGroups == null)
					{
						Trace.Write("Warning: NMS AOR groups are null, you need to populate NMS database first.");
						return;
					}

					foreach (var nmsGroup in nmsAorGroups)
					{
						var syncMachines = rdAdapter.GetSyncMachinesForAreaGroupGid(new List<long>() { nmsGroup.GlobalId });

						var aorSyncMachines = NMSModelAORConverter.ConvertSyncMachinesFromNMS(syncMachines);

						foreach (var sm in aorSyncMachines)
						{
							aorSMachinesHash.Add(sm);
						}

						aorGroup = NMSModelAORConverter.ConvertAORGroupFromNMS(nmsGroup);
						aorGroup.SynchronousMachines = aorSyncMachines;
						aorCachedGroups.Add(aorGroup);
					}

					access.Groups.AddRange(aorCachedGroups);
					k = access.SaveChanges();

					if (k <= 0)
						throw new Exception("Failed to save AOR groups.");

					#endregion

					#region AOR Areas

					AORCachedArea area1 = new AORCachedArea("Backa-Area", "", new List<Permission> { p1, p2, p3, p4 }, new List<AORCachedGroup>(aorCachedGroups)); // aorGroup[0] gets id 7 (mozda sto prva nema u sebi SM?)
					AORCachedArea area2 = new AORCachedArea("Low-Voltage-Zrenjanin-Area", "", new List<Permission> { p1, p3, p4, p5, p8 }, new List<AORCachedGroup>() { aorCachedGroups[0], aorCachedGroups[1] });
					AORCachedArea area3 = new AORCachedArea("High-Voltage-Zrenjanin-Area", "", new List<Permission> { p2, p3, p4, p5, p8 }, new List<AORCachedGroup>() { aorCachedGroups[0], aorCachedGroups[5] });
					AORCachedArea area4 = new AORCachedArea("NoviBecej-Area", "", new List<Permission> { p1, p2, p4, p5, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1], aorCachedGroups[2], aorCachedGroups[3] });
					AORCachedArea area5 = new AORCachedArea("Low-Voltage-NoviBecej-Area", "", new List<Permission> { p5, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1], aorCachedGroups[2], aorCachedGroups[3] });
					AORCachedArea area6 = new AORCachedArea("High-Voltage-NoviBecej-Area", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1], aorCachedGroups[2], aorCachedGroups[3], aorCachedGroups[4] });
					AORCachedArea area7 = new AORCachedArea("Secanj-Area", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1], aorCachedGroups[2], aorCachedGroups[3], aorCachedGroups[4] });
					AORCachedArea area8 = new AORCachedArea("ZapadnoBackiOkrug-Area", "", new List<Permission> { p1, p2, p3, p7, p8 }, new List<AORCachedGroup>() { aorCachedGroups[0], aorCachedGroups[1], aorCachedGroups[5], aorCachedGroups[6] });
					AORCachedArea area9 = new AORCachedArea("ZapadnoBackiOkrug-Area-LowVoltage", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1], aorCachedGroups[2], aorCachedGroups[3], aorCachedGroups[4] });
					AORCachedArea area10 = new AORCachedArea("ZapadnoBackiOkrug-Area-HighVoltage", "", new List<Permission> { p1, p7, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1] });
					AORCachedArea area11 = new AORCachedArea("SremskiOkrug-Area", "", new List<Permission> { p1, p2, p3, p7, p8 }, new List<AORCachedGroup>() { aorCachedGroups[0], aorCachedGroups[1], aorCachedGroups[5], aorCachedGroups[6] });
					AORCachedArea area12 = new AORCachedArea("SremskiOkrug-Area-LowVoltage", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1], aorCachedGroups[2], aorCachedGroups[3], aorCachedGroups[4] });
					AORCachedArea area13 = new AORCachedArea("SremskiOkrug-Area-HighVoltage", "", new List<Permission> { p1, p7, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1] });
					AORCachedArea area14 = new AORCachedArea("JuznoBanatskiOkrug-Area", "", new List<Permission> { p1, p2, p3, p7, p8 }, new List<AORCachedGroup>() { aorCachedGroups[0], aorCachedGroups[1], aorCachedGroups[5], aorCachedGroups[6] });
					AORCachedArea area15 = new AORCachedArea("JuznoBanatskiOkrug-LowVoltage", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1], aorCachedGroups[2], aorCachedGroups[3], aorCachedGroups[4] });
					AORCachedArea area16 = new AORCachedArea("JuznoBanatskiOkrug-Area-HighVoltage", "", new List<Permission> { p1, p7, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1] });
					AORCachedArea area17 = new AORCachedArea("SrednjeBanatskiOkrug-Area", "", new List<Permission> { p1, p2, p3, p7, p8 }, new List<AORCachedGroup>() { aorCachedGroups[0], aorCachedGroups[1], aorCachedGroups[5], aorCachedGroups[6] });
					AORCachedArea area18 = new AORCachedArea("SrednjeBanatskiOkrug-Area-LowVoltage", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1], aorCachedGroups[2], aorCachedGroups[3], aorCachedGroups[4] });
					AORCachedArea area19 = new AORCachedArea("SrednjeBanatskiOkrug-Area-HighVoltage", "", new List<Permission> { p1, p7, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1] });
					AORCachedArea area20 = new AORCachedArea("Vojvodina-Area", "", new List<Permission> { p1, p2, p3, p4, p7, p8 }, new List<AORCachedGroup>() { aorCachedGroups[0], aorCachedGroups[1], aorCachedGroups[5], aorCachedGroups[6] });
					AORCachedArea area21 = new AORCachedArea("Vojvodina-Area-LowVoltage", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1], aorCachedGroups[2], aorCachedGroups[3], aorCachedGroups[4] });
					AORCachedArea area22 = new AORCachedArea("Vojvodina-Area-HighVoltage", "", new List<Permission> { p1, p2, p7, p8 }, new List<AORCachedGroup>() { aorCachedGroups[1] });
					#endregion

					#region Users

					AORCachedUser u1 = new AORCachedUser("marko.markovic", "a", new List<DNAAuthority>() { dna1, dna4, dna6 }, new List<AORCachedArea>() { area1, area2 });
					AORCachedUser u2 = new AORCachedUser("petar.petrovic", "a", new List<DNAAuthority>() { dna2, dna4, dna6 }, new List<AORCachedArea>() { area3 });
					AORCachedUser u3 = new AORCachedUser("zika.joksimovic", "a", new List<DNAAuthority>() { dna2, dna3, dna4, dna5, dna6 }, new List<AORCachedArea>() { area1, area2, area3, area8 });
					AORCachedUser u4 = new AORCachedUser("state", "a", new List<DNAAuthority>() { dna1, dna2, dna3, dna4, dna5, dna6 }, new List<AORCachedArea>() { area1, area2, area3, area7, area8 });
					AORCachedUser u5 = new AORCachedUser("testUsername", "a", new List<DNAAuthority>() { dna1, dna2, dna3 }, new List<AORCachedArea>() { area1, area2, area3, area7 });
					AORCachedUser u6 = new AORCachedUser("admin", "a", new List<DNAAuthority>() { dna1, dna2, dna3 }, new List<AORCachedArea>() { area9 });//ima samo jednu grupu, u kojoj je 1x SM

					//u1.DNAs = new List<DNAAuthority>() { dna3, dna4 }; //zasto je ovo ubaceno, a ne preko cst?
					//u2.DNAs = new List<DNAAuthority>() { dna4, dna5, dna6 };

					access.Users.AddRange(new List<AORCachedUser>() { u1, u2, u3, u4, u5, u6 });
					access.Areas.AddRange(new List<AORCachedArea>(11) {
						area1, area2, area3, area4, area5, area6, area7, area8, area9, area10,
						area11, area12, area13, area14, area15, area16, area17, area18, area19, area20, area21, area22 });

					k = access.SaveChanges();
					if (k <= 0)
						throw new Exception("Failed to save user and area changes!");
					#endregion

					#region UserAreasNewCombined

					var userAreaCombinedNew = new AORCachedUserAreaNew
					{
						User = u1,
						Area = area1,
						IsSelectedForControl = false
					};

					var userAreaCombinedNew2 = new AORCachedUserAreaNew
					{
						User = u2,
						Area = area1,
						IsSelectedForControl = false
					};

					var userAreaCombinedNew3 = new AORCachedUserAreaNew
					{
						User = u2,
						Area = area2,
						IsSelectedForControl = false
					};

					access.CachedUserAreasNew.AddRange(new List<AORCachedUserAreaNew>() { userAreaCombinedNew, userAreaCombinedNew2, userAreaCombinedNew3 });//, userAreaCombined2, userAreaCombined3 });
					k = access.SaveChanges();
					if (k <= 0)
						throw new Exception("Failed to save user and area combined changes!");
					#endregion UserAreasNewCombined

					#region SyncMachineGroupCombined 
					List<AORCachedSyncMachine> listAorCachedSM = new List<AORCachedSyncMachine>(aorSMachinesHash);

					// mozda izbaciti snimanje grupa u stari access.Groups
					var SyncMachineGroupCombined = new AORCachedSyncMachineGroupNew
					{
						Group = aorCachedGroups[0],
						SyncMachine = listAorCachedSM[0],
						SmGidFromNMS = listAorCachedSM[0].GidFromNms,
						AORGroupName = aorCachedGroups[0].Name
					};

					var SyncMachineGroupCombined2 = new AORCachedSyncMachineGroupNew
					{
						Group = aorCachedGroups[1],
						SyncMachine = listAorCachedSM[1],
						SmGidFromNMS = listAorCachedSM[1].GidFromNms,
						AORGroupName = aorCachedGroups[1].Name
					};

					var SyncMachineGroupCombined3 = new AORCachedSyncMachineGroupNew
					{
						Group = aorCachedGroups[2],
						SyncMachine = listAorCachedSM[2],
						SmGidFromNMS = listAorCachedSM[2].GidFromNms,
						AORGroupName = aorCachedGroups[2].Name
					};

					var SyncMachineGroupCombined4 = new AORCachedSyncMachineGroupNew
					{
						Group = aorCachedGroups[2],
						SyncMachine = listAorCachedSM[3],
						SmGidFromNMS = listAorCachedSM[3].GidFromNms,
						AORGroupName = aorCachedGroups[2].Name
					};

					var SyncMachineGroupCombined5 = new AORCachedSyncMachineGroupNew
					{
						Group = aorCachedGroups[2],
						SyncMachine = listAorCachedSM[4],
						SmGidFromNMS = listAorCachedSM[4].GidFromNms,
						AORGroupName = aorCachedGroups[2].Name
					};

					var SyncMachineGroupCombined6 = new AORCachedSyncMachineGroupNew
					{
						Group = aorCachedGroups[2],
						SyncMachine = listAorCachedSM[5],
						SmGidFromNMS = listAorCachedSM[5].GidFromNms,
						AORGroupName = aorCachedGroups[2].Name
					};

					var SyncMachineGroupCombined7 = new AORCachedSyncMachineGroupNew
					{
						Group = aorCachedGroups[3],
						SyncMachine = listAorCachedSM[6],
						SmGidFromNMS = listAorCachedSM[6].GidFromNms,
						AORGroupName = aorCachedGroups[3].Name
					};


					var SyncMachineGroupCombined8 = new AORCachedSyncMachineGroupNew
					{
						Group = aorCachedGroups[4],
						SyncMachine = listAorCachedSM[7],
						SmGidFromNMS = listAorCachedSM[7].GidFromNms,
						AORGroupName = aorCachedGroups[4].Name
					};

					access.CachedSyncMachineGroupsNew.AddRange(new List<AORCachedSyncMachineGroupNew>()
					{
						SyncMachineGroupCombined, SyncMachineGroupCombined2, SyncMachineGroupCombined3,
						SyncMachineGroupCombined4, SyncMachineGroupCombined5, SyncMachineGroupCombined6,
						SyncMachineGroupCombined7, SyncMachineGroupCombined8

					});
					k = access.SaveChanges();
					if (k <= 0)
						throw new Exception("Failed to save SM and group combined changes!");
					#endregion SyncMachineGroupCombined
				}
			}
		}
	}
}
