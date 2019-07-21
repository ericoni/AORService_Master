using FTN.Common;
using FTN.Common.Logger;
using FTN.Common.SCADA;
using FTN.Common.Services;
using FTN.Services.NetworkModelService.Database.Access;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService
{
    /// <summary>
    /// Implements "IDeltaNotify" interface.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SmartContainer : ITwoPhaseCommit, INMSSubscriber, IDeltaNotify
    {
        #region Fields
        /// <summary>
        /// Lista subscribera zainteresovanih za promenu statike
        /// </summary>
        private Dictionary<ITwoPhaseCommit, List<DMSType>> subscribers;

        /// <summary>
        /// Originalni network model
        /// </summary>
		private Dictionary<DMSType, Container> original;

        /// <summary>
        /// Kopija network modela na kojoj se vrse promene
        /// </summary>
		private Dictionary<DMSType, Container> copy;

        /// <summary>
        /// Delta koja se primenjuje
        /// </summary>
		private Delta delta;

        /// <summary>
        /// Za rad sa model kodovima
        /// </summary>
        private ModelResourcesDesc resourcesDescs;

        /// <summary>
        /// Singleton instanca
        /// </summary>
        private static volatile SmartContainer instance;

        /// <summary>
        /// Objekat za lock
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// Objekat za zaklucavanje distribuirane transakcije
        /// </summary>
        private object lock2PC = new object();

        /// <summary>
        /// Lista Ui-ova koji zele da budu obavesteni kad stigne delta
        /// </summary>
        private List<IDeltaNotifyCallback> registratedUIs = new List<IDeltaNotifyCallback>();

        #endregion Fields

        /// <summary>
        /// Get za original network model
        /// </summary>
        public Dictionary<DMSType, Container> Original
        {
            get { return original; }
            set { original = value; }
        }

        /// <summary>
        /// Get za copy network model
        /// </summary>
        public Dictionary<DMSType, Container> Copy
        {
            get { return copy; }
        }

        /// <summary>
        /// Get i Set za subscribers 
        /// </summary>
        public Dictionary<ITwoPhaseCommit, List<DMSType>> Subscribers
        {
            get { return subscribers; }
            set { subscribers = value; }
        }
    

        private SmartContainer()
        {
            subscribers = new Dictionary<ITwoPhaseCommit, List<DMSType>>();
            original = new Dictionary<DMSType, Container>();
            copy = new Dictionary<DMSType, Container>();
            resourcesDescs = new ModelResourcesDesc();
        }

        /// <summary>
        /// Metoda za Singleton
        /// </summary>
        public static SmartContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SmartContainer();
                    }
                }

                return instance;
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

        #region INMSSubscriber
        /// <summary>
        /// Svi zainteresovani za promene statickog modela se prijavljuju na dogadjaje preko ove metode
        /// </summary>
        /// <param name="topics">List DMS tipova na koje se klijent prijavljuje</param>
        public void Subscribed(List<DMSType> topics)
        {
            OperationContext context = OperationContext.Current;
            ITwoPhaseCommit client = context.GetCallbackChannel<ITwoPhaseCommit>();
          
            if (!subscribers.ContainsKey(client))
            {
                subscribers.Add(client, topics);
            }

            foreach (DMSType topic in topics)
            {
                if (subscribers[client].Contains(topic))
                {
                    continue;
                }
                subscribers[client].Add(topic);
            }
        }

        /// <summary>
        /// Ukoliko klijent prestane da bude zainteresovan na neki DMS tip ili uopsteno na promene NMS modela onda radi Unsubscribed
        /// </summary>
        /// <param name="topics">Lista DMS tipova sa kojih se odjavljuje</param>
        /// <returns></returns>
        public void Unsubscribed(List<DMSType> topics)
        {
			OperationContext context = OperationContext.Current;
			ITwoPhaseCommit client = context.GetCallbackChannel<ITwoPhaseCommit>();

			if (!subscribers.ContainsKey(client))
            {
                return;
            }

            foreach (DMSType topic in topics)
            {
                if (!subscribers[client].Contains(topic))
                {
                    continue;
                }
                subscribers[client].Remove(topic);
            }

            if (subscribers[client].Count != 0)
            {
                return;
            }

            subscribers.Remove(client);
        }
        #endregion INMSSubscriber
        /// <summary>
        /// Obavestavanje subscriber-a za Prepare i kreiranje delte za svakog subscriber-a
        /// </summary>
        /// <returns></returns>
        private bool NotifySubscribersPrepare()
        {
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (ITwoPhaseCommit client in subscribers.Keys)
            {
                Delta temp = new Delta();

                foreach (ResourceDescription rd in delta.InsertOperations)
                {
                    if (!subscribers[client].Contains((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)))
                    {
                        continue;
                    }
                    temp.InsertOperations.Add(rd);
                }

                foreach (ResourceDescription rd in delta.UpdateOperations)
                {
                    if (!subscribers[client].Contains((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)))
                    {
                        continue;
                    }
                    temp.UpdateOperations.Add(rd);
                }

                foreach (ResourceDescription rd in delta.DeleteOperations)
                {
                    if (!subscribers[client].Contains((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)))
                    {
                        continue;
                    }
                    temp.DeleteOperations.Add(rd);
                }

				
                bool retVal = client.Prepare(temp);

				if (retVal)
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Obavestavanje subscriber-a za Commit
        /// </summary>
        /// <returns></returns>
        private void NotifySubscribersCommit()
        {
            foreach (ITwoPhaseCommit client in Subscribers.Keys)
            {
				client.Commit();
            }
        }

        /// <summary>
        /// Obavestavanje subscriber-a za Rolback
        /// </summary>
        /// <returns></returns>
        private void NotifySubscribersRoleback()
        {
            foreach (ITwoPhaseCommit client in Subscribers.Keys)
            {
				client.Rollback();
            }
        }

        /// <summary>
        /// Radi primenu delte i two-phase commit
        /// </summary>
        /// <param name="delta">Pristigle promene</param>
        public bool ApplayDelta(Delta delta)
        {
            //Stigna nova delta, postavimo deltu
            this.delta = delta;
            LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " INFO - SmartContainer.cs - Delta is received.");

            //Radi se Prepare ako se dogodi neki izuzetak radi se Rollback
            try
            {
                lock (Lock2PC)
                {
                    Prepare(delta);
                    LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " INFO - SmartContainer.cs - Prepare finished successfully.");
                }
            }
            catch (Exception ex)
            {
                lock (Lock2PC)
                {
                    LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " ERROR - SmartContainer.cs - Prepare is finished with error: " + ex.Message);
                    Rollback();
                    LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " INFO - SmartContainer.cs - Rollback finished successfully.");
                }
                throw ex;
            }

            if (!NotifySubscribersPrepare())
            {
                lock (Lock2PC)
                {
                    LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " ERROR - SmartContainer.cs - Prepare subscribers is finished with error.");
                    Rollback();
                    LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " INFO - SmartContainer.cs - Rollback finished successfully.");
                }

                NotifySubscribersRoleback();
                LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " INFO - SmartContainer.cs - Rollback subscribers finished successfully.");

                return false;
            }

            lock (Lock2PC)
            {
                Commit();
                LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " INFO - SmartContainer.cs - Commit is finished successfully.");
            }
        
            NotifySubscribersCommit();

            // Za ui
            new Thread(() => {
                Notify();
            }).Start(); 

            LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " INFO - SmartContainer.cs - Commit subscribers is finished successfully.");

            return true;
        }

        #region ITwoPhaseCommit
        /// <summary>
        /// Radi radi primenu delte na kopiji, poziva Prepare na svim zainteresovanim servisima
        /// </summary>
        /// <returns></returns>
        public bool Prepare(Delta delta)
        {
            //Inicijalizacija kopije tako da pokazuje na iste kontejnere kao sto pokazuje i original
            SetUp();

            try
            {
                //Ubacujemo nove entitete
                InsertEntities(delta.InsertOperations);

                //Azurira entitete
                UpdateEntities(delta.UpdateOperations);

                //Brise entitete
                DeleteEntities(delta.DeleteOperations);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        /// <summary>
        /// Kopija postaje origilan, radi Commit na svim zainteresovanim servisima
        /// </summary>
        /// <returns></returns>
        public void Commit()
        {
            original = copy;
            copy = new Dictionary<DMSType, Container>();
        }

        /// <summary>
        /// Odbacuje kopiju, delta nije primenita, radi Rollback na svim zainteresovanim servisima
        /// </summary>
        /// <returns></returns>
        public void Rollback()
        {
            copy = new Dictionary<DMSType, Container>();
        }

        #endregion ITwoPhaseCommit

        /// <summary>
        /// Radi inicijalizaciju kopije
        /// </summary>
        private void SetUp()
        {
            foreach (DMSType type in original.Keys)
            {
                copy.Add(type, original[type]);
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

        /// <summary>
        /// Azurira entitete tako sto poziva metodu UpdateEntity()
        /// </summary>
        /// <param name="rds">Lista Resource Description-a koje azurira</param>
        private void UpdateEntities(List<ResourceDescription> rds)
        {
            foreach (ResourceDescription rd in rds)
            {
                try
                {
                    UpdateEntity(rd);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Brise entitete tako sto poziva metodu DeleteEntity()
        /// </summary>
        /// <param name="rds">Lista Resource Description-a koje brise</param>
        private void DeleteEntities(List<ResourceDescription> rds)
        {
            foreach (ResourceDescription rd in rds)
            {
                try
                {
                    DeleteEntity(rd);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Shallow kopi nad kontejnerom, osnovna logika je da kopira kontejner a entitete preveze iz originala
        /// </summary>
        /// <param name="typeOfConainer">tip kontejnera</param>
        /// <returns></returns>
        private Container ConatinerShallowCopy(DMSType typeOfConainer)
        {
            Container container = new Container();

            //Proveravamo da li postoji kontejner u kopiji, ako ne postoji pravimo novi
            //Ako postoji proveravamo da li postoji u originalu, ako ne postoji vratimo kontejner iz kopije
            //Ako postoji proveravamo da li je ista referenca originala i kopije, ako nije vratimo kontejner iz kopije -> kopija je vec napravljena
            //Ako jeste napravi novi kontejner i zameni sa postojecim i nastavi dalje
            if (!ContainerExists(copy, typeOfConainer))
            {
                copy.Add(typeOfConainer, container);
            }
            else
            {
                if (!ContainerExists(original, typeOfConainer))
                {
                    return copy[typeOfConainer];
                }

                if (!object.ReferenceEquals(original[typeOfConainer], copy[typeOfConainer]))
                {
                    return copy[typeOfConainer];
                }

                copy[typeOfConainer] = container;
            }

            //Proveravamo da li postoji kontejner u originalu, ako ne postoji to znaci da nemam sta da prevezem
            if (!ContainerExists(original, typeOfConainer))
            {
                return container;
            }

            //Ako postoji kontejner onda moram da prevezem sve njegove objekte na kontejner kopije
            foreach (long gid in original[typeOfConainer].Entities.Keys)
            {
                container.Entities.Add(gid, original[typeOfConainer].Entities[gid]);
            }

            return container;
        }

        /// <summary>
        /// Insert entiteta u kopiju
        /// </summary>
        /// <param name="rd">Resource description</param>
		private void InsertEntity(ResourceDescription rd)
        {
            string message = "";

            if (rd == null)
            {
                message = String.Format("Insert entity is not done because update operation is empty.");
                LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " ERROR - SmartContainer.cs - " + message);

                throw new Exception(message);
            }

            long globalId = rd.Id;
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, "Inserting entity with GID ({0:x16}).", globalId);

            //Proveravam da li postoji entitet koji zelim da dodam u originalnom modelu ako postoji, bacam izuzetak	
            if (this.EntityExists(original, globalId))
            {
                message = String.Format("Failed to insert entity because entity with specified GID ({0:x16}) already exists in network model.", globalId);
                LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " ERROR - SmartContainer.cs - " + message);
                throw new Exception(message);
            }

            try
            {
                //Dobijam tip objekta koji se ubacuje iz njegovog globalnog identifikatora
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

                //Pravim Shallow kopiju kontejnera tako sto napravim novi kontejner i u njega prevezem sve reference iz originala
                //Ideja: Sve sto dodirnem kopiram
                Container containerCopy = ConatinerShallowCopy(type);

                //Pravim objekat DMS tipa, to moze biti region, subregion, substation u zavisnosti sta ubacujem -> ne podesavam propertije
                //Taj objekat unutar metode CreateEntity ubacujem u kopiju kontejnera
                //Sada kopija ima veze prema svim entitetima iz originalnog kontejnera plus novi objekat
                IdentifiedObject io = containerCopy.CreateEntity(globalId);

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

                    //Provera da li postoji entitet sa istim mrid-om, ako postoji ne prolazi test
                    if (property.Id == ModelCode.IDOBJ_MRID)
                    {
                        if (this.ContainsMrid(original, globalId, property.PropertyValue.StringValue))
                        {
                            message = String.Format("Failed to insert entity because entity with specified GID ({0:x16}) already exists in network model.", globalId);
                            LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " ERROR - SmartContainer.cs - " + message);
                            throw new Exception(message);
                        }
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
                        //Provera da li postoji objekat koji je referenciran od strane trenutnog propertija
                        //Ako ne postoji onda je to greska
                        if (!EntityExists(copy, targetGlobalId))
                        {
                            message = string.Format("Failed to get target entity with GID: 0x{0:X16}. {1}", targetGlobalId);
                            LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " ERROR - SmartContainer.cs - " + message);
                            throw new Exception(message);
                        }

                        //Ako postoji
                        IdentifiedObject targetEntity = GetEntity(copy, targetGlobalId);
                        targetEntity.AddReference(property.Id, io.GlobalId);
                    }

                    io.SetProperty(property);
                }

                message = String.Format("Inserting entity with GID ({0:x16}) successfully finished.", globalId);
                LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " INFO - SmartContainer.cs - " + message);
            }
            catch (Exception ex)
            {
                message = String.Format("Failed to insert entity (GID = 0x{0:x16}) into model. {1}", rd.Id, ex.Message);
                LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " ERROR - SmartContainer.cs - " + message);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Update entiteta u kopiji
        /// </summary>
        /// <param name="rd">Resource description</param>
        private void UpdateEntity(ResourceDescription rd)
        {

            if (rd == null || rd.Properties == null && rd.Properties.Count == 0)
            {
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, "Update entity is not done because update operation is empty.");
                return;
            }

            try
            {
                long globalId = rd.Id;

                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Updating entity with GID ({0:x16}).", globalId);

                if (!this.EntityExists(original, globalId))
                {
                    string message = String.Format("Failed to update entity because entity with specified GID ({0:x16}) does not exist in network model.", globalId);
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
                }

                //Dobijam tip objekta koji se ubacuje iz njegovog globalnog identifikatora
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

                //Pravim Shallow kopiju kontejnera tako sto napravim novi kontejner i u njega prevezem sve reference iz originala
                //Ideja: Sve sto dodirnem kopiram
                Container containerCopy = ConatinerShallowCopy(type);

                //Dobijam postojeci objekat koji je isti i za kopiju i za original
                IdentifiedObject io = GetEntity(copy, globalId);

                //Vrsi se deep kopi tog objekta
                IdentifiedObject copyIo = io.DeepCopy();
                //Na mesto original objekta u copy kontejneru ubacujem kopiju tog objekta
                copy[type].Entities[io.GlobalId] = copyIo;

                //Prolazimo kroz sve properties i radimo njihov update
                foreach (Property property in rd.Properties)
                {
                    //Prvo proveravamo da li je property referenca
                    if (property.Type == PropertyType.Reference)
                    {
                        long oldTargetGlobalId = copyIo.GetProperty(property.Id).AsReference();

                        if (oldTargetGlobalId != 0)
                        {
                            //Pronadjem objekat sa kojim je objekat povezan
                            IdentifiedObject oldTargetEntity = GetEntity(copy, oldTargetGlobalId);

                            //Pravi se kopija target entiteta
                            IdentifiedObject copyOldTargetEntity = oldTargetEntity.DeepCopy();

                            //Dobijem kontejner target elementa uradim ShallowCopy kontejnera
                            DMSType typeTarget = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(oldTargetEntity.GlobalId);
                            ConatinerShallowCopy(typeTarget);

                            //Zamena originalnog entiteta sa kopijom u novom kontejneru
                            copy[typeTarget].Entities[copyOldTargetEntity.GlobalId] = copyOldTargetEntity;

                            //Ukolonim tu referencu
                            copyOldTargetEntity.RemoveReference(property.Id, globalId);
                        }

                        //Postavim id nove reference
                        long targetGlobalId = property.AsReference();

                        if (targetGlobalId != 0)
                        {
                            if (!EntityExists(copy, targetGlobalId))
                            {
                                string message = string.Format("Failed to get target entity with GID: 0x{0:X16}.", targetGlobalId);
                                throw new Exception(message);
                            }

                            //Pronadjem novi obejak na koji je povezan i setujem mu referencu
                            IdentifiedObject targetEntity = GetEntity(copy, targetGlobalId);

                            //Pravii se kopija target entiteta
                            IdentifiedObject copyTargetEntity = targetEntity.DeepCopy();

                            //Dobijem kontejner target elementa uradim ShallowCopy kontejnera
                            DMSType typeTarget = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(targetEntity.GlobalId);
                            ConatinerShallowCopy(typeTarget);

                            //Zamena originalnog entiteta sa kopijom u novom kontejneru
                            copy[typeTarget].Entities[targetEntity.GlobalId] = copyTargetEntity;

                            copyTargetEntity.AddReference(property.Id, globalId);
                        }

                        //Odradi update ostalih properties
                        copyIo.SetProperty(property);
                    }
                    else
                    {
                        //Odradi update ostalih properties
                        copyIo.SetProperty(property);
                    }
                }

                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Updating entity with GID ({0:x16}) successfully finished.", globalId);
            }
            catch (Exception ex)
            {
                string message = String.Format("Failed to update entity (GID = 0x{0:x16}) in model. {1} ", rd.Id, ex.Message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Brisanje entiteta iz kopije
        /// </summary>
        /// <param name="rd">Resource description</param>
        private void DeleteEntity(ResourceDescription rd)
        {
            if (rd == null)
            {
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, "Delete entity is not done because update operation is empty.");
                return;
            }

            try
            {
                long globalId = rd.Id;

                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Deleting entity with GID ({0:x16}).", globalId);

                //Proveravamo da li GlobalId postoji u kopiji, ako ne postoji zavrsavamo 
                if (!this.EntityExists(copy, globalId))
                {
                    string message = String.Format("Failed to delete entity because entity with specified GID ({0:x16}) does not exist in network model.", globalId);
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
                }

                //Trazimo entitet koji se brise
                IdentifiedObject io = GetEntity(copy, globalId);

                //Proveravamo da li entitet moze da se brise (moze da se brise ako nije referanciran od drugog entiteta)
                if (io.IsReferenced)
                {
                    Dictionary<ModelCode, List<long>> references = new Dictionary<ModelCode, List<long>>();
                    io.GetReferences(references, TypeOfReference.Target);

                    StringBuilder sb = new StringBuilder();

                    foreach (KeyValuePair<ModelCode, List<long>> kvp in references)
                    {
                        foreach (long referenceGlobalId in kvp.Value)
                        {
                            sb.AppendFormat("0x{0:x16}, ", referenceGlobalId);
                        }
                    }

                    string message = String.Format("Failed to delete entity (GID = 0x{0:x16}) because it is referenced by entities with GIDs: {1}.", globalId, sb.ToString());
                    CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                    throw new Exception(message);
                }

                //Pronadjem sve moled kodove property-a za dati entitet
                List<ModelCode> propertyIds = resourcesDescs.GetAllSettablePropertyIdsForEntityId(io.GlobalId);

                //Brisem reference
                Property property = null;
                foreach (ModelCode propertyId in propertyIds)
                {
                    PropertyType propertyType = Property.GetPropertyType(propertyId);

                    //Ako model kod nije referenca, odbacujemo ga i proveravamo drugi
                    if (propertyType != PropertyType.Reference)
                    {
                        continue;
                    }

                    //Ako jeste uzimamo property sa tim model kodom
                    property = io.GetProperty(propertyId);

                    //Dobijamo id target entiteta
                    long targetGlobalId = property.AsReference();

                    if (targetGlobalId != 0)
                    {
                        //Dobijem target entitet i uradim njegov deep copy
                        IdentifiedObject targetEntity = GetEntity(copy, targetGlobalId);
                        IdentifiedObject copyTargetEntity = targetEntity.DeepCopy();

                        //Dobijem kontejner target elementa uradim ShallowCopy kontejnera
                        DMSType typeTarget = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(targetEntity.GlobalId);
                        ConatinerShallowCopy(typeTarget);

                        //Zamena originalnog entiteta sa kopijom u novom kontejneru
                        copy[typeTarget].Entities[copyTargetEntity.GlobalId] = copyTargetEntity;

                        //Brise se referenca 
                        copyTargetEntity.RemoveReference(propertyId, globalId);
                    }
                }

                //Radimo shallow copy kontejnera
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
                ConatinerShallowCopy(type);
                Container container = GetContainer(copy, type);

                //Brisemo entitet iz kopije
                container.RemoveEntity(globalId);

                CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Deleting entity with GID ({0:x16}) successfully finished.", globalId);
            }
            catch (Exception ex)
            {
                string message = String.Format("Failed to delete entity (GID = 0x{0:x16}) from model. {1}", rd.Id, ex.Message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }
        }

        #region Find
        /// <summary>
        /// Checks if container exists in model.
        /// </summary>
        /// <param name="type">Type of container.</param>
        /// <returns>True if container exists, otherwise FALSE.</returns>
        public bool ContainerExists(Dictionary<DMSType, Container> model, DMSType type)
        {
            if (model.ContainsKey(type))
            {
                return true;
            }

            return false;
        }

        public bool EntityExists(Dictionary<DMSType, Container> model, long globalId)
        {
            DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

            if (ContainerExists(model, type))
            {
                Container container = GetContainer(model, type);

                if (container.EntityExists(globalId))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsMrid(Dictionary<DMSType, Container> model, long globalId, string mrid)
        {
            DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);

            if (ContainerExists(model, type))
            {
                Container container = GetContainer(model, type);

                if (container.ContainsMrid(mrid))
                {
                    return true;
                }
            }

            return false;
        }

        public IdentifiedObject GetEntity(Dictionary<DMSType, Container> model, long globalId)
        {
            if (EntityExists(model, globalId))
            {
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId); // vrati se, iz nepoznatog razloga vrati pogresan DMS type (34359738372)
				IdentifiedObject io = GetContainer(model, type).GetEntity(globalId);

                return io;
            }
            else
            {
                string message = string.Format("Entity  (GID = 0x{0:x16}) does not exist.", globalId);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Gets container of specified type.
        /// </summary>
        /// <param name="type">Type of container.</param>
        /// <returns>Container for specified local id</returns>
        public Container GetContainer(Dictionary<DMSType, Container> model, DMSType type)
        {
            if (ContainerExists(model, type))
            {
                return model[type];
            }
            else
            {
                string message = string.Format("Container does not exist for type {0}.", type);
                throw new Exception(message);
            }

        }
        #endregion Find

        /// <summary>
        /// Metoda za inicijalizaciju Smart Containera
        /// </summary>
        public void Initialize()
        {
            List<Delta> result = ReadAllDeltas();

            foreach (Delta delta in result)
            {
                try
                {
                    InsertEntities(delta.InsertOperations);
                    UpdateEntities(delta.UpdateOperations);
                    DeleteEntities(delta.DeleteOperations);
                    original = copy;
                    copy = new Dictionary<DMSType, Container>();
                }
                catch (Exception ex)
                {
                    CommonTrace.WriteTrace(CommonTrace.TraceError, "Error while applying delta (id = {0}) during service initialization. {1}", delta.Id, ex.Message);
                }
            }
        }

        /// <summary>
        /// Metoda za cuvanje uspesno primenite delte
        /// </summary>
        /// <param name="delta"></param>
        public void SaveDelta(Delta delta)
        {
            if (NmsDB.Instance.AddDelta(delta))
            {
                LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " INFO - SmartContainer.cs - Successfull saved delta entry.");
            }
            else
            {
                LogHelper.Log(LogTarget.File, LogService.NMSSmartContainer, " ERROR - SmartContainer.cs - Failed to save delta entry.");
            }

        }

        /// <summary>
        /// Metoda za iscitavanje prethodno uspesno primenitih delti
        /// </summary>
        /// <returns></returns>
        private List<Delta> ReadAllDeltas()
        {
            return NmsDB.Instance.ReadDeltas();
        }

        #region IDeltaNotify implementation
        public void Register()
        {
            // Subscribe the user to the conversation
            IDeltaNotifyCallback registeredUser = OperationContext.Current.GetCallbackChannel<IDeltaNotifyCallback>();

            if (!registratedUIs.Contains(registeredUser))
            {
                registratedUIs.Add(registeredUser);
            }
        }

        public void Notify()
        {
            List<IDeltaNotifyCallback> removeUIs = new List<IDeltaNotifyCallback>();

            // Notify the users of a cache change.
            foreach (IDeltaNotifyCallback user in registratedUIs)
            {
                try
                {
                    user.Refresh();
                }
                catch (Exception)
                {
                    removeUIs.Add(user);
                }
            }

            foreach (IDeltaNotifyCallback service in removeUIs)
            {
                registratedUIs.Remove(service);
            }
        }

        public void Unregister()
        {
            // Unsubscribe the user from the conversation.      
            IDeltaNotifyCallback registeredUser = OperationContext.Current.GetCallbackChannel<IDeltaNotifyCallback>();

            if (registratedUIs.Contains(registeredUser))
            {
                registratedUIs.Remove(registeredUser);
            }
        }
        #endregion IDeltaNotify implementation
    }
}
