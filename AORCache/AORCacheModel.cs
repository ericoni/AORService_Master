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

namespace ActiveAORCache
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class AORCacheModel : ITwoPhaseCommit
	{
		#region Fields
		private List<AORGroup> aorGroups;
		private List<AORGroup> aorGroupsCopy;
		private List<AORArea> aorAreas;
		private List<AORArea> aorAreasCopy;
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

		public List<AORArea> GetModelAORAreas()
		{
			return aorAreas;
		}

		public List<AORGroup> GetModelAORGroups()
		{
			return aorGroups;
		}

		#endregion
		public AORCacheModel()
		{
			rdAdapter = new RDAdapter();
			aorAreas = rdAdapter.GetAORAreas();
			aorGroups = rdAdapter.GetAORGroups();

			//var aggs = rdAdapter.GetAORAgAggregatorsRDs();
			//var aggs = rdAdapter.GetAORAgAggregators();
			//var g = rdAdapter.GetGroupsForAgr(42949672962);
			//var areasForAg = rdAdapter.GetAreasForAgr(42949672962);
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
				aorAreasCopy = new List<AORArea>();

				aorGroups = aorGroupsCopy;
				aorGroupsCopy = new List<AORGroup>();
			}
		}


		public void Rollback()
		{
			lock (lock2PC)
			{
				aorGroupsCopy = new List<AORGroup>();
				aorAreasCopy = new List<AORArea>();
			}
		}

		private void MakeCopy()
		{
			aorGroupsCopy = new List<AORGroup>(aorGroups.Count);
			aorAreasCopy = new List<AORArea>(aorAreas.Count);

			foreach (var group in aorGroups)
			{
				aorGroupsCopy.Add(group);
			}
			foreach (var area in aorAreas)
			{
				aorAreasCopy.Add(area);
			}
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
					AORArea newAorArea = new AORArea(rd.Id);
					newAorArea.ConvertFromRD(rd);
					aorAreasCopy.Add(newAorArea);
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
					return AORAreaExists(mrid);
				case DMSType.AOR_GROUP:
					return AORGroupExists(mrid);
				default:
					break;
			}

			return false;
		}

		private bool AORAreaExists(string mrid)
		{
			return aorAreas.Select(u => u.Mrid.Equals(mrid)).ToList().Count > 0;
		}

		private bool AORGroupExists(string mrid)
		{
			return aorGroups.Select(u => u.Mrid.Equals(mrid)).ToList().Count > 0;
		}
	}
}

