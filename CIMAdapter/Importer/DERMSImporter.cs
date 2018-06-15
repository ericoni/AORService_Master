using System;
using System.Collections.Generic;
using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	/// <summary>
	/// DERMSImporter
	/// </summary>
	public class DERMSImporter
	{
		/// <summary> Singleton </summary>
		private static DERMSImporter ptImporter = null;
		private static object singletoneLock = new object();

		private ConcreteModel concreteModel;
		private Delta delta;
		private ImportHelper importHelper;
		private TransformAndLoadReport report;


		#region Properties
		public static DERMSImporter Instance
		{
			get
			{
				if (ptImporter == null)
				{
					lock (singletoneLock)
					{
						if (ptImporter == null)
						{
							ptImporter = new DERMSImporter();
							ptImporter.Reset();
						}
					}
				}
				return ptImporter;
			}
		}

		public Delta NMSDelta
		{
			get 
			{
				return delta;
			}
		}
		#endregion Properties


		public void Reset()
		{
			concreteModel = null;
			delta = new Delta();
			importHelper = new ImportHelper();
			report = null;
		}

		public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
		{
			LogManager.Log("Importing DERMS Elements...", LogLevel.Info);
			report = new TransformAndLoadReport();
			concreteModel = cimConcreteModel;
			delta.ClearDeltaOperations();

			if ((concreteModel != null) && (concreteModel.ModelMap != null))
			{
				try
				{
					// convert into DMS elements
					ConvertModelAndPopulateDelta();
				}
				catch (Exception ex)
				{
					string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
					LogManager.Log(message);
					report.Report.AppendLine(ex.Message);
					report.Success = false;
				}
			}
			LogManager.Log("Importing DERMS Elements - END.", LogLevel.Info);
			return report;
		}

		/// <summary>
		/// Method performs conversion of network elements from CIM based concrete model into DMS model.
		/// </summary>
		private void ConvertModelAndPopulateDelta()
		{
			LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

			//// import all concrete model types (DMSType enum)

			ImportGeographicalRegions();
			ImportSubGeographicalRegions();
			ImportSubstations();
			ImportSynchronousMachines();
			ImportAnalogValues();
			ImportDiscretValues();

			LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
		}

		#region Import

		#region Import Region
		private void ImportGeographicalRegions()
		{
			SortedDictionary<string, object> cimRegions = concreteModel.GetAllObjectsOfType("DERMS.GeographicalRegion");
			if (cimRegions != null)
			{
				foreach (KeyValuePair<string, object> cimRegionPair in cimRegions)
				{
					DERMS.GeographicalRegion cimRegion = cimRegionPair.Value as DERMS.GeographicalRegion;

					ResourceDescription rd = CreateGeographicalRegionResourceDescription(cimRegion);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("GeographicalRegion ID = ").Append(cimRegion.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("GeographicalRegion ID = ").Append(cimRegion.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateGeographicalRegionResourceDescription(DERMS.GeographicalRegion cimRegion)
		{
			ResourceDescription rd = null;
			if (cimRegion != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.REGION, importHelper.CheckOutIndexForDMSType(DMSType.REGION));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimRegion.ID, gid);

				////populate ResourceDescription
				DERMSConveter.PopulateGeographicalRegionProperties(cimRegion, rd);
			}
			return rd;
		}

		#endregion Import Region

		#region Import SubRegion
		private void ImportSubGeographicalRegions()
		{
			SortedDictionary<string, object> cimSubRegions = concreteModel.GetAllObjectsOfType("DERMS.SubGeographicalRegion");
			if (cimSubRegions != null)
			{
				foreach (KeyValuePair<string, object> cimSubRegionPair in cimSubRegions)
				{
					DERMS.SubGeographicalRegion cimSubRegion = cimSubRegionPair.Value as DERMS.SubGeographicalRegion;

					ResourceDescription rd = CreateSubGeographicalRegionResourceDescription(cimSubRegion);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("SubGeographicalRegion ID = ").Append(cimSubRegion.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("SubGeographicalRegion ID = ").Append(cimSubRegion.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateSubGeographicalRegionResourceDescription(DERMS.SubGeographicalRegion cimSubRegion)
		{
			ResourceDescription rd = null;
			if (cimSubRegion != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SUBREGION, importHelper.CheckOutIndexForDMSType(DMSType.SUBREGION));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimSubRegion.ID, gid);

				////populate ResourceDescription
				DERMSConveter.PopulateSubGeographicalRegionProperties(cimSubRegion, rd, importHelper, report);
			}
			return rd;
		}

		#endregion Import SubRegion

		#region Import Substation
		private void ImportSubstations()
		{
			SortedDictionary<string, object> cimSubstations = concreteModel.GetAllObjectsOfType("DERMS.Substation");
			if (cimSubstations != null)
			{
				foreach (KeyValuePair<string, object> cimSubstationPair in cimSubstations)
				{
					DERMS.Substation cimSubstation = cimSubstationPair.Value as DERMS.Substation;

					ResourceDescription rd = CreateSubstationResourceDescription(cimSubstation);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("Substation ID = ").Append(cimSubstation.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("Substation ID = ").Append(cimSubstation.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateSubstationResourceDescription(DERMS.Substation cimSubstation)
		{
			ResourceDescription rd = null;
			if (cimSubstation != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SUBSTATION, importHelper.CheckOutIndexForDMSType(DMSType.SUBSTATION));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimSubstation.ID, gid);

				////populate ResourceDescription
				DERMSConveter.PopulateSubstationProperties(cimSubstation, rd, importHelper, report);
			}
			return rd;
		}
		#endregion Import Substation

		#region Import SynchronousMachine
		private void ImportSynchronousMachines()
		{
			SortedDictionary<string, object> cimSynchronousMachines = concreteModel.GetAllObjectsOfType("DERMS.SynchronousMachine");
			if (cimSynchronousMachines != null)
			{
				foreach (KeyValuePair<string, object> cimSynchronousMachinePair in cimSynchronousMachines)
				{
					DERMS.SynchronousMachine cimSynchronousMachine = cimSynchronousMachinePair.Value as DERMS.SynchronousMachine;

					ResourceDescription rd = CreateSynchronousMachineResourceDescription(cimSynchronousMachine);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("SynchronousMachine ID = ").Append(cimSynchronousMachine.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("SynchronousMachine ID = ").Append(cimSynchronousMachine.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateSynchronousMachineResourceDescription(DERMS.SynchronousMachine cimSynchronousMachine)
		{
			ResourceDescription rd = null;
			if (cimSynchronousMachine != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SYNCMACHINE, importHelper.CheckOutIndexForDMSType(DMSType.SYNCMACHINE));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimSynchronousMachine.ID, gid);

				////populate ResourceDescription
				DERMSConveter.PopulateSynchronousMachineProperties(cimSynchronousMachine, rd, importHelper, report);
			}
			return rd;
		}
		#endregion Import SynchronousMachine

		#region Import AnalogValue
		private void ImportAnalogValues()
		{
			SortedDictionary<string, object> cimAnalogValues = concreteModel.GetAllObjectsOfType("DERMS.AnalogValue");
			if (cimAnalogValues != null)
			{
				foreach (KeyValuePair<string, object> cimAnalogValuePair in cimAnalogValues)
				{
					DERMS.AnalogValue cimAnalogValue = cimAnalogValuePair.Value as DERMS.AnalogValue;

					ResourceDescription rd = CreateAnalogValueResourceDescription(cimAnalogValue);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("AnalogValue ID = ").Append(cimAnalogValue.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("AnalogValue ID = ").Append(cimAnalogValue.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateAnalogValueResourceDescription(DERMS.AnalogValue cimAnalogValue)
		{
			ResourceDescription rd = null;
			if (cimAnalogValue != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ANALOGVALUE, importHelper.CheckOutIndexForDMSType(DMSType.ANALOGVALUE));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimAnalogValue.ID, gid);

				////populate ResourceDescription
				DERMSConveter.PopulateAnalogValueProperties(cimAnalogValue, rd, importHelper, report);
			}
			return rd;
		}
		#endregion Import AnalogValue

		#region Import DiscretValue
		private void ImportDiscretValues()
		{
			SortedDictionary<string, object> cimDiscretValues = concreteModel.GetAllObjectsOfType("DERMS.DiscreteValue");
			if (cimDiscretValues != null)
			{
				foreach (KeyValuePair<string, object> cimDiscretValuePair in cimDiscretValues)
				{
					DERMS.DiscreteValue cimDiscretValue = cimDiscretValuePair.Value as DERMS.DiscreteValue;

					ResourceDescription rd = CreateDiscreteValueResourceDescription(cimDiscretValue);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("DiscreteValue ID = ").Append(cimDiscretValue.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("DiscreteValue ID = ").Append(cimDiscretValue.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateDiscreteValueResourceDescription(DERMS.DiscreteValue cimDiscretValue)
		{
			ResourceDescription rd = null;
			if (cimDiscretValue != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.DISCRETEVALUE, importHelper.CheckOutIndexForDMSType(DMSType.DISCRETEVALUE));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimDiscretValue.ID, gid);

				////populate ResourceDescription
				DERMSConveter.PopulateDiscreteValueProperties(cimDiscretValue, rd, importHelper, report);
			}
			return rd;
		}
		#endregion Import DiscretValue

		#endregion Import
	}
}

