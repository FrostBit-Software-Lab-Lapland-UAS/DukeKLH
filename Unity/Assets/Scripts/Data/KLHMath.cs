/******************************************************************************
 * File        : KLHMath.cs
 * Version     : 1.0
 * Author      : Miika Puljujärvi (miika.puljujarvi@lapinamk.fi)
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

using UnityEngine;
using System;


namespace DUKE.KLHData {

    /// <summary>
    /// Intented utility of a <typeparamref name="Building"/>.
    /// </summary>
    public enum BuildingType {
        DetachedHome,
        ApartmentBuilding,
        OfficeBuilding
    }

    /// <summary>
    /// Structure composition of a <typeparamref name="Building"/>.
    /// </summary>
    public enum StructureComposition {
        Light,
        Medium,
        Heavy
    }

    /// <summary>
    /// Defines the area within which the annual average temperature is approximately the same.
    /// </summary>
    public enum BuildingZone {
        I,
        II,
        III,
        IV
    }




    /// <summary>
    /// Provides calculation formulas to be used in KLH data visualisation.
    /// </summary>
    [CreateAssetMenu(fileName = "KLHMath", menuName = "ScriptableObjects/Create KLHMath Instance", order = 0)]
    public class KLHMath : ScriptableObject {

        #region Variables

        private static KLHMath current;
       
        #region Constant Value Variables

        /// <summary>
        /// Air density in kg/m3.
        /// </summary>
        public const float airDensity = 1.2f;                  /// kg/m3
        
        /// <summary>
        /// Air specific heat capacity in J/kg, Celsius.
        /// </summary>
        public const int airSpecificHeatCapacity = 1006;       /// J/kg,Celsius
        
        /// <summary>
        /// Water density in kg/m3.
        /// </summary>
        public const float waterDensity = 1000f;               /// kg/m3
        
        /// <summary>
        /// Water specific heat capacity in kJ/kg, Celsius.
        /// </summary>
        public const float waterSpecificHeatCapacity = 4.2f;   /// kJ/kg,Celsius
        
        /// <summary>
        /// Heated water temperature in Celsius.
        /// </summary>
        public const float heatedWaterTemperature = 55.0f;     /// Celsius
        
        /// <summary>
        /// Cold water temperature in Celsius.
        /// </summary>
        public const float coldWaterTemperature = 5.0f;        /// Celsius
        
        /// <summary>
        /// Indoor temperature in Celsius.
        /// </summary>
        public const float indoorTemperature = 21.0f;          /// Celsius
        
        /// <summary>
        /// Utilisation period of maximum load in h/a.
        /// </summary>
        public const float utilisationPeriodOfMaximumLoad = 2500;
        
        /// <summary>
        /// Building's floor height in millimetres.
        /// </summary>
        public const float floorHeight = 2.800f;

        #endregion
        #region Electricity and District Variables

        [SerializeField] Vector2 electricityCostRange;
        [SerializeField] Vector2 districtHeatingCostRange;
        [Space(10f), SerializeField] float electricityCost;
        [SerializeField] float districtHeatingCost;

        [Space(20f)]
        [SerializeField] Vector2 electricityCO2Range;
        [SerializeField] Vector2 districtHeatingCO2Range;
        [Space(10f), SerializeField] float electricityCO2;
        [SerializeField] float districtHeatingCO2;

        #endregion

        #endregion


        #region Properties
        /// <summary>
        /// Public static instance of <typeparamref name="KLHMath"/>.
        /// </summary> 
        public static KLHMath Current 
        { 
            get { return GetInstance(); } 
        }

        /// <summary>
        /// Minimum and maximum cost of electricity in €/kWh.
        /// </summary>
        public static Vector2 ElectricityCostRange 
        {
            get { return Current.electricityCostRange; }
            set { Current.electricityCostRange = value; } 
        }

        /// <summary>
        /// Minimum and maximum CO2 pollution of electricity in g/kWh.
        /// </summary>
        public static Vector2 ElectricityCO2Range 
        {
            get { return Current.electricityCO2Range; }
            set { Current.electricityCO2Range = value; } 
        }


        /// <summary>
        /// Current cost of electricity in €/kWh.
        /// </summary>
        public static float ElectricityCost 
        {
            get { return Current.electricityCost; }
            set { Current.electricityCost = value; } 
        }


        /// <summary>
        /// Current CO2 pollution of electricity in g/kWh.
        /// </summary>
        public static float ElectricityCO2EmssionFactor 
        { 
            get { return Current.electricityCO2; }
            set { Current.electricityCO2 = value; } 
        }

        /// <summary>
        /// Minimum and maximum cost of district heating in €/kWh.
        /// </summary>
        public static Vector2 DistrictHeatingCostRange 
        {
            get { return Current.districtHeatingCostRange; }
            set { Current.districtHeatingCostRange = value; }
        }


        /// <summary>
        /// Minimum and maximum CO2 pollution of district heating in g/kWh.
        /// </summary>
        public static Vector2 DistrictHeatingCO2Range 
        {
            get { return Current.districtHeatingCO2Range; }
            set { Current.districtHeatingCO2Range = value; } 
        }


        /// <summary>
        /// Current cost of district heating in €/kWh.
        /// </summary>
        public static float DistrictHeatingCost 
        {
            get { return Current.districtHeatingCost; }
            set { Current.districtHeatingCost = value; } 
        }


        /// <summary>
        /// /// Current CO2 pollution of district heating in g/kWh.
        /// </summary>
        public static float DistrictHeatingCO2EmissionFactor 
        {
            get { return Current.districtHeatingCO2; }
            set { Current.districtHeatingCO2 = value; } 
        }

        #endregion


        #region Events

        /// <summary>
        /// Called when electricity cost changes.
        /// </summary>
        public static Action ElectricityCostChanged;
       
        /// <summary>
        /// Called when district heating cost changes.
        /// </summary>
        public static Action DistrictHeatingCostChanged;
        
        /// <summary>
        /// Called when electricity CO2 value changes.
        /// </summary>
        public static Action ElectricityCO2Changed;
        
        /// <summary>
        /// Called when district heating value changes.
        /// </summary>
        public static Action DistrictHeatingCO2Changed;

        #endregion


        #region Methods

        #region Zone and Era Calculation

        /// <summary>
        /// Estimate the average outdoor temperature of the area the building is situated in.
        /// Values are from Kaukolämmön Käsikirja p. 52 table 3.1.
        /// </summary>
        /// <param name="_zone">Building Zone (I-IV) which defines the average temperature.</param>
        /// <returns>Average temperature of the zone.</returns>
        public static float AverageTemperatureOfZone (BuildingZone _zone)
        {
            switch (_zone) 
            {
                default:
                case BuildingZone.IV: return 0f;
                case BuildingZone.III: return 2f;
                case BuildingZone.II: return 4f;
                case BuildingZone.I: return 5f;
            }
        }   
        
        /// <summary>
        /// Estimate the average U-value of the building based on construction year and surface areas.
        /// </summary>
        /// <param name="_era">The year of construction.</param>
        /// <param name="_floorArea">Area of the bottom level floor in square metres.</param>
        /// <param name="_roofArea">Area of the roof in square metres.</param>
        /// <param name="_wallArea">Area of the wall surfaces in square metres.</param>
        /// <returns>Average U-value of the building in W/m2K.</returns>
        public static float UValueAverageOfBuildingEra (int _era, int _floorArea, int _roofArea, int _wallArea)
        {
            float[] uValues = UValuesByYear(_era);

            float wallAndWindowU = uValues[0];
            float floorU = uValues[1];
            float roofU = uValues[2];

            float totalArea = _roofArea + _floorArea + _wallArea;
            float wallRatio = _wallArea / totalArea;
            float roofRatio = _roofArea / totalArea;
            float floorRatio = _floorArea / totalArea;

            return wallRatio * wallAndWindowU + roofRatio * roofU + floorRatio * floorU;
        }
        
        /// <summary>
        /// Get the maximal allowed U-value of a given year.
        /// </summary>
        /// <param name="_era">The year of construction.</param>
        /// <param name="_windowRatio">The ratio of window surface to wall surface (default 20% = 0.2f).</param>
        /// <returns>Float[3] { floor, roof, wallAndWindow }.</returns>
        public static float[] UValuesByYear (int _era, float _windowRatio = 0.2f)
        {
            float wallU;
            float floorU;
            float roofU;
            float windowU;


            switch (_era) 
            {
                case 0:
                    // -1975
                    wallU = 0.81f;
                    floorU = 0.47f;
                    roofU = 0.47f;
                    windowU = 2.8f;
                    break;

                case 1:
                    // 1976-1977
                    wallU = 0.4f;
                    floorU = 0.4f;
                    roofU = 0.35f;
                    windowU = 2.1f;
                    break;

                case 2:
                    // 1978-1984
                    wallU = 0.35f;
                    floorU = 0.4f;
                    roofU = 0.29f;
                    windowU = 2.1f;
                    break;

                case 3:
                    // 1985-2002
                    wallU = 0.28f;
                    floorU = 0.36f;
                    roofU = 0.22f;
                    windowU = 1.4f;
                    break;

                case 4:
                    // 2003-2007 
                    wallU = 0.24f;
                    floorU = 0.24f;
                    roofU = 0.15f;
                    windowU = 1.4f;
                    break;

                case 5:
                    // 2008-2009 
                    wallU = 0.24f;
                    floorU = 0.24f;
                    roofU = 0.15f;
                    windowU = 1.4f;
                    break;

                default:
                case 6:
                    // 2010-
                    wallU = 0.17f;
                    floorU = 0.16f;
                    roofU = 0.09f;
                    windowU = 1f;
                    break;
            }


            /// Estimated 20% window area out of total wall area.
            float wallAndWindowU = (1f - _windowRatio) * wallU + _windowRatio * windowU;

            return new float[3] { floorU, roofU, wallAndWindowU };
        }
        
        /// <summary>
        /// Calculate the difference of indoor and average outdoor temperature.
        /// </summary>
        /// <param name="_inTemp">Indoor temperature in Celcius.</param>
        /// <param name="_avgOutTemp">Average outdoor temperature over the measured timespan in Celcius.</param>
        /// <returns>Temperature difference in Celcius.</returns>
        public static float TemperatureDifference ( float _inTemp, float _avgOutTemp)
        {
            return _inTemp - _avgOutTemp;
        }
        public static float TemperatureDifference (BuildingZone _zone)
        {
            return TemperatureDifference(indoorTemperature, AverageTemperatureOfZone(_zone));
        }
    
        /// <summary>
    /// Get the era a <paramref name="_year"/> is in.
    /// </summary>
    /// <param name="_year"></param>
    /// <returns>The era to which the specified <paramref name="_year"/> belongs to.</returns>
        public static int YearToEra (int _year)
        {
            return _year switch
            {
                < 1976 => 0,
                < 1978 => 1,
                < 1985 => 2,
                < 2003 => 3,
                < 2008 => 4,
                < 2010 => 5,
                >= 2010 => 6
            };
        }
        
        /// <summary>
        /// Get the first year of an <paramref name="_era"/>.
        /// </summary>
        /// <param name="_era"></param>
        /// <returns>The first year of the specified <param name="_era"/>.</returns>
        public static int EraToYEar (int _era)
        {
            return _era switch 
            {
                0 => 1975,
                1 => 1976,
                2 => 1978,
                3 => 1985,
                4 => 2003,
                5 => 2008,
                _ => 2010
            };
        }

        #endregion
        #region Cost and CO2 Values

        /// <summary>
        /// Get electricity cost within range.
        /// </summary>
        /// <param name="_ratio">Ratio between min and max.</param>
        /// <returns>Current electricity cost.</returns>
        public static float GetElectricityCost(float _ratio)
        {
            ElectricityCost = Mathf.Lerp(ElectricityCostRange.x, ElectricityCostRange.y, _ratio);
            ElectricityCostChanged?.Invoke();
            return ElectricityCost;
        }

        /// <summary>
        /// Get district heating cost within range.
        /// </summary>
        /// <param name="_ratio">Ratio between min and max.</param>
        /// <returns>Current district heating cost.</returns>
        public static float GetDistrictHeatingCost(float _ratio)
        {
            DistrictHeatingCost = Mathf.Lerp(DistrictHeatingCostRange.x, DistrictHeatingCostRange.y, _ratio);
            DistrictHeatingCostChanged?.Invoke();
            return DistrictHeatingCost;
        }

        /// <summary>
        /// Get electricity CO2 amount within range.
        /// </summary>
        /// <param name="_ratio">Ratio between min and max.</param>
        /// <returns>Current CO2 emission of electricity.</returns>
        public static float GetElectricityCo2EmissionFactor(float _ratio)
        {
            ElectricityCO2EmssionFactor = Mathf.Lerp(ElectricityCO2Range.x, ElectricityCO2Range.y, _ratio);
            ElectricityCO2Changed?.Invoke();
            return ElectricityCO2EmssionFactor;
        }

        /// <summary>
        /// Get district heating CO2 amount within range.
        /// </summary>
        /// <param name="_ratio">Ratio between min and max.</param>
        /// <returns>Current CO2 emission of district heating.</returns>
        public static float GetDistrictHeatingCo2(float _ratio)
        {
            DistrictHeatingCO2EmissionFactor = Mathf.Lerp(DistrictHeatingCO2Range.x, DistrictHeatingCO2Range.y, _ratio);
            DistrictHeatingCO2Changed?.Invoke();
            return DistrictHeatingCO2EmissionFactor;
        }

        #endregion
        #region Heat Loss Calculation

        /// <summary>
        /// Calculate the total energy needed to cover the heat loss through conduction through the building envelope.
        /// </summary>
        /// <param name="_U">The average value of the surface's heat condution in W/m2K.</param>
        /// <param name="_wallArea">Wall area in square metres.</param>
        /// <param name="_floorArea">Roof / floor area in square metres.</param>
        /// <param name="_tempDiff">The difference between indoor temperature and average outside temperature of the measured timespan in Celcius.</param>
        /// <param name="_durationInHours">The duration of the measured timespan in hours (default 8760 = one year).</param>
        /// <returns>Estimated heat loss through conduction over the measured timespan in kWh.</returns>
        public static int HeatLossThroughConduction (float _U, float _wallArea, float _floorArea, float _tempDiff, int _durationInHours = 8760)
        {
            float wallHeatLoss = _U * _wallArea * _tempDiff * _durationInHours / 1000f;
            float roofHeatLoss = _U * _floorArea * _tempDiff * _durationInHours / 1000f;
            float floorHeatLoss = _U * _floorArea * (_tempDiff - 2f) * _durationInHours / 1000f;

            return Mathf.RoundToInt(wallHeatLoss + roofHeatLoss + floorHeatLoss);
        }
        
        /// <summary>
        /// Calculate the total energy needed to cover the heat loss through conduction through the building envelope.
        /// </summary>
        /// <param name="_wallU">Average value of the wall surface's heat conduction in W/m2K.</param>
        /// <param name="_floorU">Average value of the floor surface's heat conduction in W/m2K.</param>
        /// <param name="_roofU">Average value of the roof surface's heat conduction in W/m2K.</param>
        /// <param name="_wallArea">Wall area in square metres.</param>
        /// <param name="_floorArea">Roof / floor area in square metres.</param>
        /// <param name="_tempDiff">The difference between indoor temperature and average outside temperature of the measured timespan in Celcius.</param>
        /// <param name="_durationInHours">The duration of the measured timespan in hours (default 8760 = one year).</param>
        /// <returns>Estimated heat loss through conduction over the measured timespan in kWh.</returns>
        public static int HeatLossThroughConduction (float _floorU, float _roofU, float _wallU, float _wallArea, float _floorArea, 
            float _tempDiff, int _durationInHours = 8760)
        {
            float floorHeatLoss = _floorU * _floorArea * (_tempDiff - 2f) * _durationInHours / 1000f;
            float roofHeatLoss = _roofU * _floorArea * _tempDiff * _durationInHours / 1000f;
            float wallHeatLoss = _wallU * _wallArea * _tempDiff * _durationInHours / 1000f;

            return Mathf.RoundToInt(floorHeatLoss + roofHeatLoss + wallHeatLoss);
        }
        
        /// <summary>
        /// Calculate the total energy needed to heat the incoming air through ventilation.
        /// </summary>
        /// <param name="_grossBuildingVolume">Gross volume of the building in cubic metres.</param>
        /// <param name="_activityRatioPerDay">A ratio between 0...1 that represents the daily activity of the system.</param>
        /// <param name="_activityRatioPerWeek">A ratio between 0...1 that represents the weekly activity of the system.</param>
        /// <param name="_operatingEfficiencyOfHeatRecovery">A ratio between 0...1 that represents the coefficiency of the heat recovery (0.25 means the system has a coefficiency of 25%).</param>
        /// <param name="_tempDiff">The difference between indoor temperature and average outside temperature of the measured timespan in Celcius.</param>
        /// <param name="_durationInHours">The duration of the measured timespan in hours (default 8760 = one year).</param>
        /// <returns>Estimated energy needed to cover the heat loss through ventilation over the measured timespan in kWh.</returns>
        public static int HeatLossThroughVentilation (int _grossBuildingVolume, float _activityRatioPerDay, float _activityRatioPerWeek, 
            float _operatingEfficiencyOfHeatRecovery, float _tempDiff, int _durationInHours = 8760)
        {
            return Mathf.RoundToInt(airDensity * airSpecificHeatCapacity * GetEstimatedAirflow(_grossBuildingVolume) * _activityRatioPerDay
                * _activityRatioPerWeek * (1 - _operatingEfficiencyOfHeatRecovery) * _tempDiff * _durationInHours / 1000f);
        }
        
        /// <summary>
        /// Calculate the total energy needed to heat the incoming air through gaps in the building envelope.
        /// </summary>
        /// <param name="_airReplacementRate">The rate at which the air is completely replaced in a space (1/h).</param>
        /// <param name="_grossBuildingVolume">The gross volume of the building in cubic metres.</param>
        /// <param name="_tempDiff">The difference between indoor temperature and average outside temperature of the measured timespan in Celcius.</param>
        /// <param name="_durationInHours">The duration of the measured timespan in hours (default 8760 = one year).</param>
        /// <returns>Estimated energy neede dto cover the heat loss through replacement air over the measured timespan in kWh.</returns>
        public static int HeatLossThroughReplacementAir (int _grossBuildingVolume, float _tempDiff, float _airReplacementRate = 0.1f, int _durationInHours = 8760)
        {
            return Mathf.RoundToInt(airDensity * airSpecificHeatCapacity * _airReplacementRate * _grossBuildingVolume * _tempDiff
                * _durationInHours / (3.6f * 1000000));
        }
        
        /// <summary>
        /// Calculate the total energy needed to heat the water used by the residents.
        /// </summary>
        /// <param name="_heatedWaterConsumption"></param>
        /// <returns>Total energy needed to heat the water for the building's residents in kWh.</returns>
        public static int HeatLossThroughWaterHeating (float _heatedWaterConsumption)
        {
            return Mathf.RoundToInt(waterDensity * waterSpecificHeatCapacity * _heatedWaterConsumption 
                * (heatedWaterTemperature - coldWaterTemperature) / 3600);
        }
        
        /// <summary>
        /// Calculate the estimated consumption of heated water based on gross building area.
        /// </summary>
        /// <param name="_grossBuildingArea">Gross area of the building in square metres.</param>
        /// <param name="_waterUsage">Area-multiplied water usage of a building in litres.</param>
        /// <returns>Estimated consumption of heated water over the measured timespan in cubic metres.</returns>
        public static float ConsumptionOfHeatedWaterByGrossArea (int _grossBuildingArea, int _waterUsage = 600)
        {
            return _grossBuildingArea * _waterUsage / 1000f;
        }
        
        /// <summary>
        /// Calculate the estimated consumption of heated water based on the number of residents.
        /// </summary>
        /// <param name="_numberOfResidents">The number of residents living in the building.</param>
        /// <param name="_waterUsage">The amount used by each person in litres per day.</param>
        /// <param name="_durationInDays">The duration of the measured timespan in days (default 365 = one year).</param>
        /// <returns>Estimated consumption of heated water over the measured timespan in cubic metres.</returns>
        public static float ConsumptionOfHeatedWaterByResidentCount (int _numberOfResidents, int _waterUsage = 50, int _durationInDays = 365)
        {
            return _numberOfResidents * _waterUsage * _durationInDays / 1000;
        }
        
        /// <summary>
        /// USE HeatGainFromTotalHeatLoad INSTEAD!
        /// </summary>
        /// <param name="_grossBuildingArea">Gross area of the building in square metres.</param>
        /// <param name="_heatFromSun">Heat gain from sun in kWh.</param>
        /// <param name="_heatFromWaterUsage">Heat gain from heated water usage in kWh.</param>
        /// <param name="_heatFromHeatingSystemUsage">Heat gain from heating systems (indirect) in kWh.</param>
        /// <returns>Estimated energy gain</returns>
        public static int HeatGainFromInternalSourcesAndSun (int _grossBuildingArea, int _heatFromSun, int _heatFromWaterUsage = 0, int _heatFromHeatingSystemUsage = 0)
        {
            int gainFromDevicesAndLighting = GetHeatGainFromDevicesAndLighting(_grossBuildingArea);
            int gainFromResidents = GetHeatGainFromResidentsByGrossArea(_grossBuildingArea);

            return gainFromDevicesAndLighting + gainFromResidents + _heatFromSun;
        }
        
        /// <summary>
        /// Calculate the specific heat gain of a building. The total specific heat gain is a ratio of the total heat load generated by different sources.
        /// </summary>
        /// <param name="_grossBuildingArea">Gross area of the building in square metres.</param>
        /// <param name="_heatLossThroughConduction">The amount of energy needed to replace the heat lost by conduction through the building envelope.</param>
        /// <param name="_tempDiff">The difference between indoor temperature and average outside temperature of the measured timespan in Celcius.</param>
        /// <param name="_effectiveHeatCapacityOfBuilding"></param>
        /// <param name="_dutationInHours"></param>
        /// <returns></returns>
        public static int HeatGainFromTotalHeatLoad (int _grossBuildingArea, int _heatLossThroughConduction, int _heatLossThroughVentilation, 
            int _heatLossThroughReplacementAir, float _tempDiff, StructureComposition _composition = StructureComposition.Medium, int _dutationInHours = 8760)
        {
            if (_heatLossThroughConduction == 0) { return 0; }

            int totalHeatLoad = GetTotalHeatLoad(_grossBuildingArea, _dutationInHours);
            int totalHeatLoss = (_heatLossThroughConduction + _heatLossThroughVentilation + _heatLossThroughReplacementAir);
            
            /// Calculate the specific heat loss of the building.
            float hSpace = totalHeatLoss / (_tempDiff * _dutationInHours) / 1000f;

            /// Calculate the time constant 'tau' and numeric parameter 'a'.
            float tau = GetEffectiveHeatCapacityOfBuilding(_composition, BuildingType.ApartmentBuilding) / hSpace;
            float a = 1 + tau / 15;
        
            /// Calculate the ratio 'gamma'.
            float gamma = (float)totalHeatLoad / totalHeatLoss;

            /// Calculate the ratio of 'etaHeat'.
            float etaComponentA = (1 - Mathf.Pow(gamma, a));
            float etaComponentB = (1 - Mathf.Pow(gamma, a + 1));
            float etaHeat = (gamma >= 1f)
                ? a / (a + 1)
                : (float)(etaComponentA / etaComponentB);
        
            int result = Mathf.RoundToInt(Mathf.Max(etaHeat * totalHeatLoad, 0));

            return result;
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Get a static instance of <typeparamref name="KLHMath"/>.
        /// </summary>
        /// <returns>Static instance of <typeparamref name="KLHMath"/>.</returns>
        protected static KLHMath GetInstance ()
        {
            if (null == current) 
            { 
                current = Resources.Load("ScriptableObjects/KLHMath") as KLHMath; 
            }

            return current;
        }

        /// <summary>
        /// Calulate the estimated airflow (m3/s) of the building.
        /// </summary>
        /// <param name="_grossBuildingVolume">Gross volume of the building in cubic metres.</param>
        /// <returns>Estimated airflow of the building in cubic metres per second.</returns>
        protected static float GetEstimatedAirflow (int _grossBuildingVolume)
        {
            float cubicMetersPerSecond = _grossBuildingVolume / 2f / 3600f;
            
            return cubicMetersPerSecond;
        }
        
        /// <summary>
        /// Calculate the estimated heat gain from electrical devices and lighting.
        /// </summary>
        /// <param name="_grossBuildingArea">Gross area of the building in square metres.</param>
        /// <returns>Heat gained from devices and lighting (kWh).</returns>
        protected static int GetHeatGainFromDevicesAndLighting (int _grossBuildingArea)
        {
            float gainFromDevices = 0.009f * _grossBuildingArea * 365 * 0.1f;
            float gainFromLighting = 0.004f * _grossBuildingArea * 365 * 0.6f;

            return Mathf.RoundToInt(gainFromDevices + gainFromLighting);
        }
        
        /// <summary>
        /// Get the total heat gain from residents calculated from total gross area of the building.
        /// </summary>
        /// <param name="_grossBuildingArea">Gross area of the building in square metres.</param>
        /// <returns>Estimated annual heat gain from residents in kWh.</returns>
        protected static int GetHeatGainFromResidentsByGrossArea (int _grossBuildingArea)
        {
            return Mathf.RoundToInt(0.003f * _grossBuildingArea * 365 * 0.6f);
        }
        
        /// <summary>
        /// Get the total heat gain from residents calculated from the number of residents.
        /// </summary>
        /// <param name="_numberOfResidents"></param>
        /// <returns>Estimated annual heat gain from residents in kWh.</returns>
        protected static int GetHeatGainFromResidentsByResidentCount (int _numberOfResidents)
        {
            return Mathf.RoundToInt(0.085f * _numberOfResidents * 365);
        }
        
        /// <summary>
        /// Calculate the total heat load of a building. 
        /// </summary>
        /// <param name="_grossBuildingArea">Gross area of the building in square metres.</param>
        /// <param name="_durationInHours">The duration of the measured timespan in hours (default 8760 = one year).</param>
        /// <returns>Total heat load over the measured timespan in kWh.</returns>
        protected static int GetTotalHeatLoad (int _grossBuildingArea, int _durationInHours = 8760)
        {
            float heatLoadOfElectricalEquipment = 80 * _grossBuildingArea;
            float heatLoadOfResidents = 17 * _grossBuildingArea;
            float heatLoadOfHeatedWater = 15 * _grossBuildingArea;

            return Mathf.RoundToInt((heatLoadOfElectricalEquipment + heatLoadOfResidents + heatLoadOfHeatedWater) * (_durationInHours / 8760));
        }
        
        /// <summary>
        /// Get the estimated heat capacity of the building.
        /// </summary>
        /// <param name="_composition">Composition of the structure, defined as light, medium or heavy.</param>
        /// <param name="_type">Type of the building, defined as detached home, apartment building or office building.</param>
        /// <returns>Estimated heat capacity of the building.</returns>
        protected static int GetEffectiveHeatCapacityOfBuilding (StructureComposition _composition, BuildingType _type)
        {
            switch (_type) 
            {
                case BuildingType.DetachedHome:

                    switch (_composition) 
                    {
                        default:
                        case StructureComposition.Light: return 40;
                        case StructureComposition.Medium: return 90;
                        case StructureComposition.Heavy: return 200;
                    }

                default:
                case BuildingType.ApartmentBuilding:

                    switch (_composition) 
                    {
                        default:
                        case StructureComposition.Light: return 40;
                        case StructureComposition.Medium: return 160;
                        case StructureComposition.Heavy: return 220;
                    }

                case BuildingType.OfficeBuilding:

                    switch (_composition) 
                    {
                        default:
                        case StructureComposition.Light: return 70;
                        case StructureComposition.Medium: return 110;
                        case StructureComposition.Heavy: return 160;
                    }
            }
        }

        #endregion
        #region General Math Functions

        /// <summary>
        /// Convert a float value to percentage value.
        /// </summary>
        /// <param name="_value">The value to be converted. Value of 1 will become 100.0 and 0.275 will become 27.5.</param>
        /// <param name="_decimals">The amount of decimal numbers to include.</param>
        /// <returns>Percentage value.</returns>
        public static float GetPercent (float _value, int _decimals)
        {
            float power = Mathf.Pow(10,_decimals);
            
            return Mathf.FloorToInt(_value * 100f * power) / power;
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace