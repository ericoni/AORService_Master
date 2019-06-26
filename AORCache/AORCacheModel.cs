using FTN.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Common.AORModel;
using FTN.Common.Logger;
using Adapter;
using AORC.Acess;
using FTN.Common.Model;
using FTN.Common.AORCachedModel;

namespace ActiveAORCache
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class AORCacheModel : ITwoPhaseCommit
	{
		#region Fields
		private List<AORGroup> aorGroups;
		private List<AORGroup> aorGroupsCopy;
		private List<AORCachedArea> aorAreas;
		private List<AORCachedArea> aorAreasCopy;
		List<AORCachedGroup> aorGroupsModel;
		private RDAdapter rdAdapter; 
		/// <summary>
		/// Singleton instance
		/// </summary>
		private static volatile AORCacheModel instance;

		/// <summary>
		/// Lock object
		/// </summary>
		private static object syncRoot = new Object();

		/// <summary>
		/// Lock object for two phase commit
		/// </summary>
		private object lock2PC = new object();

		public List<AORCachedGroup> GetModelAORGroups()
		{
			return aorGroupsModel;
		}

		public List<AORCachedArea> GetNewAORAreas() // novo dodata metoda 
		{
			return aorAreas;
		}

		#endregion
		public AORCacheModel()
		{
			//InitializeAORCacheWithAORData(); //TODO srediti inicijalizaciju
		}

		private void InitializeAORCacheWithAORData()
		{
			aorGroupsModel = GetModelAORGroup();
		}

		/// <summary>
		/// Singleton method
		/// </summary>
		public static AORCacheModel Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new AORCacheModel();
					}
				}

				return instance;
			}
		}

		#region ITwoPhaseCommit
		public bool Prepare(Delta delta)
		{
			lock (lock2PC)
			{
				MakeCopy();
			}

			try
			{
				lock (lock2PC)
				{
					InsertEntities(delta.InsertOperations);
				}
			}
			catch (Exception ex)
			{
				LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " ERROR - AORCacheModel.cs - Prepare is finished with error: " + ex.Message);
				return false;
			}

			return true;
		}

		public void Commit()
		{
			lock (lock2PC)
			{
				aorAreas = aorAreasCopy;
				aorAreasCopy = new List<AORCachedArea>();

				aorGroups = aorGroupsCopy;
				aorGroupsCopy = new List<AORGroup>();
			}
		}


		public void Rollback()
		{
			lock (lock2PC)
			{
				aorGroupsCopy = new List<AORGroup>();
				aorAreasCopy = new List<AORCachedArea>();
			}
		}
		#endregion

		#region Distributed Transaction

		private void MakeCopy()
		{
			aorGroupsCopy = new List<AORGroup>(aorGroups.Count);
			//aorAreasCopy = new List<AORCachedArea>(aorAreas.Count);

			foreach (var group in aorGroups)
			{
				aorGroupsCopy.Add(group);
			}
			//foreach (var area in aorAreas)
			//{
			//	aorAreasCopy.Add(area);
			//}
		}

		private void InsertEntities(List<ResourceDescription> rds)
		{
			foreach (ResourceDescription rd in rds)
			{
				try
				{
					InsertEntity(rd);
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}

		private void InsertEntity(ResourceDescription rd)
		{
			string message = string.Empty;

			if (rd == null)
			{
				LogHelper.Log(LogTarget.File, LogService.AORCache2PC, " ERROR - 2PC AORCacheModel.cs - ");
				throw new ArgumentNullException();
			}

			long globalId = rd.Id;
			string mrid = rd.Properties[0].PropertyValue.StringValue;

			if (EntityExists(globalId, mrid))
			{
				message = String.Format("Failed to insert value because entity already exists in network model.");
				LogHelper.Log(LogTarget.File, LogService.AORCache2PC, " ERROR - AORCacheModel.cs - " + message);
				throw new Exception(message);
			}

			DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

			switch (type)
			{
				case DMSType.AOR_AREA:
					//AORCachedArea newAorArea = new AORCachedArea(rd.Id);
					//newAorArea.ConvertFromRD(rd);
					//aorAreasCopy.Add(newAorArea); // vrati se
					break;
				case DMSType.AOR_GROUP:
					AORGroup newAorGroup = new AORGroup(rd.Id);
					newAorGroup.ConvertFromRD(rd);
					aorGroupsCopy.Add(newAorGroup);
					break;
				case DMSType.AOR_USER:
					break;
				case DMSType.AOR_AGAGGREGATOR:
					break;
				default:
					break;
			}
		}

		public bool EntityExists(long globalId, string mrid)
		{
			DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

			switch (type)
			{
				case DMSType.AOR_AREA:
					break;
				case DMSType.AOR_GROUP:
					return AORGroupExists(mrid);
				default:
					break;
			}

			return false;
		}

		private bool AORGroupExists(string mrid)
		{
			return aorGroups.Select(u => u.Mrid.Equals(mrid)).ToList().Count > 0;
		}


		#endregion Distributed Transaction

		//todo pobrisati ove stare metode sve, i ocisitti model

		public List<Permission> GetPermissionsForDNAs(long areaId)  //vrati se ovde jer vadim perms za DNA, iako pise par. areaID
		{
			using (var access = new AccessDB())
			{
				var query = access.Permissions.Where(d => d.DNAs.Any(a => a.DNAId == areaId));
				var result = query.ToList();

				return result;
			}
		}

		public List<AORCachedGroup> GetAORGroupsForArea(int areaId)
		{
			using (var access = new AccessDB())
			{
				var query = access.Groups.Where(g => g.Areas.Any(a => a.AreaId == areaId));
				var result = query.ToList();

				return result;
			}
		}

		public List<AORCachedGroup> GetModelAORGroup()
		{
			using (var access = new AccessDB())
			{
				var query = (from a in access.Groups // ne radi Include do SMs
							 select a);

				var result = query.ToList();
				return result;
			}
		}

		public List<User> GetAllUsers()
		{
			using (var access = new AccessDB())
			{
				var query = (from a in access.Users // vrati se za vezu area -> user
							 select a);
				var c = query.ToList();

				return c;
			}
		}

		public List<string> GetUsernamesForDNA(string name)
		{
			using (var access = new AccessDB())
			{
				var dna = access.DNAs.Where(u => u.Name.Equals(name)).FirstOrDefault();

				var query = access.Users.Where(d => d.DNAs.Any(a => a.DNAId == dna.DNAId));
				var result = query.ToList();

				List<string> returnList = new List<string>(result.Count);

				foreach (var r in result)
				{
					returnList.Add(r.Username);
				}

				return returnList;
			}
		}

		public List<string> GetUsernamesForArea(string name)
		{
			using (var access = new AccessDB())
			{
				var area = access.Areas.Where(u => u.Name.Equals(name)).FirstOrDefault();

				var query = access.Users.Where(d => d.Areas.Any(a => a.AreaId == area.AreaId));
				var result = query.ToList();

				List<string> returnList = new List<string>(result.Count);

				foreach (var r in result)
				{
					returnList.Add(r.Username);
				}

				return returnList;
			}
		}

		public List<string> GetPermissionsForArea(string name)
		{
			using (var access = new AccessDB())
			{
				var area = access.Areas.Where(u => u.Name.Equals(name)).FirstOrDefault();

				var query = access.Permissions.Where(d => d.Areas.Any(a => a.AreaId == area.AreaId));
				var result = query.ToList();

				List<string> returnList = new List<string>(result.Count);

				foreach (var r in result)
				{
					returnList.Add(r.Name);
				}

				return returnList;
			}
		}

		public List<string> GetGroupsForArea(string name)
		{
			using (var access = new AccessDB())
			{
				var area = access.Areas.Where(u => u.Name.Equals(name)).FirstOrDefault();

				var query = access.Groups.Where(d => d.Areas.Any(a => a.AreaId == area.AreaId));
				var result = query.ToList();

				List<string> returnList = new List<string>(result.Count);

				foreach (var r in result)
				{
					returnList.Add(r.Name);
				}

				return returnList;
			}
		}

		public HashSet<AORCachedArea> GetAORAreas()
		{
			using (var access = new AccessDB())
			{
				var areas = access.Areas.ToArray();
				return new HashSet<AORCachedArea>(areas);
			}
		}

		/// <summary>
		/// Authenticate user by checking username and password. Password hashing yet to be implemented.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public bool AuthenticateUser(string username, string password)
		{
			User user = null;

			using (var access = new AccessDB())
			{
				user = access.Users.Where(u => u.Username.Equals(username)).FirstOrDefault();
			}

			if (user == null)
			{
				return false;
			}
			else
			{
				return user.Password.Equals(password);
			}
			/* 
				//return myUser[0].Password.Equals(SecurePasswordManager.Hash(password));
				 * 
					 int i = access.SaveChanges();

				if (i > 0)
					return true;
				return false;*/
		}
	}
}

