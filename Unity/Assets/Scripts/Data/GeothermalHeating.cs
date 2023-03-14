/******************************************************************************
 * File        : GeothermalHeating.cs
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
    /// A heating device that extracts energy from ground to heat the building.
    /// </summary>
    [CreateAssetMenu(fileName = "New GeothermalDevice", menuName = "ScriptableObjects/HybridDevices/New GeothermalDevice", order = 0)]
    public class GeothermalHeating : HeatingMethod
    {
        #region Variables

        /// <summary>
        /// Ratio of how much is being covered by geothermal, 2 means half of the power is geothermal
        /// </summary>
        public float geothermalDivider = 2;
        
        #endregion


        #region Methods

        /// <summary>
        /// Update the values of the <typeparamref name="HeatingMethod"/>.
        /// </summary>
        /// <param name="_building">Selected <typeparamref name="Building"/>.</param>
        public override void UpdateValues(Building _building)
        {
            maintenancePerSqMeterRe = 0.3f - maintenancePerSqMeterDH;
            base.UpdateValues(_building);
        }
        
         /// <summary>
        /// Calculate the <paramref name="initialInvestmentCost"/> cost in Euros.
        /// </summary>
        /// <returns>Initial investment cost in Euros.</returns>
        protected override int CalculateInvestmentCost ()
        {
            devicePowerDivider = 3;
            deviceMinimumCost = 9500;
            int deviceCost = base.CalculateInvestmentCost();
    
            // from Granlund Excel
            int geoThermalWellCost = (int)(TotalPower/devicePowerDivider * 1000/45*30);

            // installation costs from Granlund Excel
            int installCost = 2000 + 5000 + 10000 + 2000;

            int total = deviceCost + geoThermalWellCost + installCost;

            return initialInvestmentCost = total;
        }

        /// <summary>
        /// Calculate the annual operating cost of district heating in Euros.
        /// </summary>
        /// <returns>Annual district heating energy cost</returns>
        protected override int CalculateAnnualDistrictEnergyCost ()
        {
            return base.CalculateAnnualDistrictEnergyCost();
        }

         /// <summary>
        /// Calculate the annual operating cost of renewable heating in Euros.
        /// </summary>
        /// <returns>Annual renewable energy operating costs in Euros</returns>
        protected override int CalculateAnnualRenewableEnergyCost()
        {
            // adjust COP if needed
            // copMultiplier = 3f;
            return base.CalculateAnnualRenewableEnergyCost();
        }
         /// <summary>
        /// Calculate total renewable energy produced.
        /// </summary>
        /// <returns>The amount (part of total energy) of renewable energy in kWh </returns>
        protected override int CalculateRenewableEnergyProduced()
        {
                   
            return (int)(totalHeatConsumption / geothermalDivider);
        }

        /// <summary>
        /// Calculate the total cost of renewable heating within the specified timespan.
        /// </summary>
        /// <param name="_yearcount">The specified timespan in years.</param>
        /// <returns>Total cost in Euros</returns>
        public override int CalculateLongtermRenewableEnergyCost(int _yearcount) 
        {
            int totalCost = 0;
      
            // add gross area based annual maintenance cost
            for (int i = 0; i < _yearcount; i++) {

                totalCost += AnnualREOperatingCost;
            }
            return totalCost;
        }

        /// <summary>
        /// Calculate long term maintenance costs by number of years
        /// </summary>
        /// <param name="_yearcount">Number of years</param>
        /// <returns>Cost in Euros</returns>
         public override int CalculateLongtermMaintenanceCosts(int _yearcount) {
            // jatka tästä
            int totalCost = initialInvestmentCost;
           float geothermalPower = TotalPower/geothermalDivider;
            
           // new method 15y maintenance is 1/2 of the pump price
            totalCost += (int)((_yearcount / 15) * (CalculateCostPerKWFromCurve(geothermalPower)*geothermalPower/2));

            for (int x=0; x<_yearcount; x++) {
                totalCost += (int)(Building.GrossArea*maintenancePerSqMeterRe);
            }
           
          return totalCost;
        }

        #endregion


    } /// End of Class


} /// End of Namespace