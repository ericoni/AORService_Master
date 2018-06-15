using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.Common.SCADA;
using FTN.Common.Logger;

namespace FTN.Services.NetworkModelService
{	
	public class NetworkModel
	{
		/// <summary>
		/// Dictionaru which contains all data: Key - DMSType, Value - Container
		/// </summary>
		private SmartContainer smartContainer;
        private static object lockObj = new object();

		/// <summary>
		/// ModelResourceDesc class contains metadata of the model
		/// </summary>
		private ModelResourcesDesc resourcesDescs;
	
		/// <summary>
		/// Initializes a new instance of the Model class.
		/// </summary>
		public NetworkModel()
		{
			smartContainer = SmartContainer.Instance;
			smartContainer.Initialize();
			resourcesDescs = new ModelResourcesDesc();			
		}
	
		#region GDA query

		/// <summary>
		/// Gets resource description for entity requested by globalId.
		/// </summary>
		/// <param name="globalId">Id of the entity</param>
		/// <param name="properties">List of requested properties</param>		
		/// <returns>Resource description of the specified entity</returns>
		public ResourceDescription GetValues(long globalId, List<ModelCode> properties)
		{
			string message = String.Format("Getting values for GID = 0x{0:x16}.", globalId);
            LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " INFO - NetworkModel.cs - " + message);

            try
			{
                ResourceDescription rd; 

                lock (SmartContainer.Instance.Lock2PC)
                {
                    IdentifiedObject io = smartContainer.GetEntity(smartContainer.Original, globalId);

                    rd = new ResourceDescription(globalId);

                    Property property = null;

                    // insert specified properties
                    foreach (ModelCode propId in properties)
                    {
                        property = new Property(propId);
                        io.GetProperty(property);
                        rd.AddProperty(property);
                    }

                    message = String.Format("Getting values for GID = 0x{0:x16} succedded.", globalId);
                    LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " INFO - NetworkModel.cs - " + message);
                }

				return rd;
			}			
			catch (Exception ex)
			{
				message = string.Format("Failed to get values for entity with GID = 0x{0:x16}. {1}", globalId, ex.Message);
                LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " ERROR - NetworkModel.cs - " + message);
                throw new Exception(message);
			}
		}

		/// <summary>
		/// Gets resource iterator that holds descriptions for all entities of the specified type.
		/// </summary>		
		/// <param name="type">Type of entity that is requested</param>
		/// <param name="properties">List of requested properties</param>		
		/// <returns>Resource iterator for the requested entities</returns>
		public ResourceIterator GetExtentValues(ModelCode entityType, List<ModelCode> properties)
		{			
			string message = String.Format("Getting extent values for entity type = {0} .", entityType);
            LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " INFO - NetworkModel.cs - " + message);

            try
            {
                ResourceIterator ri;
                lock (SmartContainer.Instance.Lock2PC)
                {
                    List<long> globalIds = new List<long>();
                    Dictionary<DMSType, List<ModelCode>> class2PropertyIDs = new Dictionary<DMSType, List<ModelCode>>();

                    DMSType entityDmsType = ModelCodeHelper.GetTypeFromModelCode(entityType);

                    if (smartContainer.ContainerExists(smartContainer.Original, entityDmsType))
                    {
                        Container container = smartContainer.GetContainer(smartContainer.Original, entityDmsType);
                        globalIds = container.GetEntitiesGlobalIds();
                        class2PropertyIDs.Add(entityDmsType, properties);
                    }

                    ri = new ResourceIterator(globalIds, class2PropertyIDs);

                    message = String.Format("Getting extent values for entity type = {0} succedded.", entityType);
                    LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " INFO - NetworkModel.cs - " + message);
                }
                return ri;
            }
            catch (Exception ex)
            {
                message = string.Format("Failed to get extent values for entity type = {0}. {1}", entityType, ex.Message);
                LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " ERROR - NetworkModel.cs - " + message);

                throw new Exception(message);
            }			
		}
		
		/// <summary>
		/// Gets resource iterator that holds descriptions for all entities related to specified source.
		/// </summary>
		/// <param name="contextId">Context Id</param>
		/// <param name="properties">List of requested properties</param>
		/// <param name="association">Relation between source and entities that should be returned</param>
		/// <param name="source">Id of entity that is start for association search</param>
		/// <param name="typeOfQuery">Query type choice(global or local)</param>
		/// <returns>Resource iterator for the requested entities</returns>
		public ResourceIterator GetRelatedValues(long source, List<ModelCode> properties, Association association)
		{
			string message = String.Format("Getting related values for source = 0x{0:x16}.", source);
            LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " INFO - NetworkModel.cs - " + message);

            try
			{
                ResourceIterator ri;

                lock (SmartContainer.Instance.Lock2PC)
                {
                    List<long> relatedGids = ApplyAssocioationOnSource(source, association);

                    lock (SmartContainer.Instance.Lock2PC)
                    {
                        Dictionary<DMSType, List<ModelCode>> class2PropertyIDs = new Dictionary<DMSType, List<ModelCode>>();

                        foreach (long relatedGid in relatedGids)
                        {
                            DMSType entityDmsType = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(relatedGid);

                            if (!class2PropertyIDs.ContainsKey(entityDmsType))
                            {
                                class2PropertyIDs.Add(entityDmsType, properties);
                            }
                        }

                        ri = new ResourceIterator(relatedGids, class2PropertyIDs);
                    }

                    message = String.Format("Getting related values for source = 0x{0:x16} succeeded.", source);
                    LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " INFO - NetworkModel.cs - " + message);
                }

				return ri;
			}
			catch (Exception ex)
			{
				message = String.Format("Failed to get related values for source GID = 0x{0:x16}. {1}.", source, ex.Message);
                LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " ERROR - NetworkModel.cs - " + message);
                throw new Exception(message);
			}
		}

		#endregion GDA query	

		public UpdateResult ApplyDelta(Delta delta)
		{
			bool ValideDelta = false;
			UpdateResult updateResult = new UpdateResult();

            try
            {
                LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " INFO - NetworkModel.cs - Applying  delta to network model.");

                Dictionary<short, int> typesCounters = GetCounters();
                Dictionary<long, long> globalIdPairs = new Dictionary<long, long>();
                delta.FixNegativeToPositiveIds(ref typesCounters, ref globalIdPairs);
                updateResult.GlobalIdPairs = globalIdPairs;
                delta.SortOperations();

                ValideDelta = smartContainer.ApplayDelta(delta);
            }
            catch (Exception ex)
            {
                string message = string.Format("Applying delta to network model failed. {0}.", ex.Message);
                LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " ERROR - NetworkModel.cs - " + message);

                updateResult.Result = ResultType.Failed;
                updateResult.Message = message;
            }
            finally
            {
                if (ValideDelta)
                {
                    smartContainer.SaveDelta(delta);
                }

                if (updateResult.Result == ResultType.Succeeded)
                {
                    string message = "Applying delta to network model successfully finished.";
                    LogHelper.Log(LogTarget.File, LogService.NMSNetworkModel, " INFO - NetworkModel.cs - " + message);
                    updateResult.Message = message;
                }
            }

			return updateResult;
		}

		/// <summary>
		/// Returns related gids with source according to the association 
		/// </summary>
		/// <param name="source">source id</param>		
		/// <param name="association">desinition of association</param>
		/// <returns>related gids</returns>
		private List<long> ApplyAssocioationOnSource(long source, Association association)
		{
			List<long> relatedGids = new List<long>();

			if (association == null)
			{
				association = new Association();
			}

            lock (SmartContainer.Instance.Lock2PC)
            {
                IdentifiedObject io = smartContainer.GetEntity(smartContainer.Original, source);

                if (!io.HasProperty(association.PropertyId))
                {
                    throw new Exception(string.Format("Entity with GID = 0x{0:x16} does not contain prperty with Id = {1}.", source, association.PropertyId));
                }

                Property propertyRef = null;
                if (Property.GetPropertyType(association.PropertyId) == PropertyType.Reference)
                {
                    propertyRef = io.GetProperty(association.PropertyId);
                    long relatedGidFromProperty = propertyRef.AsReference();

                    if (relatedGidFromProperty != 0)
                    {
                        if (association.Type == 0 || (short)ModelCodeHelper.GetTypeFromModelCode(association.Type) == ModelCodeHelper.ExtractTypeFromGlobalId(relatedGidFromProperty))
                        {
                            relatedGids.Add(relatedGidFromProperty);
                        }
                    }
                }
                else if (Property.GetPropertyType(association.PropertyId) == PropertyType.ReferenceVector)
                {
                    propertyRef = io.GetProperty(association.PropertyId);
                    List<long> relatedGidsFromProperty = propertyRef.AsReferences();

                    if (relatedGidsFromProperty != null)
                    {
                        foreach (long relatedGidFromProperty in relatedGidsFromProperty)
                        {
                            if (association.Type == 0 || (short)ModelCodeHelper.GetTypeFromModelCode(association.Type) == ModelCodeHelper.ExtractTypeFromGlobalId(relatedGidFromProperty))
                            {
                                relatedGids.Add(relatedGidFromProperty);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception(string.Format("Association propertyId = {0} is not reference or reference vector type.", association.PropertyId));
                }
            }

			return relatedGids;
		}

		/// <summary>
		/// Writes delta to log
		/// </summary>
		/// <param name="delta">delta instance which will be logged</param>
		public static void TraceDelta(Delta delta)
		{
			try
			{
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
				xmlWriter.Formatting = Formatting.Indented;
				delta.ExportToXml(xmlWriter);
				xmlWriter.Flush();
				CommonTrace.WriteTrace(CommonTrace.TraceInfo, stringWriter.ToString());				
				xmlWriter.Close();
				stringWriter.Close();
			}
			catch (Exception ex)
			{
				CommonTrace.WriteTrace(CommonTrace.TraceError, "Failed to trace delta with ID = {0}. Reason: {1}", delta.Id, ex.Message);
			}
		}

        private Dictionary<short, int> GetCounters()
        {
            Dictionary<short, int> typesCounters = new Dictionary<short, int>();


            foreach (DMSType type in Enum.GetValues(typeof(DMSType)))
            {
                typesCounters[(short)type] = 0;

                if (smartContainer.Original.ContainsKey(type))
                {
                    typesCounters[(short)type] = smartContainer.GetContainer(smartContainer.Original, type).Count;
                }
            }

            return typesCounters;
        }

	}
}
