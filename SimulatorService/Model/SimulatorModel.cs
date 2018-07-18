using Adapter;
using FTN.Common;
using FTN.Common.CE.Model;
using FTN.Common.Services;
using FTN.Common.WeatherForecast.Model;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Wires;
using ModBusSimulatorService.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModBusSimulatorService.Model
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SimulatorModel : ITwoPhaseCommit
    {
        /// <summary>
		/// List of analog points
		/// </summary>
		private List<AnalogValue> analogPoints;

        /// <summary>
        /// List of discrete points
        /// </summary>
        private List<DiscreteValue> discretPoints;

        /// <summary>
		/// Copy list of analog points
		/// </summary>
		private List<AnalogValue> analogPointsCopy;

        /// <summary>
        /// Copy list of discrete points
        /// </summary>
        private List<DiscreteValue> discretPointsCopy;

        /// <summary>
        /// Key: DER GlobalId
        /// Value: SynchonousMachine
        /// </summary>
        private Dictionary<long, SynchronousMachine> ders;

        /// <summary>
        /// Copy
        /// Key: DER GlobalId
        /// Value: SynchonousMachine
        /// </summary>
        private Dictionary<long, SynchronousMachine> dersCopy;

        /// <summary>
        /// Adapter for NMS data
        /// </summary>
        private RDAdapter adapter = new RDAdapter();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static volatile SimulatorModel instance;

        /// <summary>
        /// Thread za prikupljanje vremenske prognoze
        /// </summary>
        private CurrentWeatherJob currentWeatherJob;

        /// <summary>
        /// Lock object
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// Vremenska prognoza za svaku Sinhronu masinu, osvezavanje svaki 1h
        /// </summary>
        private Dictionary<long, WeatherInfo> currentWeathers;

        /// <summary>
        /// Objekat za zakljucavanje currentWeathers;
        /// </summary>
        private static object lock_CW_Obj = new Object();

        /// <summary>
        /// Objekat za zakljucavanje CAS;
        /// </summary>
        private static object lock_CAS_Obj = new Object();

        /// <summary>
        /// Control active signal-i
        /// </summary>
        private List<CAS> controlActiveSignals = new List<CAS>();

        /// <summary>
        /// Semafor kad dodje nova delta
        /// </summary>
        private Semaphore twoPhaseCommitSemaptore = new Semaphore(0, 1);

        private static object lock2PC = new object();


        /// <summary>
        /// Constructor
        /// </summary>
        public SimulatorModel()
        {
            this.CurrentWeathers = new Dictionary<long, WeatherInfo>();
            this.AnalogPoints = new List<AnalogValue>();
            this.AnalogPointsCopy = new List<AnalogValue>();
            this.DiscretPoints = new List<DiscreteValue>();
            this.DiscretPointsCopy = new List<DiscreteValue>();
            this.Ders = new Dictionary<long, SynchronousMachine>();
            this.DersCopy = new Dictionary<long, SynchronousMachine>();
            this.ControlActiveSignals = new List<CAS>();
        }

        /// <summary>
        /// Singleton method
        /// </summary>
        public static SimulatorModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SimulatorModel();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Get and set for AnalogPoints
        /// </summary>
        public List<AnalogValue> AnalogPoints
        {
            get
            {
                return analogPoints;
            }

            set
            {
                analogPoints = value;
            }
        }

        /// <summary>
        /// Get and set for DiscreteValue
        /// </summary>
        public List<DiscreteValue> DiscretPoints
        {
            get
            {
                return discretPoints;
            }

            set
            {
                discretPoints = value;
            }
        }

        /// <summary>
        /// Get and Set for Ders
        /// </summary>
        public Dictionary<long, SynchronousMachine> Ders
        {
            get
            {
                return ders;
            }

            set
            {
                ders = value;
            }
        }

        /// <summary>
        /// Get and Set for WeatherInfo
        /// </summary>
        public Dictionary<long, WeatherInfo> CurrentWeathers
        {
            get
            {
                lock (lock_CW_Obj)
                {
                    return currentWeathers;
                }
            }

            set
            {
                lock (lock_CW_Obj)
                {
                    currentWeathers = value;
                }
            }
        }

        public List<CAS> ControlActiveSignals
        {
            get
            {
                lock (lock_CAS_Obj)
                {
                    return controlActiveSignals;
                }
            }

            set
            {
                lock (lock_CAS_Obj)
                {
                    controlActiveSignals = value;
                }
            }
        }

        public List<AnalogValue> AnalogPointsCopy
        {
            get
            {
                return analogPointsCopy;
            }

            set
            {
                analogPointsCopy = value;
            }
        }

        public List<DiscreteValue> DiscretPointsCopy
        {
            get
            {
                return discretPointsCopy;
            }

            set
            {
                discretPointsCopy = value;
            }
        }

        public Dictionary<long, SynchronousMachine> DersCopy
        {
            get
            {
                return dersCopy;
            }

            set
            {
                dersCopy = value;
            }
        }

        public Semaphore TwoPhaseCommitSemaptore
        {
            get
            {
                return twoPhaseCommitSemaptore;
            }

            set
            {
                twoPhaseCommitSemaptore = value;
            }
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

        /// <summary>
        /// Model data initialization
        /// </summary>
        public void Initialization()
        {
            try
            {
                LoadingAnalogValues();
                LoadingDiscretValues();
                LoadingDERs(adapter.GetAllDERs());
                currentWeatherJob = new CurrentWeatherJob();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Loading analog value in analogValues
        /// </summary>
        private void LoadingAnalogValues()
        {
            AnalogPoints = adapter.GetAnalogValues();
        }

        /// <summary>
        /// Loading discret value in discretValues
        /// </summary>
        private void LoadingDiscretValues()
        {
            discretPoints = adapter.GetDiscreteValues();
        }

        /// <summary>
        /// Loading der in ders dictionary
        /// </summary>
        /// <param name="values"> List of SynchronousMachine </param>
        private void LoadingDERs(List<SynchronousMachine> values)
        {
            foreach (SynchronousMachine value in values)
            {

                if (!Ders.ContainsKey(value.GlobalId))
                {
                    Ders.Add(value.GlobalId, value);

                    continue;
                }

                Ders[value.GlobalId] = value;
            }
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        public bool Prepare(Delta delta)
        {
            lock (Lock2PC)
            {
                SetUp();
            }

            try
            {
                //Insertion of new entities
                lock (Lock2PC)
                {
                    InsertEntities(delta.InsertOperations);
                }
            }
            catch (Exception)
            {
                //throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Originals are overidden with copies
        /// </summary>
        public void Commit()
        {
            lock (Lock2PC)
            {
                analogPoints = AnalogPointsCopy;
                AnalogPointsCopy = new List<AnalogValue>();

                discretPoints = DiscretPointsCopy;
                discretPointsCopy = new List<DiscreteValue>();

                ders = dersCopy;
                dersCopy = new Dictionary<long, SynchronousMachine>();
            }

            TwoPhaseCommitSemaptore.Release();
        }

        /// <summary>
        /// Copies are reseted
        /// </summary>
        public void Rollback()
        {
            analogPointsCopy = new List<AnalogValue>();
            discretPointsCopy = new List<DiscreteValue>();
            dersCopy = new Dictionary<long, SynchronousMachine>();
        }

        /// <summary>
		/// Initialization of copies
		/// </summary>
		private void SetUp()
        {
            foreach (AnalogValue av in AnalogPoints)
            {
                AnalogPointsCopy.Add(av);
            }

            foreach (DiscreteValue dv in DiscretPoints)
            {
                DiscretPointsCopy.Add(dv);
            }

            foreach (SynchronousMachine sm in Ders.Values)
            {
                DersCopy.Add(sm.GlobalId, sm);
            }
        }

        /// <summary>
		/// Insertion of new entities using InsertEntity() method
		/// </summary>
		/// <param name="rds">List of Resource Descriptions</param>
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
            if (rd == null)
            {
                string message = String.Format("MODBUS SIMULATOR - Insert entity is not done because update operation is empty.");
                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, message);
                throw new Exception(message);
            }

            long globalId = rd.Id;
            string mrid = rd.Properties[0].PropertyValue.StringValue;
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, "MODBUS SIMULATOR - Insertion of entity with GID ({0:x16}).", globalId);

            if (EntityExists(globalId, mrid))
            {
                string message = String.Format("Failed to insert analog value because entity already exists in modbus simulator model.");
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }

            try
            {
                //Gets object type depending on its Global ID
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

                //New DMSType Object
                IdentifiedObject io = CreateEntity(globalId);

                //If object has no properties the method is finished
                if (rd.Properties == null)
                {
                    return;
                }

                foreach (Property property in rd.Properties)
                {
                    //No point in setting the GlobalID since it is already set
                    if (property.Id == ModelCode.IDOBJ_GID)
                    {
                        continue;
                    }

                    //Check if property is a Reference, set it if it is not
                    if (property.Type != PropertyType.Reference)
                    {
                        io.SetProperty(property);
                        continue;
                    }

                    //If it is, get an ID of the object which is a reference
                    long targetGlobalId = property.AsReference();

                    if (targetGlobalId != 0)
                    {

                        //if it exists
                        IdentifiedObject targetEntity = GetEntity(targetGlobalId);
                        if (targetEntity == null)
                        {
                            CommonTrace.WriteTrace(CommonTrace.TraceInfo, "MODBUS SIMULATOR - Target entity is Substation.", globalId);
                            continue;
                        }
                        targetEntity.AddReference(property.Id, io.GlobalId);
                    }

                    io.SetProperty(property);
                }

                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "MODBUS SIMULATOR - Insertion of entity with GID ({0:x16}) successfully finished.", globalId);
            }
            catch (Exception ex)
            {
                string message = String.Format("MODBUS SIMULATOR - Failed to insert entity (GID = 0x{0:x16}) into model. {1}", rd.Id, ex.Message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Creates entity for specified global ID inside the container.
        /// </summary>
        /// <param name="globalId">Global ID of the entity for insert</param>		
        /// <returns>Created entity (identified object).</returns>
        public IdentifiedObject CreateEntity(long globalId)
        {
            short type = ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

            IdentifiedObject io = null;
            switch ((DMSType)type)
            {
                case DMSType.REGION:
                    io = new GeographicalRegion(globalId);
                    break;

                case DMSType.SUBREGION:
                    io = new SubGeographicalRegion(globalId);
                    break;
                case DMSType.SUBSTATION:
                    io = new Substation(globalId);
                    break;
                case DMSType.SYNCMACHINE:
                    io = new SynchronousMachine(globalId);
                    dersCopy.Add(io.GlobalId, io as SynchronousMachine);
                    break;
                case DMSType.ANALOGVALUE:
                    io = new AnalogValue(globalId);
                    analogPointsCopy.Add(io as AnalogValue);
                    break;
                case DMSType.DISCRETEVALUE:
                    io = new DiscreteValue(globalId);
                    discretPointsCopy.Add(io as DiscreteValue);
                    break;

                default:
                    string message = String.Format("Failed to create entity because specified type ({0}) is not supported.", type);
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
            }

            return io;
        }



        /// <summary>
        /// Returns entity (identified object) on the specified index. Throws an exception if entity does not exist. 
        /// </summary>
        /// <param name="index">Index of the entity that should be returned</param>
        /// <returns>Instance of the entity in case it is found on specified position, otherwise throws exception</returns>
        public IdentifiedObject GetEntity(long globalId)
        {
            short type = ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

            switch ((DMSType)type)
            {
                case DMSType.REGION:

                    break;

                case DMSType.SUBREGION:

                    break;
                case DMSType.SUBSTATION:

                    break;
                case DMSType.SYNCMACHINE:
                    foreach (SynchronousMachine item in dersCopy.Values)
                    {
                        if (item.GlobalId != globalId)
                        {
                            continue;
                        }
                        else
                        {
                            return item;
                        }

                    }
                    break;
                case DMSType.ANALOGVALUE:
                    foreach (AnalogValue item in analogPointsCopy)
                    {
                        if (item.GlobalId != globalId)
                        {
                            continue;
                        }
                        else
                        {
                            return item;
                        }
                    }
                    break;
                case DMSType.DISCRETEVALUE:
                    foreach (DiscreteValue item in discretPointsCopy)
                    {
                        if (item.GlobalId != globalId)
                        {
                            continue;
                        }
                        else
                        {
                            return item;
                        }
                    }
                    break;

                default:
                    string message = String.Format("Failed to retrieve entity (GID = 0x{1:x16}) because entity doesn't exist.", globalId);
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
            }
            return null;

        }

        /// <summary>
        /// Checks if entity with this mrid already exists in appropriate list
        /// </summary>
        /// <param name="globalId"></param>
        /// <param name="mrid"></param>
        /// <returns></returns>
        public bool EntityExists(long globalId, string mrid)
        {
            DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

            switch (type)
            {
                case DMSType.SYNCMACHINE:
                    return DerValueExists(mrid);
                case DMSType.ANALOGVALUE:
                    return AnalogValueExists(mrid);
                case DMSType.DISCRETEVALUE:
                    return DiscreteValueExists(mrid);
                default:
                    break;
            }

            return false;
        }

        private bool AnalogValueExists(string mrid)
        {
            foreach (var item in analogPointsCopy)
            {
                if (item.Mrid.Equals(mrid))
                {
                    return true;
                }
            }

            return false;
        }
        private bool DiscreteValueExists(string mrid)
        {
            foreach (var item in discretPointsCopy)
            {
                if (item.Mrid.Equals(mrid))
                {
                    return true;
                }
            }

            return false;
        }
        private bool DerValueExists(string mrid)
        {
            foreach (var item in dersCopy.Values)
            {
                if (item.Mrid.Equals(mrid))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
