/******************************************************************************
 * File        : Building.cs
 * Version     : 1.0
 * Author      : Miika Puljujärvi (miika.puljujarvi@lapinamk.fi), Petteri Maljamäki (petteri.maljamaki@lapinamk.fi)
 * Copyright   : Lapland University of Applied Sciences
 * Licence     : MIT-Licence
 * 
 * Copyright (c) 2022 Lapland University of Applied Sciences
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 *****************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DUKE.KLHData {


    /// <summary>
    /// A collection of information about a specifi building.
    /// </summary>
    [CreateAssetMenu(fileName = "New Building", menuName = "ScriptableObjects/Create a new Building", order = 0)]
    public class Building : ScriptableObject
    {
        #region Variables

        #region General Variables

        [SerializeField] string buildingName = "New Building";
        [SerializeField] int constructionYear = 1959;
        [SerializeField] BuildingZone buildingZone = BuildingZone.IV;
        [SerializeField, HideInInspector] DrawGrid drawGrid;
        [SerializeField] HeatingMethod[] heatingDevices;
        [SerializeField] int staircaseCount = 1;
        [SerializeField] int highestLevel;
        [SerializeField] int totalHeatConsumption = -1;

        #endregion
        #region Construction Variables

        [SerializeField] int grossArea;
        [SerializeField] int grossVolume;
        [SerializeField] int envelopeArea;
        [SerializeField] int floorArea;
        [SerializeField] int wallArea;
        [SerializeField] float floorU = -1;
        [SerializeField] float roofU = -1;
        [SerializeField] float wallAndWindowU = -1;

        #endregion
        #region Debug Variables

        #if UNITY_EDITOR
        [SerializeField] bool logCalculationValues = false;
        #endif

        #endregion

        #endregion


        #region Properties

        /// <summary>
        /// Name of the <typeparamref name="Building"/>. This property is currently unused.
        /// </summary>
        public string Name 
        { 
            get { return buildingName; } 
        }
        
        /// <summary>
        /// Construction era of the <typeparamref name="Building"/>.
        /// </summary>
        public int ConstructionEra 
        { 
            get { return KLHMath.YearToEra(constructionYear); } 
        }
        
        /// <summary>
        /// Construction year of the <typeparamref name="Building"/>.
        /// </summary>
        public int ConstructionYear 
        { 
            get { return constructionYear; } 
            set { constructionYear = value; } 
        }
        

        /// <summary>
        /// <typeparamref name="BuildingZone"/> of the <typeparamref name="Building"/>.
        /// </summary>
        public BuildingZone Zone 
        { 
            get { return buildingZone; } 
            set { buildingZone = value; } 
        }
        

        /// <summary>
        /// <typeparamref name="DrawGrid"/> instance containing the shape of the <typeparamref name="Building"/>.
        /// </summary>
        public DrawGrid DrawGrid 
        { 
            get { return drawGrid; } 
            set { drawGrid = value; } 
        }
        
        /// <summary>
        /// TRUE if heat loss should be calculated, FALSE if it should be set instead.
        /// </summary>
        public bool CalculateHeatLoss 
        { 
            get { return totalHeatConsumption == -1; } 
        }
        
        /// <summary>
        /// <typeparamref name="HeatingMethod"/> array containing <typeparamref name="DistrictHeating"/> and each type of supported hybrid heating method.
        /// </summary>
        public HeatingMethod[] HeatingMethods 
        { 
            get { return heatingDevices; } 
        }

        /// <summary>
        /// Gross floor area of the <typeparamref name="Building"/> in square meters.
        /// </summary>
        public int GrossArea 
        { 
            get { return grossArea; } 
        }

        /// <summary>
        /// Gross volume of the <typeparamref name="Building"/> in cubic meters.
        /// </summary>
        public int GrossVolume 
        { 
            get { return grossVolume; } 
        }
        
        /// <summary>
        /// Total envelope area of the <typeparamref name="Building"/> in square meters.
        /// </summary>
        public int EnvelopeArea 
        { 
            get { return envelopeArea; } 
        }
        
        /// <summary>
        /// Total area of the <typeparamref name="Building"/> when viewed directly from above.
        /// </summary>
        public int FloorArea 
        { 
            get { return floorArea; } 
        }
        
        /// <summary>
        /// Total vertical surface area of the <typeparamref name="Building"/>.
        /// </summary>
        public int WallArea 
        { 
            get { return wallArea; } 
        }
        
        /// <summary>
        /// Current highest floor count of the <typeparamref name="Building"/>.
        /// </summary>
        public int HighestLevel 
        { 
            get { return highestLevel; } 
        }
        
        /// <summary>
        /// Staircase count of the <typeparamref name="Building"/>.
        /// </summary>
        public int StaircaseCount 
        { 
            get { return staircaseCount; } 
            set { staircaseCount = value; } 
        }
        
        /// <summary>
        /// Thermal transmittance of the floor.
        /// </summary>
        public float FloorU
        { 
            get { return floorU = KLHMath.UValuesByYear(KLHMath.YearToEra(constructionYear))[0]; } 
        }
        
        /// <summary>
        /// Thermal transmittance of the roof.
        /// </summary>
        public float RoofU 
        { 
            get { return roofU = KLHMath.UValuesByYear(KLHMath.YearToEra(constructionYear))[1]; } 
        }
        
        /// <summary>
        /// Averaged thermal transmittance of walls and windows. 
        /// See <typeparamref name="DUKE"/>.<typeparamref name="KLHData"/>.<typeparamref name="KLHMath"/> for further details.
        /// </summary>
        public float WallAndWindowU 
        { 
            get { return wallAndWindowU = KLHMath.UValuesByYear(KLHMath.YearToEra(constructionYear))[2]; } 
        }
        
        /// <summary>
        /// Average temperature difference within the selected <typeparamref name="BuildingZone"/>.
        /// </summary>
        public float TempDifference 
        { 
            get { return KLHMath.TemperatureDifference(Zone); } 
        }

        /// <summary>
        /// Total annual heat consumption in kWh.
        /// </summary>
        public int TotalHeatConsumption 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// Total annual heat consumption of the due to conduction in kWh.
        /// </summary>
        public int HeatConsumptionDueToConduction 
        { 
            get; 
            protected set; 
        }
        
        /// <summary>
        /// Total annual heat consumption due to ventilation in kWh.
        /// </summary>
        public int HeatConsumptionDueToVentilation 
        { 
            get; 
            protected set;
        }
        
        /// <summary>
        /// Total annual heat consumption due to air infiltration in kWh.
        /// </summary>
        public int HeatConsumptionDueToAirInfiltration 
        { 
            get; 
            protected set; 
        }
        
        /// <summary>
        /// Total annual heat consumption of the <typeparamref name="Building"/> due to water heating in kWh.
        /// </summary>
        public int HeatConsumptionDueToWaterHeating 
        { 
            get; 
            protected set; 
        }
        
        /// <summary>
        /// Total annual heat gain of the <typeparamref name="Building"/> due to internal sources in kWh.
        /// </summary>
        public int HeatGainDueToInternalSources 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// Daily heated water consumption of the <typeparamref name="Building"/> in kWh.
        /// </summary>
        public float DailyHeatedWaterConsumption 
        { 
            get; 
            protected set; 
        }

        #endregion


        #region Methods

        /// <summary>
        /// Calculate area and volumetric measurements of the <typeparamref name="Building"/>.
        /// </summary>
        public void CalculateDimensions ()
        {
            grossArea = (int)drawGrid.CalculateGrossArea();
            grossVolume = (int)drawGrid.CalculateVolume(KLHMath.floorHeight);
            envelopeArea = (int)drawGrid.CalculateEnvelopeArea(KLHMath.floorHeight);
            floorArea = (int)drawGrid.CalculateHorizontalArea();
            wallArea = (int)drawGrid.CalculateWallArea(KLHMath.floorHeight);
            highestLevel = CalculateHighestLevel();
        }

        /// <summary>
        /// Calculate the heat loss values of the Building.
        /// </summary>
        public void CalculateHeatingValues() 
        {
            HeatConsumptionDueToConduction = GetHeatLossThroughConduction();
            HeatConsumptionDueToVentilation = GetHeatLossThroughVentilation();
            HeatConsumptionDueToAirInfiltration = GetHeatLossThroughReplacementAir();
            HeatConsumptionDueToWaterHeating = GetHeatLossThroughWaterHeating();
            HeatGainDueToInternalSources = GetHeatGainThroughInternalSources();

            TotalHeatConsumption = GetTotalHeatConsumption();

            DailyHeatedWaterConsumption = KLHMath.ConsumptionOfHeatedWaterByGrossArea(GrossArea) / 365f;

            UpdateHeatingDevices();
        }

        /// <summary>
        /// Calculate the total amount of lost heat.
        /// </summary>
        /// <returns>Lost energy in kWh.</returns>
        protected int GetTotalHeatConsumption ()
        {
            if (totalHeatConsumption > 0) { return totalHeatConsumption; }

            return HeatConsumptionDueToConduction
                + HeatConsumptionDueToVentilation
                + HeatConsumptionDueToAirInfiltration
                + HeatConsumptionDueToWaterHeating
                - HeatGainDueToInternalSources;
        }

        /// <summary>
        /// Calculate the heat lost through conduction of the structure.
        /// </summary>
        /// <returns>Lost energy in kWh.</returns>
        protected int GetHeatLossThroughConduction ()
        {
            if (!CalculateHeatLoss) { return 0; }

            float[] uValues = KLHMath.UValuesByYear(ConstructionYear);
            floorU = uValues[0];
            roofU = uValues[1];
            wallAndWindowU = uValues[2]; 
            
            #if UNITY_EDITOR
            if (logCalculationValues) 
            {
                Debug.Log(
                    "Calculating Conduction: "
                    + "\nFloorU=" + FloorU
                    + "\nRoofU=" + RoofU
                    + "\nWallAndWindowU=" + WallAndWindowU
                    + "\nFloorArea=" + FloorArea
                    + "\nWallArea=" + WallArea
                    + "\nTempDiff=" + TempDifference
                );
            }
            #endif

            return KLHMath.HeatLossThroughConduction(FloorU, RoofU, WallAndWindowU, WallArea, FloorArea, TempDifference);
        }
        
        /// <summary>
        /// Calculate the heat lost through ventilation.
        /// </summary>
        /// <returns>Lost energy in kWh.</returns>
        protected int GetHeatLossThroughVentilation ()
        {
            float activityRatioPerDay = 1f;
            float activityRatioPerWeek = 1f;
            float recoveryUnitCoefficiency = 0f;

            #if UNITY_EDITOR
            if (logCalculationValues) 
            {
                Debug.Log(
                    "Calculating Ventilation: "
                    + "\nGrossVolume=" + GrossVolume
                    + "\nDailyActivityRatio=" + activityRatioPerDay
                    + "\nWeeklyActivityRatio=" + activityRatioPerWeek
                    + "\nUnitCoefficiency=" + recoveryUnitCoefficiency
                    + "\nTempDiff=" + TempDifference
                );
            }
            #endif

            return KLHMath.HeatLossThroughVentilation(_grossBuildingVolume: GrossVolume, activityRatioPerDay, activityRatioPerWeek, recoveryUnitCoefficiency, TempDifference);
        }

        /// <summary>
        /// Calculate the heat lost through replacement air.
        /// </summary>
        /// <returns>Lost energy in kWh.</returns>
        protected int GetHeatLossThroughReplacementAir ()
        {
            #if UNITY_EDITOR
            if (logCalculationValues) 
            {
                Debug.Log(
                    "Calculating Replacement Air: "
                    + "\nGrossVolume=" + GrossVolume
                    + "\nTempDiff=" + TempDifference);
            }
            #endif

            /// NOTE: Air replacement rate uses the default value of 0.1f.
            return KLHMath.HeatLossThroughReplacementAir(GrossVolume, TempDifference);
        }

        /// <summary>
        /// Calculate the heat lost through heating the water used by the residents.
        /// </summary>
        /// <returns>Lost energy in kWh.</returns>
        protected int GetHeatLossThroughWaterHeating ()
        {
            #if UNITY_EDITOR
            if (logCalculationValues) 
            {
                Debug.Log(
                    "Calculating Heated Water: "
                    + "\nGrossArea=" + GrossArea);
            }
            #endif

            return KLHMath.HeatLossThroughWaterHeating(KLHMath.ConsumptionOfHeatedWaterByGrossArea(GrossArea));
        }

        /// <summary>
        /// Calculate the heat gained from sun and internal sources.
        /// </summary>
        /// <returns>Gained energy in kWh.</returns>
        protected int GetHeatGainThroughInternalSources ()
        {
            #if UNITY_EDITOR
            if (logCalculationValues)
            {
                Debug.Log(
                    "Calculating Internal Sources: "
                    + "\nGrossArea=" + GrossArea
                    + "\nConsumption=" + HeatConsumptionDueToConduction
                    + "\nVentilation=" + HeatConsumptionDueToVentilation
                    + "\nReplacementAir=" + HeatConsumptionDueToAirInfiltration
                    + "\nTempDiff=" + TempDifference
                );
            }
            #endif

            /// NOTE: StructureComposition uses the default value of Medium.
            return KLHMath.HeatGainFromTotalHeatLoad(
                GrossArea,
                HeatConsumptionDueToConduction, 
                HeatConsumptionDueToVentilation,
                HeatConsumptionDueToAirInfiltration,
                TempDifference);
        }

        /// <summary>
        /// Rebuild the heating devices.
        /// </summary>
        public void UpdateHeatingDevices() 
        {
            heatingDevices = new HeatingMethod[4];
            heatingDevices[0] = Instantiate(Resources.Load("ScriptableObjects/HybridDevices/District") as DistrictHeating);
            heatingDevices[1] = Instantiate(Resources.Load("ScriptableObjects/HybridDevices/Geothermal") as GeothermalHeating);
            heatingDevices[2] = Instantiate(Resources.Load("ScriptableObjects/HybridDevices/Airsource") as AirsourceHeatPumpHeating);
            heatingDevices[3] = Instantiate(Resources.Load("ScriptableObjects/HybridDevices/HeatRecovery") as HeatRecoveryHeating);

            for (int i=0; i<heatingDevices.Length; i++) 
            {
                heatingDevices[i].UpdateValues(this);
            }
        }

        /// <summary>
        /// Calculate the highest level of the <typeparamref name="Building"/>.
        /// </summary>
        /// <returns>Current highest floor number of the building.</returns>
        protected int CalculateHighestLevel()
        {
            int max = 0;

            for (int x = 0; x < DrawGrid.Width; x++) 
            {
                for (int y = 0; y < DrawGrid.Depth; y++) 
                {
                    int val = DrawGrid.Cells[x,y].Value;
                    max = val > max ? val : max;
                }
            }

            return max;
        }

        #endregion


    } /// End of Class


} /// End of Namespace