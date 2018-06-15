using Adapter;
using CalculationEngine.Interfaces;
using FTN.Common;
using FTN.ServiceContracts;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.CalculationEngine.Model;
using SmartCacheLibrary;
using System.ServiceModel;
using System.Threading;
using CommonCE;
using FTN.Common.CE.Model;
using FTN.Common.SCADA;
using FTN.Common.Logger;
using SCADAReceivingProxyNS;
using CalculationEngine;

namespace CalculationEngService
{
    public class CalculationEngineDistributer : ICEDistribution
    {
        INetworkModelClient rdAdapter = null;
        CalculationEngineForecast forecast = null;
        SCADASetpointProxy SCADAProxy = null;

        /// <summary>
        /// Adapter for NMS data
        /// </summary>
        private RDAdapter adapter = new RDAdapter();

        public INetworkModelClient RDAdapter
        {
            get { return rdAdapter; }
            set { rdAdapter = value; }
        }

        public CalculationEngineDistributer()
        {
            rdAdapter = new RDAdapter();
            forecast = new CalculationEngineForecast();
            SCADAProxy = new SCADASetpointProxy();
        }

        /// <summary>
        /// Distributes demanded power to DERs associated to specific gid.
        /// </summary>
        /// <param name="gid">Region, subregion, or entire network </param>
        /// <param name="demandedValue">Demanded Value</param>
        /// <param name="powerType"> Power Type (Active/Reactive) </param>
        /// <param name="anaValues"> List of analog values </param>
        /// <returns></returns>
        public Setpoint NominalPowerDistribution(long gid, float demandedValue, PowerType powerType, List<AnalogValue> anaValues)
        {
            LogHelper.Log(LogTarget.File, LogService.CalculationEngineDistributer, " INFO - CalculationEngineDistributer.cs - Distribute demanded power to DERs associated to gid: " +gid);
            List<SynchronousMachine> syncMachines = RDAdapter.GetDERs(gid); // Get all DERs from a region, subregion or substation
            List<AnalogValue> analogValues = anaValues;
            Dictionary<long, float> PDistributionBySM = new Dictionary<long, float>(syncMachines.Count);
            SynchronousMachine syncMachine = null;

            Setpoint setpoint = new Setpoint();
            setpoint.PDistributionByAV = new Dictionary<long, float>(syncMachines.Count);

            long smGid = 0;
            int safetyCounter = 0;
            float coefficient = 0;
            float smNominalPower = 0;
            float nominalPowerSum = 0;
            float smDelta = 0;
            float smFlexibility = 0;
            float smMaxPower = 0;
            float smMinPower = 0;
            float smAvailableReserve = 0;
            float smAvailableReserveUp = 0;
            float smAvailableReserveDown = 0;
            float demandValue = demandedValue;
            bool isDeltaPositive = false;

            isDeltaPositive = demandedValue >= 0;

            switch (powerType)
            {
                case PowerType.Active:
                    nominalPowerSum = syncMachines.Sum(o => o.NominalP);
                    break;
                case PowerType.Reactive:
                    nominalPowerSum = syncMachines.Sum(o => o.NominalQ);
                    break;
                default:
                    break;
            }

            coefficient = demandedValue / nominalPowerSum;

            while (demandValue > 0 && isDeltaPositive == true ||
                demandValue < 0 && isDeltaPositive == false)
            {
                foreach (AnalogValue aValue in analogValues.Where(p => p.PowerType == powerType))
                {
                    syncMachine = syncMachines.Find(sm => sm.GlobalId == aValue.SynchronousMachine);

                    switch (powerType)
                    {
                        case PowerType.Active:
                            smFlexibility = syncMachine.DERFlexibilityP;
                            smMaxPower = syncMachine.MaxP;
                            smMinPower = syncMachine.MinP;
                            smNominalPower = syncMachine.NominalP;
                            smAvailableReserveUp = syncMachine.MaxP - aValue.Value;
                            smAvailableReserveDown = aValue.Value - syncMachine.MinP;
                            break;
                        case PowerType.Reactive:
                            smFlexibility = syncMachine.DERFlexibilityQ;
                            smMaxPower = syncMachine.MaxQ;
                            smMinPower = syncMachine.MinQ;
                            smNominalPower = syncMachine.NominalQ;
                            smAvailableReserveUp = syncMachine.MaxQ - aValue.Value;
                            smAvailableReserveDown = aValue.Value - syncMachine.MinQ;
                            break;
                        default:
                            break;
                    }

                    if (aValue.Value == smMaxPower && isDeltaPositive == true)
                    {
                        continue;
                    }

                    if (aValue.Value == smMinPower && isDeltaPositive == false)
                    {
                        continue;
                    }

                    smDelta = coefficient * smNominalPower; // Proportionaly split required power, by nominalP value
                    smGid = syncMachine.GlobalId;

                    smAvailableReserve = isDeltaPositive ? smAvailableReserveUp : smAvailableReserveDown;

                    if (smAvailableReserve == 0)
                    {
                        LogHelper.Log(LogTarget.File, LogService.CalculationEngineDistributer, " WARNING - CalculationEngineDistributer.cs - No available reserve for gid: " + gid);
                        continue;
                    }

                    //korak redukovanja K, ne moze biti veci od MAX ni manji od MIN
                    ReduceKToProperValue(ref smDelta, smAvailableReserve, isDeltaPositive, aValue);

                    if (Math.Abs(smDelta) <= smFlexibility) // ako delta ne prelazi max, da li je DER dovoljno flexibilan?
                    {
                        CheckDistributedPartsOnGoodFlex(PDistributionBySM, aValue, setpoint, smDelta, smFlexibility, isDeltaPositive, ref demandValue);
                    }
                    else // ako DER nije dovoljno fleksibilan
                    {
                        CheckDistributedPartsOnPoorFlex(PDistributionBySM, aValue, setpoint, smDelta, smFlexibility, isDeltaPositive, ref demandValue);
                    }
                }

                if (safetyCounter++ == 5)
                {
                    Trace.Write("Warning: 5 iterations were done, but no solution has been found.");
                    LogHelper.Log(LogTarget.File, LogService.CalculationEngineDistributer, " WARNING - CalculationEngineDistributer.cs - 5 iterations were done, but no solution has been found to gid: " + gid);

                    return setpoint;
                }
            }
            return setpoint;
        }

        /// <summary>
        /// Gets available Active or Reactive power, based on DERs' flexibities
        /// </summary>
        /// <param name="gid">Gid of region, subregion or entire network </param>
        /// <param name="demandedValue"> Demanded increase</param>
        /// <param name="anaValues"> List of analog values </param>
        /// <param name="powerType"> Power type (active or reactive) </param>
        /// <returns></returns>
        public float GetAvailablePower(long gid, float demandedValue, List<AnalogValue> anaValues, PowerType powerType)
        {
            LogHelper.Log(LogTarget.File, LogService.CalculationEngineDistributer, " INFO - CalculationEngineDistributer.cs - Get available Active or Reactive power, based on DERs' flexibities for gid: " + gid);

            float availableSum = 0;
            float availableDERReserve = 0;
            float smFlexibility = 0;
            SynchronousMachine syncMachine = null;
            List<SynchronousMachine> syncMachines = RDAdapter.GetDERs(gid);

            foreach (var aValue in anaValues)
            {
                if (aValue.PowerType != powerType)
                {
                    continue;
                }

                syncMachine = syncMachines.Find(sm => sm.GlobalId == aValue.SynchronousMachine);

                switch (powerType)
                {
                    case PowerType.Active:
                        availableDERReserve = syncMachine.MaxP - aValue.Value;
                        smFlexibility = syncMachine.DERFlexibilityP;
                        break;
                    case PowerType.Reactive:
                        availableDERReserve = syncMachine.MaxQ - aValue.Value;
                        smFlexibility = syncMachine.DERFlexibilityQ;
                        break;
                    default:
                        return -1;
                }

                if (smFlexibility <= availableDERReserve)
                {
                    availableSum += smFlexibility;
                }
                else
                {
                    availableSum += availableDERReserve;
                }
            }
            return availableSum;
        }

        /// <summary>
        /// Distribute required power by available reserve per each DER
        /// </summary>
        /// <param name="gid"> Region, subRegion or Entire network</param>
        /// <param name="demandedValue">Demanded power value</param>
        /// <param name="powerType"> Power type (Active/Reactive)</param>
        /// <param name="anaValues"> List of Analog Values</param>
        /// <returns></returns>
        public Setpoint AvailableReserveDistribution(long gid, float demandedValue, PowerType powerType, List<AnalogValue> anaValues)
        {
            LogHelper.Log(LogTarget.File, LogService.CalculationEngineDistributer, " INFO - CalculationEngineDistributer.cs - Distribute required power by available reserve per each DER for gid: " + gid);

            List<SynchronousMachine> syncMachines = RDAdapter.GetDERs(gid); // Get all DERs from a region, subregion or substation
            List<AnalogValue> analogValues = anaValues;
            SynchronousMachine syncMachine = null;

            Dictionary<long, float> PDistributionBySM = new Dictionary<long, float>(syncMachines.Count);

            //Dictionary<long, float> currentReserveBySm = new Dictionary<long, float>(syncMachines.Count);

            Setpoint setpoint = new Setpoint();
            setpoint.PDistributionByAV = new Dictionary<long, float>();

            float demandValue = demandedValue;
            float smFlexibility = 0;
            float smAvailableReserve = 0;
            float smAvailableReserveUp = 0;
            float smAvailableReserveDown = 0;
            float smAvailableReserveSum = 0;
            float smMaxPower = 0;
            float smMinPower = 0;
            float safetyCounter = 0;
            bool isDeltaPositive = false;

            isDeltaPositive = demandedValue >= 0;

            foreach (AnalogValue aValue in anaValues.Where(a => a.PowerType == powerType))
            {
                syncMachine = syncMachines.Find(sm => sm.GlobalId == aValue.SynchronousMachine);

                switch (powerType)
                {
                    case PowerType.Active:
                        smAvailableReserveUp = syncMachine.MaxP - aValue.Value;
                        smAvailableReserveDown = aValue.Value - syncMachine.MinP;
                        break;
                    case PowerType.Reactive:
                        smAvailableReserveUp = syncMachine.MaxQ - aValue.Value;
                        smAvailableReserveDown = aValue.Value - syncMachine.MinQ;
                        break;
                    default:
                        throw new FormatException("Invalid power type assigned to AnalogValue.");
                }

                smAvailableReserve = isDeltaPositive ? smAvailableReserveUp : smAvailableReserveDown;
                smAvailableReserveSum += smAvailableReserve;
            }

            while (demandValue > 0 && isDeltaPositive == true ||
                demandValue < 0 && isDeltaPositive == false)
            {
                foreach (AnalogValue aValue in anaValues.Where(a => a.PowerType == powerType))
                {
                    syncMachine = syncMachines.Find(sm => sm.GlobalId == aValue.SynchronousMachine);

                    switch (powerType)
                    {
                        case PowerType.Active:
                            smMaxPower = syncMachine.MaxP;
                            smMinPower = syncMachine.MinP;
                            smFlexibility = syncMachine.DERFlexibilityP;
                            smAvailableReserveUp = syncMachine.MaxP - aValue.Value;
                            smAvailableReserveDown = aValue.Value - syncMachine.MinP;
                            break;
                        case PowerType.Reactive:
                            smMaxPower = syncMachine.MaxQ;
                            smMinPower = syncMachine.MinQ;
                            smFlexibility = syncMachine.DERFlexibilityQ;
                            smAvailableReserveUp = syncMachine.MaxQ - aValue.Value;
                            smAvailableReserveDown = aValue.Value - syncMachine.MinQ;
                            break;
                        default:
                            break;
                    }

                    if (aValue.Value == smMaxPower && isDeltaPositive == true)
                    {
                        continue;
                    }

                    if (aValue.Value == smMinPower && isDeltaPositive == false)
                    {
                        continue;
                    }

                    long referencedSMgid = aValue.SynchronousMachine;

                    smAvailableReserve = isDeltaPositive ? smAvailableReserveUp : smAvailableReserveDown;

                    if (smAvailableReserve == 0)
                    {
                        continue;
                    }

                    float k = smAvailableReserve * demandedValue / smAvailableReserveSum;

                    //korak redukovanja K, ne moze biti veci od MAX ni manji od MIN
                    ReduceKToProperValue(ref k, smAvailableReserve, isDeltaPositive, aValue);

                    if (Math.Abs(k) <= smFlexibility)
                    {
                        CheckDistributedPartsOnGoodFlex(PDistributionBySM, aValue, setpoint, k, smFlexibility, isDeltaPositive, ref demandValue);
                    }
                    else
                    {
                        CheckDistributedPartsOnPoorFlex(PDistributionBySM, aValue, setpoint, k, smFlexibility, isDeltaPositive, ref demandValue);
                    }

                }

                if (safetyCounter++ == 5)
                {
                    Trace.Write("Warning: 5 iterations were done, but no solution has been found.");
                    return setpoint;
                }
            }
            return setpoint;
        }

        /// <summary>
        /// Helper function to distribute power when DER is flexible enough
        /// </summary>
        /// <param name="aValue"></param>
        /// <param name="setpoint"></param>
        /// <param name="smDelta"></param>
        /// <param name="smFlexibility"></param>
        /// <param name="dValue"></param>
        private void CheckDistributedPartsOnGoodFlex(Dictionary<long, float> pDistributionBySM, AnalogValue aValue,
            Setpoint setpoint, float smDelta, float smFlexibility, bool isPositiveD, ref float dValue)
        {
            long smGid = aValue.SynchronousMachine;
            LogHelper.Log(LogTarget.File, LogService.CalculationEngineDistributer, " INFO - CalculationEngineDistributer.cs - Distribute power for flexible enough der: " + smGid);

            if (pDistributionBySM.ContainsKey(smGid)) // ako se vec nalazi u listi za distr, provjeriti trenutno opterecenje
            {
                if (Math.Abs(pDistributionBySM[smGid] + smDelta) <= smFlexibility)
                {
                    if (isPositiveD) //ubaceno zbog negativnih delti
                    {
                        if (dValue >= smDelta)
                        {
                            pDistributionBySM[smGid] += smDelta;
                            dValue -= smDelta;
                            aValue.Value += smDelta;
                            UpdateAnalogValuesList(aValue, setpoint);
                        }

                        else
                        {
                            pDistributionBySM[smGid] += dValue;
                            aValue.Value += dValue;
                            UpdateAnalogValuesList(aValue, setpoint);
                            dValue = 0;
                        }
                    }
                    else
                    {
                        if (dValue <= smDelta)
                        {
                            pDistributionBySM[smGid] += smDelta;
                            dValue -= smDelta;
                            aValue.Value += smDelta;
                            UpdateAnalogValuesList(aValue, setpoint);
                        }
                        else
                        {
                            pDistributionBySM[smGid] += dValue;
                            aValue.Value += dValue;
                            UpdateAnalogValuesList(aValue, setpoint);
                            dValue = 0;
                        }
                    }
                }
                else // ne moze se dodati cjelokupna delta
                {
                    float availableOnDER = smFlexibility - Math.Abs(pDistributionBySM[smGid]);

                    if (isPositiveD) //ubaceno zbog negativnih delti
                    {
                        if (dValue >= availableOnDER)
                        {
                            pDistributionBySM[smGid] += availableOnDER;
                            dValue -= availableOnDER;
                            aValue.Value += availableOnDER;
                            UpdateAnalogValuesList(aValue, setpoint);
                        }
                        else
                        {
                            pDistributionBySM[smGid] += dValue;
                            aValue.Value += dValue;
                            UpdateAnalogValuesList(aValue, setpoint);
                            dValue = 0;
                        }
                    }
                    else
                    {
                        availableOnDER = availableOnDER * (-1);
                        if (dValue <= availableOnDER)
                        {
                            pDistributionBySM[smGid] += availableOnDER;
                            dValue -= availableOnDER;
                            aValue.Value += availableOnDER;
                            UpdateAnalogValuesList(aValue, setpoint);
                        }
                        else
                        {
                            pDistributionBySM[smGid] += dValue;
                            aValue.Value += dValue;
                            UpdateAnalogValuesList(aValue, setpoint);
                            dValue = 0;
                        }
                    }
                }
            }
            else // ako se prvi put pojavljuje ta SM u raspodjeli
            {
                if (isPositiveD) // ubaceno zbog negativnih delti
                {
                    if (dValue >= smDelta)
                    {
                        pDistributionBySM.Add(smGid, smDelta); // napunjen je do kraja, a ostaka smDelte nas ne zanima
                        dValue -= smDelta;
                        aValue.Value += smDelta;
                        UpdateAnalogValuesList(aValue, setpoint);
                    }
                    else
                    {
                        pDistributionBySM.Add(smGid, dValue); // napunjen je do kraja, a ostaka smDelte nas ne zanima
                        aValue.Value += dValue;
                        UpdateAnalogValuesList(aValue, setpoint);
                        dValue = 0;
                    }
                }
                else
                {
                    if (dValue <= smDelta)
                    {
                        pDistributionBySM.Add(smGid, smDelta);
                        dValue -= smDelta;
                        aValue.Value += smDelta;
                        UpdateAnalogValuesList(aValue, setpoint);
                    }
                    else
                    {
                        pDistributionBySM.Add(smGid, dValue);
                        aValue.Value += dValue;
                        UpdateAnalogValuesList(aValue, setpoint);
                        dValue = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Helper function to distribute power when DER is not flexible enough
        /// </summary>
        /// <param name="aValue"></param>
        /// <param name="setpoint"></param>
        /// <param name="smDelta"></param>
        /// <param name="smFlexibility"></param>
        /// <param name="dValue"></param>
        private void CheckDistributedPartsOnPoorFlex(Dictionary<long, float> pDistributionBySM, AnalogValue aValue,
            Setpoint setpoint, float smDelta, float smFlexibility, bool isPositiveD, ref float dValue)
        {
            long smGid = aValue.SynchronousMachine;
            LogHelper.Log(LogTarget.File, LogService.CalculationEngineDistributer, " INFO - CalculationEngineDistributer.cs - Distribute power for NOT flexible enough der: " + smGid);

            if (pDistributionBySM.ContainsKey(smGid)) // Da li je vec nesto rasporedjeno na DER?
            {
                float smDeltaPart = smFlexibility - Math.Abs(pDistributionBySM[smGid]); // zahtjeva se vrijednost koja je preko flex, stoga napunimo DER do kraja njegovog flexibility

                if (isPositiveD) //ubaceno zbog negativnih delti
                {
                    if (dValue >= smDeltaPart)
                    {
                        pDistributionBySM[smGid] += smDeltaPart;
                        dValue -= smDeltaPart;
                        aValue.Value += smDeltaPart;
                        UpdateAnalogValuesList(aValue, setpoint);
                    }

                    else
                    {
                        pDistributionBySM[smGid] += dValue;
                        aValue.Value += dValue;
                        UpdateAnalogValuesList(aValue, setpoint);
                        dValue = 0;
                    }
                }
                else
                {
                    if (dValue <= smDeltaPart)
                    {
                        pDistributionBySM[smGid] += smDeltaPart;
                        dValue -= smDeltaPart;
                        aValue.Value += smDeltaPart;
                        UpdateAnalogValuesList(aValue, setpoint);
                    }
                    else
                    {
                        pDistributionBySM[smGid] += dValue;
                        aValue.Value += dValue;
                        UpdateAnalogValuesList(aValue, setpoint);
                        dValue = 0;
                    }
                }
            }
            else
            {
                if (isPositiveD) // ubaceno zbog negativnih delti
                {
                    if (dValue >= smFlexibility)
                    {
                        pDistributionBySM.Add(smGid, smFlexibility); // napunjen je do kraja, a ostaka smDelte nas ne zanima
                        dValue -= smFlexibility;
                        aValue.Value += smFlexibility;
                        UpdateAnalogValuesList(aValue, setpoint);
                    }
                    else
                    {
                        pDistributionBySM.Add(smGid, dValue); // napunjen je do kraja, a ostaka smDelte nas ne zanima
                        aValue.Value += dValue;
                        UpdateAnalogValuesList(aValue, setpoint);
                        dValue = 0;
                    }
                }
                else
                {
                    smFlexibility = smFlexibility * -1;
                    if (dValue <= smFlexibility)
                    {
                        pDistributionBySM.Add(smGid, smFlexibility);
                        dValue -= smFlexibility;
                        aValue.Value += smFlexibility;
                        UpdateAnalogValuesList(aValue, setpoint);
                    }
                    else
                    {
                        pDistributionBySM.Add(smGid, dValue);
                        aValue.Value += dValue;
                        UpdateAnalogValuesList(aValue, setpoint);
                        dValue = 0;
                    }
                }
            }
        }

        private void UpdateAnalogValuesList(AnalogValue analogValue, Setpoint setpoint)
        {
            long aValueGid = analogValue.GlobalId;

            if (setpoint.PDistributionByAV.ContainsKey(aValueGid))
            {
                setpoint.PDistributionByAV[aValueGid] = analogValue.Value;
            }
            else
            {
                setpoint.PDistributionByAV.Add(aValueGid, analogValue.Value);
            }
        }

        /// <summary>
        /// Helper function to reduce delta step to be not higher than Max, not lower than Min
        /// </summary>
        /// <param name="k"></param>
        /// <param name="availableReserve"></param>
        /// <param name="isDeltaPositive"></param>
        /// <param name="aValue"></param>
        private void ReduceKToProperValue(ref float k, float availableReserve, bool isDeltaPositive, AnalogValue aValue)
        {
            if (isDeltaPositive)
            {
                if (k > availableReserve)
                {
                    k = availableReserve;
                }
            }
            else
            {
                if (Math.Abs(k) > availableReserve)
                {
                    k = availableReserve * (-1);
                }
            }
        }

        /// <summary>
        /// Funkcija koju UI poziva i vrsi resporedjivanje i upravljanje SetPoint-ima
        /// </summary>
        /// <param name="command"> Komanda </param>
        /// <returns> Da li je moguce primenit komandu</returns>
        public bool DistributePowerClient(Command command)
        {
            // Rezultat Proracuna ce biti smesten u dictionary. Kljuc je vreme a vrednost set point
            SortedDictionary<long, Setpoint> distributePowerResult = new SortedDictionary<long, Setpoint>();

            // Ako ako je vec komandovano, komanda se brise i pokusava se ponovo komandovati
            if (CommandExists(command.GlobalId, command.PowerType))
            {
                return false;
            }

            // Lista sinhroni masina nad kojima se komanduje
            List<SynchronousMachine> ders = GetDERsForGlobalID(command.GlobalId);

            // Ako nema sinhroni masina -> kraj
            if (ders == null || ders.Count.Equals(0))
            {
                return false;
            }

            // Podaci sa skade, prikupljeni iz SmartCache za sve der-ove koji pripadaju gid-u komande
            List<AnalogValue> currentMeasurement = GetSCADADataForDERs(ders, command.PowerType);

            // Ako nema merenja, ne mogu da se postave setPoint-i
            if (currentMeasurement == null || currentMeasurement.Count.Equals(0))
            {
                return false;
            }

            // Prognoza za naredni period
            List<ForecastObject> forecastObjects = GetPowerForecastForDERs(ders);

            // Uradimo filter za samo ono vreme koje nam je klijent vratio i sortiramo po vremenu
            SortedDictionary<long, List<AnalogValue>> measurementDictionary = GroupAndFilterByTime(command.Duration, command.PowerType, forecastObjects);

            // Ubacimo i currentMeasurement u sva merenja
            measurementDictionary.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0).Ticks, currentMeasurement);

            // Proracun SetPoint-a za svaki ForecastObject
            foreach (long key in measurementDictionary.Keys)
            {
                // Proracun SetPoint-a
                Setpoint setPoint = GetSetPoint(command, CopyList(measurementDictionary[key]));

                // Ako se ne mogu odrediti setPoint-i -> kraj
                if (setPoint == null)
                {
                    return false;
                }

                distributePowerResult.Add(key, setPoint);
            }

            List<Command> commands = CreateCommands(measurementDictionary, distributePowerResult, command);

            // Pokretanje thread-a koji ce da primenjuje jedan po jedan SetPoint u tacno predefinisanom vremenu
            Thread coordinator = new Thread(() => ThreadCoordinator(distributePowerResult, commands));
            coordinator.Start();

            return true;
        }

        public bool CommandExists(long gid, PowerType powerType)
        {
            // Lista sinhroni masina nad kojima se komanduje
            List<SynchronousMachine> ders = GetDERsForGlobalID(gid);

            // Ako nema sinhroni masina -> kraj
            if (ders == null || ders.Count.Equals(0))
            {
                return false;
            }

            foreach (SynchronousMachine der in ders)
            {
                AnalogValue analogValue = CalculationEngineModel.Instance.AnalogPointsOriginal.Where(o => o.SynchronousMachine.Equals(der.GlobalId) && o.PowerType.Equals(powerType)).FirstOrDefault();

                if (analogValue == null)
                {
                    continue;
                }

                List<Command> commands = null;
                CalculationEngineModel.Instance.AppliedCommands.TryGetValue(analogValue.GlobalId, out commands);

                if (commands == null)
                {
                    continue;
                }

                List<Command> exist = commands.Where(o => o.EndTime >= DateTime.Now.Ticks).ToList();

                if(exist.Count == 0 || exist == null)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public bool CancelCommand(long gid, PowerType powerType)
        {
            // Lista sinhroni masina nad kojima se komanduje
            List<SynchronousMachine> ders = GetDERsForGlobalID(gid);

            // Lista sa komandama za brisanje
            List<Command> commandsForRemove = new List<Command>();

            // Ako nema sinhroni masina -> kraj
            if (ders == null || ders.Count.Equals(0))
            {
                return false;
            }

            foreach (SynchronousMachine der in ders)
            {
                AnalogValue analogValue = CalculationEngineModel.Instance.AnalogPointsOriginal.Where(o => o.SynchronousMachine.Equals(der.GlobalId) && o.PowerType.Equals(powerType)).FirstOrDefault();

                if (analogValue == null)
                {
                    continue;
                }

                List<Command> commands = null;
                CalculationEngineModel.Instance.AppliedCommands.TryGetValue(analogValue.GlobalId, out commands);

                if (commands == null)
                {
                    continue;
                }

                commandsForRemove.AddRange(commands.Where(o => o.EndTime > DateTime.Now.Ticks).ToList());
            }

            foreach (Command command in commandsForRemove)
            {
                CalculationEngineModel.Instance.RemoveCommand(command);
            }

            return true;
        }

        public SortedDictionary<long, float> GetApplaiedCommands(long gid, PowerType powerType)
        {
            // Povratna vrednost
            SortedDictionary<long, float> returnValue = new SortedDictionary<long, float>();

            // Pocetak dana
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime endTime = startTime.AddDays(1);

            // Inicijalizuje se povratna vrednost, vreme od pocetka do kraja dana se postavlja za kljuceve a vrednosti su 0
            for (int i = 0; i < 23; i++)
            {
                returnValue.Add(startTime.AddHours(i).Ticks, 0);
            }

            // Lista sinhroni masina nad kojima se komanduje
            List<SynchronousMachine> ders = GetDERsForGlobalID(gid);

            // Ako nema sinhroni masina -> kraj
            if (ders == null || ders.Count.Equals(0))
            {
                return null;
            }

            foreach (SynchronousMachine der in ders)
            {
                AnalogValue analogValue = CalculationEngineModel.Instance.AnalogPointsOriginal.Where(o => o.SynchronousMachine.Equals(der.GlobalId) && o.PowerType.Equals(powerType)).FirstOrDefault();

                if (analogValue == null)
                {
                    continue;
                }

                List<Command> applaiedCommands = null;
                CalculationEngineModel.Instance.AppliedCommands.TryGetValue(analogValue.GlobalId, out applaiedCommands);

                if (applaiedCommands == null)
                {
                    continue;
                }

                List<Command> activeCommands = applaiedCommands.Where(o => o.StartTime >= startTime.Ticks && o.EndTime <= endTime.Ticks).ToList();

                if (activeCommands == null || activeCommands.Count.Equals(0))
                {
                    continue;
                }

                foreach (Command activeComand in activeCommands)
                {
                    returnValue[activeComand.StartTime] += activeComand.DemandedPower;
                }
            }


            return returnValue;
        }

        #region Helper functions for DistributePowerClient

        /// <summary>
        /// Pomocna funkcija koja radi deep copy liste merenja
        /// </summary>
        /// <param name="mesurement"> Lista merenja </param>
        /// <returns> Kopija liste </returns>
        private List<AnalogValue> CopyList(List<AnalogValue> mesurement)
        {
            List<AnalogValue> copy = new List<AnalogValue>();

            foreach (AnalogValue av in mesurement)
            {
                AnalogValue copyAv = new AnalogValue(av.GlobalId);
                copyAv.Mrid = av.Mrid;
                copyAv.Name = av.Name;
                copyAv.Description = av.Description;
                copyAv.Address = av.Address;
                copyAv.Value = av.Value;
                copyAv.Timestamp = av.Timestamp;
                copyAv.PowerType = av.PowerType;
                copyAv.SynchronousMachine = av.SynchronousMachine;
                copyAv.PowIncrease = av.PowIncrease;
                copyAv.PowDecrease = av.PowDecrease;

                copy.Add(copyAv);
            }

            return copy;
        }

        private List<Command> CreateCommands(SortedDictionary<long, List<AnalogValue>> measurementDictionary, SortedDictionary<long, Setpoint> distributePowerResult, Command globalCommand)
        {
            List<Command> commands = new List<Command>();

            foreach (long key in measurementDictionary.Keys)
            {
                foreach (AnalogValue value in measurementDictionary[key])
                {
                    // Oredjivanje koliki je bio increase ili decrease
                    float change = distributePowerResult[key].PDistributionByAV[value.GlobalId] - value.Value;

                    // Kreiranje komande
                    Command command = new Command(value.GlobalId, change, globalCommand.PowerType, 1, globalCommand.OptimizationType);

                    // Mrid ostaje isti, jer je to ista komanda samo izdeljena
                    command.Mrid = globalCommand.Mrid;

                    // Pocetak vazenja komande
                    command.StartTime = key;

                    // Kraj vazenja komande
                    command.EndTime = new DateTime(key).AddHours(1).Ticks;

                    commands.Add(command);
                }
            }

            return commands;
        }

        /// <summary>
        /// Pomocna funkcija koja primenjuje jedan po jedan SetPoint u tacno predefinisanom vremenu
        /// </summary>
        /// <param name="distributePowerResult"> Vremena i SetPoint-i </param>
        /// <param name="command"> Komanda </param>
        private void ThreadCoordinator(SortedDictionary<long, Setpoint> distributePowerResult, List<Command> commands)
        {
            // Dodavanje komande
            CalculationEngineModel.Instance.AddCommand(commands);
            string commandMRID = commands.FirstOrDefault().Mrid;

            long previousKey = -1;
            foreach (long key in distributePowerResult.Keys)
            {
                // Ako Komanda ne postoji u listi komandi, onda je stigla nova komanda ili je ova ponistena - thread se zavrsava
                List<Command> existCommands = CalculationEngineModel.Instance.AppliedCommands.Values.Where(o => o.Any(p => p.Mrid.Equals(commandMRID) &&
                                                                                                                           p.StartTime.Equals(key) &&
                                                                                                                           p.EndTime.Equals(new DateTime(key).AddHours(1).Ticks))).FirstOrDefault();
                if (existCommands == null)
                {
                    return;
                }

                List<CAS> casSignals = new List<CAS>();

                foreach (long gid in distributePowerResult[key].PDistributionByAV.Keys)
                {
                    casSignals.Add(new CAS(gid, CASEnum.Normal));
                }

                if (previousKey != -1)
                {
                    foreach (long gid in distributePowerResult[previousKey].PDistributionByAV.Keys)
                    {
                        if (casSignals.Where(o => o.Gid.Equals(gid)).FirstOrDefault() == null)
                        {
                            casSignals.Add(new CAS(gid, CASEnum.FailSafe));
                        }
                    }
                }

                CASSubscriber.Instance.NotifySubscribers(casSignals);

                SCADAProxy.Proxy.ReceiveAndSetSetpoint(distributePowerResult[key]);

                // Budjenje u tacno odredjenom vremenskom trenutku
                SetUpTimer(new DateTime(key).AddHours(1).Ticks);
                previousKey = key;
            }
        }

        /// <summary>
        /// Timer - budjenje thread-a u tacno vreme
        /// </summary>
        /// <param name="ticks"></param>
        private void SetUpTimer(long ticks)
        {
            DateTime current = DateTime.Now;

            // Vreme koliko treba da se spava do prvog okruglog sata
            int sleepTime = (int)(new DateTime(ticks) - current).TotalMilliseconds;

            Thread.Sleep(sleepTime);
        }


        /// <summary>
        /// Pomocna funkcija koja na osnovu GlobalId pribavlja sve DER-ove, GlobalId moze da se odnosi na Region, SubRegion, Substation ili sam DER
        /// </summary>
        /// <param name="gid"> Globalni identifikator</param>
        /// <returns> Lista DER-ova </returns>
        private List<SynchronousMachine> GetDERsForGlobalID(long gid)
        {
            // Povratna vrednost - lista DER-ova
            List<SynchronousMachine> ders = new List<SynchronousMachine>();

            // U zavisnosti od tipa gid-a, pribavljaju se sinhrone masine
            switch (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(gid)))
            {
                case DMSType.REGION:
                case DMSType.SUBREGION:
                case DMSType.SUBSTATION:
                    ders = adapter.GetDERs(gid);
                    break;
                case DMSType.SYNCMACHINE:
                    ders.Add(CalculationEngineModel.Instance.DersOriginal.Where(o => o.GlobalId.Equals(gid)).FirstOrDefault());
                    break;
            }

            return ders;
        }

        /// <summary>
        /// Dobijanje SCADA podataka, smestenih u SmartCache za list-u DER-ova
        /// Pribavljaju se samo podaci odredjenog tipa
        /// </summary>
        /// <param name="ders"> Lista DER-ova</param>
        /// <param name="type"> Power Type </param>
        /// <returns></returns>
        private List<AnalogValue> GetSCADADataForDERs(List<SynchronousMachine> ders, PowerType type)
        {
            // Instanca CacheObject-a
            CacheObject cacheObject = SmartCache.Instance.Cache.CacheList.FirstOrDefault();

            // Povratna vrednost
            List<AnalogValue> currentMeasurement = new List<AnalogValue>();

            // Ako je Cache prazan onda nesto nije u redu sa SCADA-om, i proracun nije moguc
            if (cacheObject == null)
            {
                throw new Exception("Cache is empty.");
            }

            foreach (SynchronousMachine der in ders)
            {
                // Upit -> prikupljanje svih merenja za odredjeni DER sa odredjenim Power Type-om
                List<object> measurements = cacheObject.Measurements.Where(o => ((AnalogValue)o).SynchronousMachine.Equals(der.GlobalId) &&
                                                                                ((AnalogValue)o).PowerType.Equals(type)).ToList();

                // Moze DER da nema analognih tacaka, tada se prelazi na drugi DER
                if (measurements == null)
                {
                    continue;
                }

                // Konverzija object -> AnalogValue
                foreach (AnalogValue analogValue in measurements)
                {
                    currentMeasurement.Add(analogValue);
                }
            }

            return currentMeasurement;
        }

        /// <summary>
        /// Pomocna funkcija koja za listu DER-ova dobavlja ForecastObject-e
        /// </summary>
        /// <param name="ders"> Lista DER-ova</param>
        /// <returns></returns>
        private List<ForecastObject> GetPowerForecastForDERs(List<SynchronousMachine> ders)
        {
            // Povratna vrednost lista ForecastObject
            List<ForecastObject> returnValue = new List<ForecastObject>();

            // Prolazi se kroz DER-ove i dobavlja se forecst proracun
            foreach (SynchronousMachine der in ders)
            {
                // Prognoza sa rezoluciom od sat vremena za zeljeni der
                ForecastObject forecastObject = forecast.HourlyForecastForDer(der.GlobalId);

                // Ukoliko je nema proracuna onda je problem sa Forecast servisom
                if (forecastObject == null)
                {
                    throw new Exception("Forecast is not available");
                }

                // Ukoliko ima, onda ga ubacujemo u listu povratnih vrednosti
                returnValue.Add(forecastObject);
            }

            return returnValue;
        }

        /// <summary>
        /// Pomocna funkcija koja radi uzimanje forecast proracuna samo za interval koji je od interesa i vrsi grupisanje po vremenu
        /// </summary>
        /// <param name="duration"> Vreme koje nam je potrebno</param>
        /// <param name="type"> Power Type </param>
        /// <param name="forecastObjects"> List-a ForecastObject-a</param>
        /// <returns> Grupisana i filtrirana predvidjanja po vremenu </returns>
        private SortedDictionary<long, List<AnalogValue>> GroupAndFilterByTime(long duration, PowerType type, List<ForecastObject> forecastObjects)
        {
            // Trenutno vreme
            DateTime current = DateTime.Now;

            // Pocetak od kada nam treba forecast proracun
            DateTime startTime = new DateTime(current.Year, current.Month, current.Day, current.Hour, 0, 0);

            // Povratna vrednost
            SortedDictionary<long, List<AnalogValue>> retValue = new SortedDictionary<long, List<AnalogValue>>();

            // Pomocna promenjiva koja se uvecava za jedan sat u svakoj iteraciji
            DateTime temp = startTime;

            for (int i = 0; i < duration; i++)
            {
                temp = temp.AddHours(1);

                List<AnalogValue> forecastMeasurement = new List<AnalogValue>();

                // Prolazi se kroz forecast objekte i na osnovu power type uzima se vrednost za prvi Date time i tako dog se ne dodje do kraja duration-a
                foreach (ForecastObject forecastObject in forecastObjects)
                {
                    if (type.Equals(PowerType.Active))
                    {
                        forecastMeasurement.AddRange(forecastObject.HourlyP.Where(o => o.Timestamp.Equals(temp.Ticks)).ToList());
                    }
                    else
                    {
                        forecastMeasurement.AddRange(forecastObject.HourlyQ.Where(o => o.Timestamp.Equals(temp.Ticks)).ToList());
                    }
                }

                retValue.Add(temp.Ticks, forecastMeasurement);
            }

            return retValue;
        }

        /// <summary>
        /// Dobijanje set point-a za odredjenu komandu i merenja
        /// </summary>
        /// <param name="command"> Komanda</param>
        /// <param name="measurement"> Merenja sa SCADA-a ili forecast vrednosti </param>
        /// <returns></returns>
        private Setpoint GetSetPoint(Command command, List<AnalogValue> measurement)
        {
            Setpoint setPoint = null;

            switch (command.OptimizationType)
            {
                case OptimizationType.DerFlexibility:
                    // SetPoint za optimizacioni metod raspodela prema raspolozivoj rezervi
                    setPoint = AvailableReserveDistribution(command.GlobalId, command.DemandedPower, command.PowerType, measurement);
                    break;
                case OptimizationType.NominalPower:
                    // SetPoint za optimizacioni metod raspodela prema nominalnoj snazi
                    setPoint = NominalPowerDistribution(command.GlobalId, command.DemandedPower, command.PowerType, measurement);
                    break;
            }

            return setPoint;
        }

        #endregion Helper functions for DistributePowerClient


    }
}
