using Adapter;
using FTN.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using System.Threading;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Common.SCADA;
using FTN.Common.Logger;

namespace SCADAService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class SCADAModel : ITwoPhaseCommit
	{
		#region fields
		/// <summary>
		/// Adapted for GDA queries
		/// </summary>
		private RDAdapter adapter;

		/// <summary>
		/// List of analog values, which will be populated in SCADA acquisition
		/// </summary>
		private List<AnalogValue> analogValues;

		/// <summary>
		/// List of copied values, for DT
		/// </summary>
		private List<AnalogValue> analogValuesCopy;

		/// <summary>
		/// Class used for thread synchronization
		/// </summary>
		private static ManualResetEvent mre = new ManualResetEvent(false);

		/// <summary>
		/// Singleton instance
		/// </summary>
		private static volatile SCADAModel instance;

		/// <summary>
		/// Lock object
		/// </summary>
		private static object syncRoot = new Object();

        /// <summary>
        /// Lock object for two phase commit
        /// </summary>
        private object lock2PC = new object();

		///// <summary>
		///// Lock used for acquisition and data retrieval
		///// </summary>
		//private static object obj = new object();

		#endregion fields
		public SCADAModel()
		{
			adapter = new RDAdapter();
			analogValues = adapter.GetAnalogValues();
		}

		/// <summary>
		/// Singleton method
		/// </summary>
		public static SCADAModel Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new SCADAModel();
					}
				}

				return instance;
			}
		}

        public bool Prepare(Delta delta)
        {
            LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " INFO - SCADAModel.cs - Prepare is started.");

            lock (Lock2PC)
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
                LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " ERROR - SCADAModel.cs - Prepare is finished with error: " + ex.Message);
                return false;
            }

            LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " INFO - SCADAModel.cs - Prepare is finished.");

            return true;
        }

		public void Commit()
		{
            LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " INFO - SCADAModel.cs - Commit is started.");

            lock (Lock2PC)
            {
                analogValues = analogValuesCopy;
                analogValuesCopy = new List<AnalogValue>();
            }

            LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " INFO - SCADAModel.cs - Commit is finished.");
        }

        public void Rollback()
        {
            LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " INFO - SCADAModel.cs - Roleback is started.");

            lock (Lock2PC)
            {
                analogValuesCopy = new List<AnalogValue>();
            }
            LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " INFO - SCADAModel.cs - Roleback is finished.");
        }

		private void MakeCopy()
		{
			analogValuesCopy = new List<AnalogValue>(analogValues.Count);

			foreach (AnalogValue analogValue in analogValues)
			{
				analogValuesCopy.Add(analogValue);
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
            string message = "";

            if (rd == null)
			{
				message = String.Format("Insert entity is not done because update operation is empty.");
                LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " ERROR - SCADAModel.cs - " + message);

				throw new Exception(message);
			}

			long globalId = rd.Id;
			string mrid = rd.Properties[0].PropertyValue.StringValue;

			message = String.Format("Inserting entity with GID ({0:x16}).", globalId);
            LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " INFO - SCADAModel.cs - " + message);

            if (EntityExists(globalId, mrid))
			{
				message = String.Format("Failed to insert analog value because entity already exists in network model.");
                LogHelper.Log(LogTarget.File, LogService.SCADATwoPhaseCommit, " ERROR - SCADAModel.cs - " + message);
                throw new Exception(message);
			}

			AnalogValue newAnalogV = new AnalogValue(rd.Id);
			newAnalogV.ConvertFromRD(rd);
			analogValuesCopy.Add(newAnalogV);
		}

		public bool EntityExists(long globalId, string mrid)
		{
			DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

			switch (type)
			{
				case DMSType.ANALOGVALUE:
					return AnalogValueExists(mrid);
				case DMSType.DISCRETEVALUE:
					break;
				default:
					break;
			}

			return false;
		}

		private bool AnalogValueExists(string mrid)
		{
			foreach (var item in analogValues)
			{
				if (item.Mrid.Equals(mrid))
				{
					return true;
				}
			}

			return false;
		}

		public static ManualResetEvent GetManualREvent()
		{
			return mre;
		}

		public List<AnalogValue> AnalogValues
		{
			get { return analogValues; }
			set { analogValues = value; }
		}

        public object Lock2PC
        {
            get
            {
                return lock2PC;
            }

            set
            {
                lock2PC = value;
            }
        }
    }
}
