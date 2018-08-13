namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	using FTN.Services.NetworkModelService.DataModel.Meas;
	using FTN.Services.NetworkModelService.DataModel.Core;
	using FTN.Services.NetworkModelService.DataModel.Wires;
	using Common;

	/// <summary>
	/// DERMSConveter has methods for populating
	/// ResourceDescription objects using PowerTransformerCIMProfile_Labs objects.
	/// </summary>
	public static class DERMSConveter 
	{

		#region Populate ResourceDescription
		public static void PopulateIdentifiedObjectProperties(DERMS.IdentifiedObject cimIdentifiedObject, ResourceDescription rd)
		{
			if ((cimIdentifiedObject != null) && (rd != null))
			{
				if (cimIdentifiedObject.MRIDHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimIdentifiedObject.MRID));
				}
				if (cimIdentifiedObject.NameHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimIdentifiedObject.Name));
				}
			}
		}

		public static void PopulatePowerSystemResourceProperties(DERMS.PowerSystemResource cimPowerSystemResource, ResourceDescription rd)
		{
			if ((cimPowerSystemResource != null) && (rd != null))
			{
				DERMSConveter.PopulateIdentifiedObjectProperties(cimPowerSystemResource, rd);
			}
		}

		public static void PopulateGeographicalRegionProperties(DERMS.GeographicalRegion cimGeographicalRegion, ResourceDescription rd)
		{
			if ((cimGeographicalRegion != null) && (rd != null))
			{
				DERMSConveter.PopulateIdentifiedObjectProperties(cimGeographicalRegion, rd);
			}
		}

		public static void PopulateSubGeographicalRegionProperties(DERMS.SubGeographicalRegion cimSubGeographicalRegion, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimSubGeographicalRegion != null) && (rd != null))
			{
				DERMSConveter.PopulateIdentifiedObjectProperties(cimSubGeographicalRegion, rd);

				if (cimSubGeographicalRegion.RegionHasValue)
				{
					long gid = importHelper.GetMappedGID(cimSubGeographicalRegion.Region.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimSubGeographicalRegion.GetType().ToString()).Append(" rdfID = \"").Append(cimSubGeographicalRegion.ID);
						report.Report.Append("\" - Failed to set reference to SubGeographicalRegion: rdfID \"").Append(cimSubGeographicalRegion.Region.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.SUBREGION_REGION, gid));
				}
			}
		}

		public static void PopulateMeasurementValueProperties(DERMS.MeasurementValue cimWMeasurementValue, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimWMeasurementValue != null) && (rd != null))
			{
				DERMSConveter.PopulateIdentifiedObjectProperties(cimWMeasurementValue, rd);

				if (cimWMeasurementValue.TimeStampHasValue)
				{
					rd.AddProperty(new Property(ModelCode.MEASUREMENTVALUE_TIMESTAMP, cimWMeasurementValue.TimeStamp.Ticks));
				}

				if (cimWMeasurementValue.PowerTypeHasValue)
				{
					rd.AddProperty(new Property(ModelCode.MEASUREMENTVALUE_POWERTYPE, (short)cimWMeasurementValue.PowerType)); 
				}

				if (cimWMeasurementValue.AddressHasValue)
				{
					rd.AddProperty(new Property(ModelCode.MEASUREMENTVALUE_ADDRESS, cimWMeasurementValue.Address));
				}

				if (cimWMeasurementValue.SynchronousMachineHasValue)
				{
					long gid = importHelper.GetMappedGID(cimWMeasurementValue.SynchronousMachine.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimWMeasurementValue.GetType().ToString()).Append(" rdfID = \"").Append(cimWMeasurementValue.ID);
						report.Report.Append("\" - Failed to set reference to MeasurementValue: rdfID \"").Append(cimWMeasurementValue.SynchronousMachine.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.MEASUREMENTVALUE_SYNCMACHINE, gid));
				}
			}
		}

		public static void PopulateEquipmentProperties(DERMS.Equipment cimEquipment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimEquipment != null) && (rd != null))
			{
				DERMSConveter.PopulatePowerSystemResourceProperties(cimEquipment, rd);
			
				if (cimEquipment.EquipmentContainerHasValue)
				{
					long gid = importHelper.GetMappedGID(cimEquipment.EquipmentContainer.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimEquipment.GetType().ToString()).Append(" rdfID = \"").Append(cimEquipment.ID);
						report.Report.Append("\" - Failed to set reference to Equipment: rdfID \"").Append(cimEquipment.EquipmentContainer.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.EQUIPMENT_EQCONTAINER, gid));
				}
			}
		}

		public static void PopulateConductingEquipmentProperties(DERMS.ConductingEquipment cimConductingEquipment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimConductingEquipment != null) && (rd != null))
			{
				DERMSConveter.PopulateEquipmentProperties(cimConductingEquipment, rd, importHelper, report);
			}
		}

		public static void PopulateRegulatingConductingEqProperties(DERMS.RegulatingCondEq cimRegulatingConductingEq, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimRegulatingConductingEq != null) && (rd != null))
			{
				DERMSConveter.PopulateConductingEquipmentProperties(cimRegulatingConductingEq, rd, importHelper, report);
			}
		}

		public static void PopulateRotatingMachineProperties(DERMS.RotatingMachine cimRotatingMachine, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimRotatingMachine != null) && (rd != null))
			{
				DERMSConveter.PopulateRegulatingConductingEqProperties(cimRotatingMachine, rd, importHelper, report);

				if (cimRotatingMachine.FuelTypeHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ROTATINGMACHINE_FUELTYPE, (short)GetDMSFuelType(cimRotatingMachine.FuelType)));
				}
				if (cimRotatingMachine.RatedSHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ROTATINGMACHINE_RATEDS, cimRotatingMachine.RatedS));
				}
				if (cimRotatingMachine.NominalPHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ROTATINGMACHINE_NOMINALP, cimRotatingMachine.NominalP));
				}
				if (cimRotatingMachine.NominalQHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ROTATINGMACHINE_NOMINALQ, cimRotatingMachine.NominalQ));
				}
				if (cimRotatingMachine.DERFlexibilityPHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ROTATINGMACHINE_DERFLEXIBILITYP, cimRotatingMachine.DERFlexibilityP));
				}
				if (cimRotatingMachine.DERFlexibilityQHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ROTATINGMACHINE_DERFLEXIBILITYQ, cimRotatingMachine.DERFlexibilityQ));
				}
			}
		}

		public static void PopulateSynchronousMachineProperties(DERMS.SynchronousMachine cimSynchronousMachine, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimSynchronousMachine != null) && (rd != null))
			{
				DERMSConveter.PopulateRotatingMachineProperties(cimSynchronousMachine, rd, importHelper, report);

				if (cimSynchronousMachine.BaseQHasValue)
				{
					rd.AddProperty(new Property(ModelCode.SYNCMACHINE_BASEQ, cimSynchronousMachine.BaseQ));
				}
				if (cimSynchronousMachine.MinQHasValue)
				{
					rd.AddProperty(new Property(ModelCode.SYNCMACHINE_MINQ, cimSynchronousMachine.MinQ));
				}
				if (cimSynchronousMachine.MaxQHasValue)
				{
					rd.AddProperty(new Property(ModelCode.SYNCMACHINE_MAXQ, cimSynchronousMachine.MaxQ));
				}
				if (cimSynchronousMachine.MinPHasValue)
				{
					rd.AddProperty(new Property(ModelCode.SYNCMACHINE_MINP, cimSynchronousMachine.MinP));
				}
				if (cimSynchronousMachine.MaxPHasValue)
				{
					rd.AddProperty(new Property(ModelCode.SYNCMACHINE_MAXP, cimSynchronousMachine.MaxP));
				}
			}
		}

		public static void PopulateAnalogValueProperties(DERMS.AnalogValue cimAnalogValue, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimAnalogValue != null) && (rd != null))
			{
				DERMSConveter.PopulateMeasurementValueProperties(cimAnalogValue, rd, importHelper, report);

				if (cimAnalogValue.ValueHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ANALOGVALUE_VALUE, cimAnalogValue.Value));
				}
			}
		}

		public static void PopulateDiscreteValueProperties(DERMS.DiscreteValue cimDiscreteValue, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimDiscreteValue != null) && (rd != null))
			{
				DERMSConveter.PopulateMeasurementValueProperties(cimDiscreteValue, rd, importHelper, report);

				if (cimDiscreteValue.ValueHasValue)
				{
					rd.AddProperty(new Property(ModelCode.DISCRETEVALUE_VALUE, cimDiscreteValue.Value));
				}
			}
		}

		public static void PopulateConnectivityNodeContainerProperties(DERMS.ConnectivityNodeContainer cimConnectivityNodeContainer, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimConnectivityNodeContainer != null) && (rd != null))
			{
				DERMSConveter.PopulatePowerSystemResourceProperties(cimConnectivityNodeContainer, rd);
			}
		}

		public static void PopulateEquipmentContainerProperties(DERMS.EquipmentContainer cimEquipmentContainer, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimEquipmentContainer != null) && (rd != null))
			{
				DERMSConveter.PopulateConnectivityNodeContainerProperties(cimEquipmentContainer, rd, importHelper, report);
			}
		}

		public static void PopulateSubstationProperties(DERMS.Substation cimSubstation, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimSubstation != null) && (rd != null))
			{
				DERMSConveter.PopulateEquipmentContainerProperties(cimSubstation, rd, importHelper, report);

				if (cimSubstation.LatitudeHasValue)
				{
					rd.AddProperty(new Property(ModelCode.SUBSTATION_LATITUDE, cimSubstation.Latitude));
				}
				if (cimSubstation.LongitudeHasValue)
				{
					rd.AddProperty(new Property(ModelCode.SUBSTATION_LONGITUDE, cimSubstation.Longitude));
				}

				if (cimSubstation.RegionHasValue)
				{
					long gid = importHelper.GetMappedGID(cimSubstation.Region.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimSubstation.GetType().ToString()).Append(" rdfID = \"").Append(cimSubstation.ID);
						report.Report.Append("\" - Failed to set reference to Equipment: rdfID \"").Append(cimSubstation.Region.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.SUBSTATION_SUBREGION, gid));
				}

			}
		}

		public static void PopulateAORAreaProperties(DERMS.AORArea cimAORArea, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimAORArea != null) && (rd != null))
			{
				DERMSConveter.PopulateIdentifiedObjectProperties(cimAORArea, rd);

				if (cimAORArea.IsControlableHasValue)
				{
					rd.AddProperty(new Property(ModelCode.AOR_AREA_CONTROLLABLE, cimAORArea.IsControlable));
				}
				if (cimAORArea.IsViewableHasValue)
				{
					rd.AddProperty(new Property(ModelCode.AOR_AREA_VIEWABLE, cimAORArea.IsViewable));
				}
				if (cimAORArea.CoveredByHasValue)
				{
					rd.AddProperty(new Property(ModelCode.AOR_AREA_COVEREDBY, cimAORArea.CoveredBy));
				}

				if (cimAORArea.AORUserHasValue)
				{
					long gid = importHelper.GetMappedGID(cimAORArea.AORUser.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimAORArea.GetType().ToString()).Append(" rdfID = \"").Append(cimAORArea.ID);
						report.Report.Append("\" - Failed to set reference to AORArea: rdfID \"").Append(cimAORArea.AORUser.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.AOR_AREA_USER, gid));
				}

				if (cimAORArea.AOR_AGAggregatorHasValue)
				{
					long gid = importHelper.GetMappedGID(cimAORArea.AOR_AGAggregator.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimAORArea.GetType().ToString()).Append(" rdfID = \"").Append(cimAORArea.ID);
						report.Report.Append("\" - Failed to set reference to AORArea: rdfID \"").Append(cimAORArea.AOR_AGAggregator.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.AOR_AREA_AGGREGATOR, gid));
				}
			}
		}

		public static void PopulateAORGroupProperties(DERMS.AORGroup cimAORGroup, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimAORGroup != null) && (rd != null))
			{
				DERMSConveter.PopulateIdentifiedObjectProperties(cimAORGroup, rd);

				if (cimAORGroup.IsCoveredHasValue)
				{
					rd.AddProperty(new Property(ModelCode.AOR_GROUP_COVERED, cimAORGroup.IsCovered));
				}

				if (cimAORGroup.AOR_AGAggregatorHasValue)
				{
					long gid = importHelper.GetMappedGID(cimAORGroup.AOR_AGAggregator.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimAORGroup.GetType().ToString()).Append(" rdfID = \"").Append(cimAORGroup.ID);
						report.Report.Append("\" - Failed to set reference to AORGroup: rdfID \"").Append(cimAORGroup.AOR_AGAggregator.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.AOR_GROUP_AGGREGATOR, gid));
				}
			}
		}

		public static void PopulateAORUserProperties(DERMS.AORUser cimAORUser, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimAORUser != null) && (rd != null))
			{
				DERMSConveter.PopulateIdentifiedObjectProperties(cimAORUser, rd);

				if (cimAORUser.ViewAreasHasValue)
				{
					rd.AddProperty(new Property(ModelCode.AOR_USER_VIEWAREAS, cimAORUser.ViewAreas));
				}
				if (cimAORUser.ControlAreasHasValue)
				{
					rd.AddProperty(new Property(ModelCode.AOR_USER_CONTROLAREAS, cimAORUser.ControlAreas));
				}
			}
		}

		#endregion Populate ResourceDescription

		#region Enums convert

		public static FuelType GetDMSFuelType(DERMS.FuelType fuelType)
		{
			switch (fuelType)
			{
				case DERMS.FuelType.coal:
					return FuelType.Coal;
				case DERMS.FuelType.oil:
					return FuelType.Oil;
				case DERMS.FuelType.gas:
					return FuelType.Gas;
				case DERMS.FuelType.lignite:
					return FuelType.Lignite;
				case DERMS.FuelType.sun:
					return FuelType.Sun;
				case DERMS.FuelType.wind:
					return FuelType.Wind;
				default:
					return FuelType.Coal;
			}
		}

		public static DERMS.PowerType GetDMSPowerType(DERMS.PowerType powerType)
		{
			switch (powerType)
			{
				case DERMS.PowerType.Active:
					return DERMS.PowerType.Active;
				case DERMS.PowerType.Reactive:
					return DERMS.PowerType.Reactive;
				default:
					return DERMS.PowerType.Active;
			}
		}

		#endregion Enums convert
	}
}
