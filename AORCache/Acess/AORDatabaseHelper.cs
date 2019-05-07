using System;
using System.Collections.Generic;
using System.Linq;
using FTN.Common.Model;
using Adapter;
using FTN.Common.AORCachedModel;
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.Common.AORHelpers;
using FTN.Common.AORModel;
using System.Diagnostics;

namespace AORC.Acess
{
	/// <summary>
	/// Made for DB population.
	/// </summary>
	public class AORDatabaseHelper : IAORDatabaseHelper
	{
		private static IAORDatabaseHelper myDB;
		private RDAdapter rdAdapter = null;
		private AORCachedGroup aorGroup = null;
		private List<AORCachedGroup> aorGroups = null;
		private List<SynchronousMachine> syncMachines = null;

		public static IAORDatabaseHelper Instance
		{
			get
			{
				if (myDB == null)
					myDB = new AORDatabaseHelper();

				return myDB;
			}
			set
			{
				if (myDB == null)
					myDB = value;
			}
		}

		public AORDatabaseHelper()
		{
			InitializeAORCacheDB();
		}

		public bool LoginUserAndSetPrincipal(string username, string password)
		{
			using (var access = new AccessDB())
			{
				var myUser = access.Users.Where(u => u.Username.Equals(username)).ToList();

				//return myUser[0].Password.Equals(SecurePasswordManager.Hash(password));
				return myUser[0].Password.Equals(password);
				/* 
					 int i = access.SaveChanges();

				if (i > 0)
					return true;
				return false;*/
			}
		}

		private void InitializeAORCacheDB()
		{
			rdAdapter = new RDAdapter();
			aorGroups = new List<AORCachedGroup>();

			using (var access = new AccessDB())
			{
				if (access.Users.Count() != 0)
				{
					Trace.Write("Skipping db fill (db is populated.");
					return;
				}

				#region Permissions
				Permission p1 = new Permission("DNA_PermissionControlSCADA", "Permission to issue commands towards SCADA system.");
				Permission p2 = new Permission("DNA_PermissionUpdateNetworkModel", "Permission to apply delta (model changes)- update current network model within their assigned AOR");
				Permission p3 = new Permission("DNA_PermissionViewSystem", "Permission to view content of AORViewer");
				Permission p4 = new Permission("DNA_PermissionSystemAdministration", "Permission to view system settings in AORViewer");
				Permission p5 = new Permission("DNA_PermissionViewSecurity", "Permission to view security content of AORViewer");
				Permission p6 = new Permission("DNA_PermissionSecurityAdministration", "Permission to edit security content of AORViewer");
				Permission p7 = new Permission("DNA_PermissionViewAdministration", "Permission to edit security content of AORViewer");
				Permission p8 = new Permission("DNA_PermissionViewSCADA", "Permission to view content operating under SCADA system.");

				IList<Permission> perms = new List<Permission>() { p1, p2, p3, p4, p5, p6, p7, p8 };
				access.Permissions.AddRange(perms);

				int k = access.SaveChanges();

				if (k <= 0)
					throw new Exception("Failed to save permissions.");
				#endregion

				#region DNAs
				DNAAuthority dna1 = new DNAAuthority("DNA_AuthorityDispatcher", new List<Permission>() { p1, p8, p5, p7 });
				DNAAuthority dna2 = new DNAAuthority("DNA_AuthorityNetworkEditor", new List<Permission>() { p2 });
				DNAAuthority dna3 = new DNAAuthority("DNA_SCADAAdmin", "Provides complete control to all aspects of the SCADA system.", new List<Permission>() { p1, p8 });
				DNAAuthority dna4 = new DNAAuthority("DNA_Viewer", "Required for a user to access the SCADA system.  Provides non-interactive access to data according to AOR.", new List<Permission>() { p3, p5, p7, p8 });
				DNAAuthority dna5 = new DNAAuthority("DNA_DMSAdmin", new List<Permission>() { p3, p5, p7 });
				DNAAuthority dna6 = new DNAAuthority("DNA_Operator", new List<Permission>() { p1, p2, p8 });

				access.DNAs.AddRange(new List<DNAAuthority>() { dna1, dna2, dna3, dna4, dna5, dna6 });

				int l = access.SaveChanges();

				if (l <= 0)
					throw new Exception("Failed to save DNAs in UserHelperDB");
				#endregion DNAs

				#region AOR Groups
				var nmsAorGroups = rdAdapter.GetAORGroups();

				if (nmsAorGroups == null)
				{
					Trace.Write("NMS AOR groups are null, you need to populate NMS database first.");
					return;
				}

				foreach (var nmsGroup in nmsAorGroups)
				{
					syncMachines = rdAdapter.GetSyncMachinesForAreaGroupGid(new List<long>() { nmsGroup.GlobalId });

					var aorSyncMachines = NMSModelAORConverter.ConvertSyncMachinesFromNMS(syncMachines);

					aorGroup = NMSModelAORConverter.ConvertAORGroupFromNMS(nmsGroup);
					aorGroup.SynchronousMachines = aorSyncMachines;
					aorGroups.Add(aorGroup);
				}

				access.Groups.AddRange(aorGroups);
				k = access.SaveChanges();

				if (k <= 0)
					throw new Exception("Failed to save AOR groups.");

				#endregion

				#region AOR Areas

				AORCachedArea area1 = new AORCachedArea("West-Area", "", new List<Permission> { p1, p2, p3, p4 }, new List<AORCachedGroup>(aorGroups)); // for some reason, aorGroup[0] get's id 7 (mozda zato sto prva nema u sebi masinu?)
				AORCachedArea area2 = new AORCachedArea("East-Area", "", new List<Permission> { p1, p3, p4, p5, p8 }, new List<AORCachedGroup>() { aorGroups[0], aorGroups[1] });
				AORCachedArea area3 = new AORCachedArea("South-Area", "", new List<Permission> { p2, p3, p4, p5, p8 }, new List<AORCachedGroup>() { aorGroups[0], aorGroups[5] });
				AORCachedArea area4 = new AORCachedArea("North-Area", "", new List<Permission> { p1, p2, p4, p5, p8 }, new List<AORCachedGroup>() { aorGroups[1], aorGroups[2], aorGroups[3] });
				AORCachedArea area5 = new AORCachedArea("North-Area2", "", new List<Permission> { p5, p8 }, new List<AORCachedGroup>() { aorGroups[1], aorGroups[2], aorGroups[3] });
				AORCachedArea area6 = new AORCachedArea("North-Area-HighVoltage", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorGroups[1], aorGroups[2], aorGroups[3], aorGroups[4] });
				AORCachedArea area7 = new AORCachedArea("East-Area-Wind", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorGroups[1], aorGroups[2], aorGroups[3], aorGroups[4] });
				AORCachedArea area8 = new AORCachedArea("East-Area-LowVoltage", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorGroups[1], aorGroups[2], aorGroups[3], aorGroups[4] });
				AORCachedArea area9 = new AORCachedArea("East-Area-HighVoltage", "", new List<Permission> { p1, p7, p8 }, new List<AORCachedGroup>() { aorGroups[0] });
				AORCachedArea area10 = new AORCachedArea("Central-Area-HighVoltage", "", new List<Permission> { p1, p2, p3, p7, p8 }, new List<AORCachedGroup>() { aorGroups[0], aorGroups[1], aorGroups[5], aorGroups[6] });
				AORCachedArea area11 = new AORCachedArea("Central-Area-LowVoltage", "", new List<Permission> { p1, p2, p5, p7 }, new List<AORCachedGroup>() { aorGroups[0], aorGroups[1], aorGroups[4], aorGroups[6] });

				#endregion

				#region Users

				User u1 = new User("marko.markovic", "a", new List<DNAAuthority>() { dna1, dna4, dna6 }, new List<AORCachedArea>() { area1, area2 });
				User u2 = new User("petar.petrovic", "a", new List<DNAAuthority>() { dna2, dna4, dna6 }, new List<AORCachedArea>() { area3 });
				User u3 = new User("zika.joksimovic", "a", new List<DNAAuthority>() { dna2, dna3, dna4, dna5, dna6 }, new List<AORCachedArea>() { area1, area2, area3, area8 });

				u1.DNAs = new List<DNAAuthority>() { dna3, dna4 };
				u2.DNAs = new List<DNAAuthority>() { dna4, dna5, dna6 };

				access.Users.AddRange(new List<User>() { u1, u2, u3 });
				access.Areas.AddRange(new List<AORCachedArea>(11) { area1, area2, area3, area4, area5, area6, area7, area8, area9, area10, area11 });

				k = access.SaveChanges();
				if (k <= 0)
					throw new Exception("Failed to save user and area changes!");

				#endregion
			}
		}
	}
}
