/******************************************************************************
 * File        : HeatingMethod.cs
 * Version     : 1.0
 * Author      : Petteri Maljamäki (petteri.maljamaki@lapinamk.fi), Miika Puljujärvi (miika.puljujarvi@lapinamk.fi)
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
    /// Base class for hybrid devices that are attached to the district heating system.
    /// </summary>
    public class HeatingMethod : ScriptableObject
    {
        #region Variables

        #region Energy Variables
        
        /// <summary>
        /// Total annual heat consumption of a <typeparamref name="Building"/> in kWh.
        /// </summary>
        [SerializeField, HideInInspector] protected int totalHeatConsumption;
         
        /// <summary>
        /// Total annual amount of renewable energy used by a <typeparamref name="Building"/> in kWh.
        /// This amount is a part of <paramref name="totalHeatConsumption"/>.
        /// </summary>
        [SerializeField, HideInInspector] protected int renewableEnergyProduced;
  
        /// <summary>
        /// Cost multiplier.
        /// </summary>
        [SerializeField, HideInInspector] protected float districtHeatingK1Multiplier = 6.23f;

        /// <summary>
        /// Coefficiency of performance multiplier.
        /// </summary>
        [SerializeField] protected float copMultiplier = 3f;
        
        /// <summary>
        /// Annual maintenance cost per m2 for district heating.
        /// </summary>
        [SerializeField] protected static float maintenancePerSqMeterDH = 0.25f;
       
        /// <summary>
        /// Annual maintenance cost per m2 for renewable energy.
        /// </summary>
        [SerializeField] protected float maintenancePerSqMeterRe = 0.0f;

        /// <summary>
        /// Rate of Euros per kW.
        /// </summary>
        [SerializeField] protected AnimationCurve costPerKWCurve;
       
        /// <summary>
        /// Rate of Euros per m3.
        /// </summary>
        [SerializeField] protected AnimationCurve costPerM3Curve;

        /// <summary>
        /// Divider value is used to calculate which portion of the total power is covered with selected <typeparamref name="HeatingMethod"/>.
        /// </summary>
        protected float devicePowerDivider = 1f;

        /// <summary>
        /// Minimum cost of the <typeparamref name="HeatingMethod"/> independent of a <typeparamref name="Building"/>'s size.
        /// </summary>
        protected int deviceMinimumCost = 5000;

        #endregion
        #region Monetary Variables
  
        /// <summary>
        /// Initial investment cost of the <typeparamref name="HeatingMethod"/>.
        /// </summary>
        [SerializeField, HideInInspector] protected int initialInvestmentCost;
       
        /// <summary>
        /// Annual operating cost of district heating.
        /// </summary>
        [SerializeField, HideInInspector] protected int annualDHOperatingCost;
       
        /// <summary>
        /// Annual operating cost of the renewable heating method.
        /// </summary>
        [SerializeField, HideInInspector] protected int annualREOperatingCost;
        
        /// <summary>
        /// Annual CO2 amount of district heating.
        /// </summary>
        [SerializeField, HideInInspector] protected int annualDHCO2Amount;
          
        /// <summary>
        /// Annual CO2 amount of the renewable heating method.
        /// </summary>
        [SerializeField, HideInInspector] protected int annualRECO2Amount;

        #endregion
        
        #endregion


        #region Properties
        public Building Building            { get ; protected set; }

        /// <summary>
        /// Ratio of reneweable energy from total energy consumption.
        /// </summary>
        /// <returns>Ratio as float</returns>
        public float RenewableRatio         { get { return (float)renewableEnergyProduced/totalHeatConsumption; } }

        /// <summary>
        /// Total annual heat consumption of a building in kWh
        /// </summary>
        public int TotalHeatConsumption     { get { return totalHeatConsumption;}}

        /// <summary>
        ///  Annual district heating operating costs
        /// </summary>
        public int AnnualDHOperatingCost    { get { return annualDHOperatingCost; }}
        /// <summary>
        /// Annual renewable energy heating operating costs
        /// </summary>
        public int AnnualREOperatingCost    { get { return annualREOperatingCost; }}
        /// <summary>
        /// Total annual operating costs
        /// </summary>
        public int AnnualOperatingCost      { get { return AnnualDHOperatingCost + AnnualREOperatingCost; } }
        /// <summary>
        /// Annual district heating CO2 amount
        /// </summary>
        public int AnnualDHCO2Amount        { get { return annualDHCO2Amount; }}
        /// <summary>
        /// Annual renewable energy heating CO2 amount
        /// </summary>
        public int AnnualRECO2Amount        { get { return annualRECO2Amount; }}
        /// <summary>
        /// power of a device, calculated from total consumption divided with <typeparamref name="KLHMath"/>.<paramref name="utilisationPeriodOfMaximumLoad"/>
        /// </summary>
        /// <returns>power in kW</returns>
        public int TotalPower               { get { return (int)(TotalHeatConsumption/KLHMath.utilisationPeriodOfMaximumLoad); }}

        /// <summary>
        /// Amount of district heating energy after the renewable energy has been subtracted from the <paramref name="totalHeatConsumption"/>, using <paramref name="RenewableRatio"/>
        /// </summary>
        /// <returns>Energy in kWh</returns>        
        public int DistrictHeatingConsumption { get { return Mathf.RoundToInt(totalHeatConsumption*(1-RenewableRatio)); }}
        /// <summary>
        /// The initial investment cost in Euros.
        /// </summary>
        /// <returns>Cost in Euros</returns>
        public int InvestmentCost   { get { return CalculateInvestmentCost(); } }

        #endregion


        #region Methods

        /// <summary>
        /// Update the values of the HeatingMethod.
        /// </summary>
        /// <param name="_building">This Building's information is used in calculations.</param>
        public virtual void UpdateValues(Building _building ) 
        {
            Building = _building;

            totalHeatConsumption = _building.TotalHeatConsumption;
            
            renewableEnergyProduced = CalculateRenewableEnergyProduced();
        
            annualDHOperatingCost = CalculateAnnualDistrictEnergyCost();
            
            annualREOperatingCost = CalculateAnnualRenewableEnergyCost();
            
            initialInvestmentCost = CalculateInvestmentCost();
           
            annualDHCO2Amount = CalculateAnnualDistrictCO2Amount();
            
            annualRECO2Amount = CalculateAnnualRenewableCO2Amount();
            
        }



        /// <summary>
        /// Calculates annual base cost for district heating based on demanded energy
        /// Formulas copied from Energialaskenta Excel
        /// </summary>
        /// <param name="_districtHeatingEnergyDemand">Energy demand in kW (NOT kWh).</param>
        /// <returns>districtHeatingAnnualBaseCost</returns>
        public float CalculateAnnualDistrictBaseCost(int _districtHeatingEnergyDemand) 
        {

            float districtHeatingAnnualBaseCost;
            float contractedWaterQuantity = 
                _districtHeatingEnergyDemand 
                / KLHMath.waterSpecificHeatCapacity
                / KLHMath.utilisationPeriodOfMaximumLoad
                / 45 
                * 3.6f;
            
            switch (contractedWaterQuantity)
            {
                case <= 0.8f:
                districtHeatingAnnualBaseCost = districtHeatingK1Multiplier * 742 * contractedWaterQuantity;
                break;
                case <= 2.0f:
                districtHeatingAnnualBaseCost = districtHeatingK1Multiplier * (48 + 682 * contractedWaterQuantity);
                break;
                case <= 8.0f:
                districtHeatingAnnualBaseCost = districtHeatingK1Multiplier * (706 + 353 * contractedWaterQuantity);
                break;
                case <= 15.0f:
                districtHeatingAnnualBaseCost = districtHeatingK1Multiplier * (2122 + 176 * contractedWaterQuantity);
                break;
                default:
                case > 15.0f:
                districtHeatingAnnualBaseCost = districtHeatingK1Multiplier * (2400 + 156 * contractedWaterQuantity);
                break;
            }

            return districtHeatingAnnualBaseCost;
        }

        /// <summary>
        /// Calculate the initial investment cost in Euros.
        /// </summary>
        /// <returns>Device(s) cost</returns>
        protected virtual int CalculateInvestmentCost () 
        {
            float devicePower = TotalPower / devicePowerDivider;
            int deviceCost = (int)(CalculateCostPerKWFromCurve(devicePower)*devicePower);
            if (deviceCost < deviceMinimumCost) { deviceCost = deviceMinimumCost;}

            // Only one District heating hybrid device package price per building
            deviceCost += (int)(CalculateCostPerM3FromCurve(Building.GrossVolume)*Building.GrossVolume);

            return deviceCost;
        }
        /// <summary>
        ///  Calculate the initial investment cost in Euros.
        /// </summary>
        /// <param name="_staircaseCount"> Number of staircases</param>
        /// <returns>Device(s) cost</returns>
        protected virtual int CalculateInvestmentCost (int _staircaseCount) 
        {
            
            float devicePower = TotalPower / devicePowerDivider;
            int deviceCost = (int)(CalculateCostPerKWFromCurve(devicePower)*devicePower);
            if (deviceCost < deviceMinimumCost) { deviceCost = deviceMinimumCost;}
            
            // Heat recovery heating devices are needed one per staircase
            deviceCost = deviceCost*_staircaseCount;

            // Only one District heating hybrid device package price per building
            deviceCost += (int)(CalculateCostPerM3FromCurve(Building.GrossVolume)*Building.GrossVolume);

            return deviceCost;
        }


        /// <summary>
        /// Calculates the cost per every kW from the curve.
        /// </summary>
        /// <param name="_power"> Power in kW</param>
        /// <returns></returns>
        protected float CalculateCostPerKWFromCurve(float _power)
        {
            return costPerKWCurve.Evaluate(_power);
        }
        /// <summary>
        /// Calculates cost for district heating hybrid device cost. 
        /// Only used for hybrid devices. 
        /// </summary>
        /// <param name="_buildingVolume"> Building volume in m3</param>
        /// <returns>Cost in Euros per m3</returns>
        protected float CalculateCostPerM3FromCurve(float _buildingVolume)
        {
            return costPerM3Curve.Evaluate(_buildingVolume);
        }



        /// <summary>
        /// Calculate the annual operating cost of district heating in Euros.
        /// </summary>
        /// <returns>Annual district heating energy cost</returns>
        protected virtual int CalculateAnnualDistrictEnergyCost () 
        {
            float baseCost = CalculateAnnualDistrictBaseCost(DistrictHeatingConsumption);
            float energyCost = (int)(DistrictHeatingConsumption * KLHMath.DistrictHeatingCost);
            annualDHOperatingCost = (int)(baseCost + energyCost);

            return annualDHOperatingCost; 
        }
        
        /// <summary>
        /// Calculate the annual operating cost of renewable heating in Euros.
        /// </summary>
        /// <returns>Annual renewable energy operating costs in Euros</returns>
        protected virtual int CalculateAnnualRenewableEnergyCost () 
        {
            annualREOperatingCost = ((int)(renewableEnergyProduced / copMultiplier * KLHMath.ElectricityCost));
            return annualREOperatingCost; 
        }

        /// <summary>
        /// Calculate total renewable energy produced.
        /// </summary>
        /// <returns>The amount (part of total energy) of renewable energy in kWh </returns>
        protected virtual int CalculateRenewableEnergyProduced() 
        {       
            return renewableEnergyProduced = 0;  
        }

        /// <summary>
        /// Calculate the annual CO2 amount of district heating.
        /// </summary>
        /// <returns>Annual CO2 amount in kilograms.</returns>
        protected virtual int CalculateAnnualDistrictCO2Amount() 
        {
            return annualDHCO2Amount = (int)(DistrictHeatingConsumption * KLHMath.DistrictHeatingCO2EmissionFactor / 1000);     
        }

        /// <summary>
        /// Calculate the annual CO2 amount of renewable heating.
        /// </summary>
        /// <returns>Annual CO2 amount in kilograms.</returns>
        protected virtual int CalculateAnnualRenewableCO2Amount() 
        {
            return annualRECO2Amount = (int)(renewableEnergyProduced * KLHMath.ElectricityCO2EmssionFactor / copMultiplier / 1000); 
        }

        /// <summary>
        /// Calculate the total cost of district heating within the specified timespan.
        /// </summary>
        /// <param name="_yearcount">The specified timespan in years.</param>
        /// <returns>Total cost in Euros</returns>
        public virtual int CalculateLongtermDistrictEnergyCost(int _yearcount) 
        {
            int totalCost = 0;

            for (int i = 0; i < _yearcount; i++) {
                totalCost += annualDHOperatingCost;
            }

            return totalCost;
        }
        /// <summary>
        /// Calculate long term maintenance costs by number of years
        /// </summary>
        /// <param name="_yearcount">Number of years</param>
        /// <returns>Cost in Euros</returns>
        public virtual int CalculateLongtermMaintenanceCosts(int _yearcount)
        {
            int totalCost = 0;
            
            for (int i = 0; i < _yearcount; i++) {

            totalCost += (int)(Building.GrossArea * maintenancePerSqMeterDH);
            }
            return totalCost;
        }

        /// <summary>
        /// Calculate the total cost of renewable heating within the specified timespan.
        /// </summary>
        /// <param name="_yearcount">The specified timespan in years.</param>
        /// <returns>Total cost in Euros</returns>
        public virtual int CalculateLongtermRenewableEnergyCost(int _yearcount) 
        {
            int totalCost = 0;

            return totalCost; 
        }


        #endregion


    } /// End of Class


} /// End of Namespace