/******************************************************************************
 * File        : AirsourceHeatPumpHeating.cs
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
    /// A heating device that extracts energy from outdoor air to heat the building.
    /// </summary>
    [CreateAssetMenu(fileName = "New HeatPumpDevice", menuName = "ScriptableObjects/HybridDevices/New HeatPumpDevice", order = 0)]
    public class AirsourceHeatPumpHeating : HeatingMethod 
    {

        #region Variables

        /// <summary>
        /// Ratio of how much is being covered by airsource, 3 means one third of the power is airsource
        /// </summary>
        public float airsourceDivider = 3;

        #endregion


        #region Methods

        #region Override Methods

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
            devicePowerDivider = 2;
            deviceMinimumCost = 28000;
            int deviceCost = base.CalculateInvestmentCost(); 
            // installation costs from Granlund Excel
            int installCost = 2000 + 5000 + 10000 + 2000;

            int total = deviceCost + installCost;

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
            return base.CalculateAnnualRenewableEnergyCost();
        }
        
        /// <summary>
        /// Calculate the total cost of renewable heating within the specified timespan.
        /// </summary>
        /// <param name="_yearcount">The specified timespan in years.</param>
        /// <returns>Total cost in Euros</returns>
        public override int CalculateLongtermRenewableEnergyCost(int _yearcount) {
            
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

            int totalCost = initialInvestmentCost;
            float airHeatPumpPower = TotalPower/airsourceDivider;
            
            // new method 15y maintenance is 1/2 of the pump price
            totalCost += (int)((_yearcount / 15) * (CalculateCostPerKWFromCurve(airHeatPumpPower)*airHeatPumpPower/2));

            for (int x=0; x<_yearcount; x++) {
                totalCost += (int)(Building.GrossArea*maintenancePerSqMeterRe);
            }
           
          return totalCost;
        }
         /// <summary>
        /// Calculate total renewable energy produced.
        /// </summary>
        /// <returns>The amount (part of total energy) of renewable energy in kWh </returns>
        protected override int CalculateRenewableEnergyProduced()
        {
            // new: airsource is one third of the total
            return (int)(totalHeatConsumption / airsourceDivider);
        }

        #endregion
        #region Calculation Methods


        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace