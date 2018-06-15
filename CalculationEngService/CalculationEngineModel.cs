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
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Common.CalculationEngine.Model;
using FTN.Common.CE.Model;
using CommonCE;
using FTN.Common.SCADA;
using CalculationEngine;
using FTN.Common.Logger;

namespace CalculationEngService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CalculationEngineModel : ITwoPhaseCommit
    {
        /// <summary>
        /// Lock object
        /// </summary>
        private static readonly object syncObject = new object();

        /// <summary>
        /// List of original analog points
        /// </summary>
        private List<AnalogValue> analogPointsOriginal;

        /// <summary>
        /// List of copy analog points
        /// </summary>
        private List<AnalogValue> analogPointsCopy;

        /// <summary>
        /// List of original discrete points
        /// </summary>
        private List<DiscreteValue> discretePointsOriginal;

        /// <summary>
        /// List of copy discrete points
        /// </summary>
        private List<DiscreteValue> discretePointsCopy;

        /// <summary>
        /// Key: DER GlobalId
        /// Value: SynchonousMachine
        /// </summary>
        private List<SynchronousMachine> dersOriginal;

        /// <summary>
        /// Key: DER GlobalId
        /// Value: SynchonousMachine
        /// </summary>
        private List<SynchronousMachine> dersCopy;

        /// <summary>
        /// Adapter for NMS data
        /// </summary>
        private RDAdapter adapter = new RDAdapter();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static volatile CalculationEngineModel instance;

        /// <summary>
        /// Lock object
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// List that captures all forecast objects for ders, containing der.GlobalID, dailyP, dailyQ, hourlyP, hourlyQ
        /// </summary>
        private List<ForecastObject> forecastListOfDers = null;

        /// <summary>
        /// List of appliedCommands
        /// </summary>
        private Dictionary<long, List<Command>> appliedCommands = new Dictionary<long, List<Command>>();

        /// <summary>
        /// Lock object for appliedCommands
        /// </summary>
        private static object lockAC = new object();

        /// <summary>
        /// Lock object for two phase commit
        /// </summary>
        private object lock2PC = new object();

        /// <summary>
        /// Semafor kad dodje nova delta
        /// </summary>
        private Semaphore twoPhaseCommitSemaptore = new Semaphore(0, 1);

        /// <summary>
        /// Constructor
        /// </summary>
        public CalculationEngineModel()
        {
            this.AnalogPointsOriginal = new List<AnalogValue>();
            this.DiscretePointsOriginal = new List<DiscreteValue>();
            this.DersOriginal = new List<SynchronousMachine>();

            this.AnalogPointsCopy = new List<AnalogValue>();
            this.DiscretePointsCopy = new List<DiscreteValue>();
            this.DersCopy = new List<SynchronousMachine>();

            this.ForecastListOfDers = new List<ForecastObject>();
        }

        /// <summary>
        /// Singleton method
        /// </summary>
        public static CalculationEngineModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CalculationEngineModel();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Get and set for ForecastListOfDers
        /// </summary>
        public List<ForecastObject> ForecastListOfDers
        {
            get
            {
                return forecastListOfDers;
            }

            set
            {
                lock (syncObject)
                {
                    forecastListOfDers = value;
                }

            }
        }

        /// <summary>
        /// Get and set for AnalogPointsOriginal
        /// </summary>
        public List<AnalogValue> AnalogPointsOriginal
        {
            get
            {
                return analogPointsOriginal;
            }

            set
            {
                analogPointsOriginal = value;
            }
        }

        /// <summary>
        /// Get and set for AnalogPointsCopy
        /// </summary>
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

        /// <summary>
        /// Get and set for DiscreteValueOriginal
        /// </summary>
        public List<DiscreteValue> DiscretePointsOriginal
        {
            get
            {
                return discretePointsOriginal;
            }

            set
            {
                discretePointsOriginal = value;
            }
        }

        /// <summary>
        /// Get and set for DiscreteValueCopy
        /// </summary>
        public List<DiscreteValue> DiscretePointsCopy
        {
            get
            {
                return discretePointsCopy;
            }

            set
            {
                discretePointsCopy = value;
            }
        }

        /// <summary>
        /// Get and Set for DersOriginal
        /// </summary>
        public List<SynchronousMachine> DersOriginal
        {
            get
            {
                return dersOriginal;
            }

            set
            {
                dersOriginal = value;
            }
        }

        /// <summary>
        /// Get and Set for DersCopy
        /// </summary>
        public List<SynchronousMachine> DersCopy
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

        #region AppliedCommands
        public Dictionary<long, List<Command>> AppliedCommands
        {
            get
            {
                lock (lockAC)
                {
                    return appliedCommands;
                }
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
    
        public void AddCommand(List<Command> commands)
        {
            lock (lockAC)
            {
                foreach (Command command in commands)
                {
                    if (!appliedCommands.ContainsKey(command.GlobalId))
                    {
                        appliedCommands.Add(command.GlobalId, new List<Command>());
                    }

                    appliedCommands[command.GlobalId].Add(command);
                }
            }
        }

        public void RemoveCommand(Command removeCommand)
        {
            lock (lockAC)
            {
                List<CAS> casSignals = new List<CAS>();

                if (!appliedCommands.ContainsKey(removeCommand.GlobalId))
                {
                    return;
                }

                appliedCommands[removeCommand.GlobalId].Remove(removeCommand);

                casSignals.Add(new CAS(removeCommand.GlobalId, CASEnum.FailSafe));
			    CASSubscriber.Instance.NotifySubscribers(casSignals);
            }
        }

        #endregion AppliedCommands

        /// <summary>
        /// Model data initialization
        /// </summary>
        public void Initialization()
        {
            try
            {
                LoadingAnalogValues();
                LoadingDiscreteValues();
                LoadingDERs(adapter.GetAllDERs());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Loading analog value in analogValuesOriginal
        /// </summary>
        private void LoadingAnalogValues()
        {
            analogPointsOriginal = adapter.GetAnalogValues();
        }
        /// <summary>
        /// Loading dicrete value in discreteValuesOriginal
        /// </summary>
        private void LoadingDiscreteValues()
        {
            discretePointsOriginal = adapter.GetDiscreteValues();
        }


        /// <summary>
        /// Loading der in ders dictionary
        /// </summary>
        /// <param name="values"> List of SynchronousMachine </param>
        private void LoadingDERs(List<SynchronousMachine> values)
        {
            foreach (SynchronousMachine value in values)
            {

                if (!dersOriginal.Contains(value))
                {
                    dersOriginal.Add(value);

                    continue;
                }

                dersOriginal.Add(value);
            }
        }

        public void Commit()
        {
            LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " Info - CalculationEngineModel.cs - Commit is started.");

            lock (CalculationEngineModel.Instance.Lock2PC)
            {
                analogPointsOriginal = analogPointsCopy;
                analogPointsCopy = new List<AnalogValue>();

                discretePointsOriginal = discretePointsCopy;
                discretePointsCopy = new List<DiscreteValue>();

                dersOriginal = dersCopy;
                dersCopy = new List<SynchronousMachine>();
            }

            LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " Info - CalculationEngineModel.cs - Commit is finished.");

            TwoPhaseCommitSemaptore.Release();
        }

        public bool Prepare(Delta delta)
        {
            //Inicijalizacija kopije tako da pokazuje na iste kontejnere kao sto pokazuje i original
            LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " INFO - CalculationEngineModel.cs - Prepare is started.");

            lock (CalculationEngineModel.Instance.Lock2PC)
            {
                SetUp();
            }

            try
            {
                lock (CalculationEngineModel.Instance.Lock2PC)
                {
                    InsertEntities(delta.InsertOperations);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " Error - CalculationEngineModel.cs - Prepare is finished with error: " + ex.Message);

                return false;
            }

            LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " INFO - CalculationEngineModel.cs - Prepare is finished.");

            return true;
        }

        public void Rollback()
        {
            LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " INFO - CalculationEngineModel.cs - Rollback is started.");

            lock (CalculationEngineModel.Instance.Lock2PC)
            {
                analogPointsCopy = new List<AnalogValue>();
                discretePointsCopy = new List<DiscreteValue>();
                dersCopy = new List<SynchronousMachine>();
            }
            LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " INFO - CalculationEngineModel.cs - Rollback is finished.");
        }

        /// <summary>
        /// Radi inicijalizaciju kopije
        /// </summary>
        private void SetUp()
        {
            foreach (AnalogValue av in AnalogPointsOriginal)
            {
                AnalogPointsCopy.Add(av);
            }

            foreach (DiscreteValue dv in DiscretePointsOriginal)
            {
                DiscretePointsCopy.Add(dv);
            }

            foreach (SynchronousMachine sm in DersOriginal)
            {
                DersCopy.Add(sm);
            }
        }

        /// <summary>
        /// Ubacuje nove entitete tako sto poziva metodu InsertEntity()
        /// </summary>
        /// <param name="rds">Lista Resource Description-a koje ubacujemo</param>
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
                string message = String.Format("Insert entity is not done because update operation is empty.");
                LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " ERROR - CalculationEngineModel.cs - " + message);

                throw new Exception(message);
            }

            long globalId = rd.Id;

            string mrid = rd.Properties[0].PropertyValue.StringValue;
            LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " INFO - CalculationEngineModel.cs - Insert entity with {" + mrid + "} mrid.");

            if (EntityExists(globalId, mrid))
            {
                string message = String.Format("Failed to insert analog value because entity already exists in calculation engine model.");
                LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " ERROR - CalculationEngineModel.cs - " + message);
                throw new Exception(message);
            }

            try
            {
                //Dobijam tip objekta koji se ubacuje iz njegovog globalnog identifikatora, u ovom slucaju DER, ANALOG ili DISCRETE
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

                //Pravim objekat DMS tipa, to moze biti der, analog ili discrete
                IdentifiedObject io = CreateEntity(globalId);

                //Ako nemamo properties onda smo zavrsili i napustamo i napustamo metodu InsertEntity
                if (rd.Properties == null)
                {
                    return;
                }

                foreach (Property property in rd.Properties)
                {
                    //GID se podesi kroz konstruktor tako da ga sada ne podesavamo
                    if (property.Id == ModelCode.IDOBJ_GID)
                    {
                        continue;
                    }

                    //Provera da li je property referenca, ako nije postavi property i uzmi sledeci
                    if (property.Type != PropertyType.Reference)
                    {
                        io.SetProperty(property);
                        continue;
                    }

                    //Ako je referenca 
                    //Uzmemo id objekta na koga se property odnosi
                    long targetGlobalId = property.AsReference();

                    if (targetGlobalId != 0)
                    {

                        //Ako postoji
                        IdentifiedObject targetEntity = GetEntity(targetGlobalId);
                        if (targetEntity == null)
                        {
                            string message1 = String.Format("Target entity is Substation (GID = 0x{0:x16})", globalId);
                            LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " ERROR - CalculationEngineModel.cs - " + message1);
                            continue;
                        }
                        targetEntity.AddReference(property.Id, io.GlobalId);
                    }

                    io.SetProperty(property);
                }

                string message = String.Format("Inserting entity with (GID = 0x{0:x16}) successfully finished.", globalId);
                LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " INFO - CalculationEngineModel.cs - " + message);
            }
            catch (Exception ex)
            {
                string message = String.Format("CALCULATION ENGINE - Failed to insert entity (GID = 0x{0:x16}) into model. {1}", rd.Id, ex.Message);
                LogHelper.Log(LogTarget.File, LogService.CETwoPhaseCommit, " ERROR - CalculationEngineModel.cs - " + message);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Creates entity for specified global inside the container.
        /// </summary>
        /// <param name="globalId">Global id of the entity for insert</param>		
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
                    dersCopy.Add(io as SynchronousMachine);
                    break;
                case DMSType.ANALOGVALUE:
                    io = new AnalogValue(globalId);
                    analogPointsCopy.Add(io as AnalogValue);
                    break;
                case DMSType.DISCRETEVALUE:
                    io = new DiscreteValue(globalId);
                    discretePointsCopy.Add(io as DiscreteValue);
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
                    foreach (SynchronousMachine item in dersCopy)
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
                    foreach (DiscreteValue item in discretePointsCopy)
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
            foreach (var item in discretePointsCopy)
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
            foreach (var item in dersCopy)
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
