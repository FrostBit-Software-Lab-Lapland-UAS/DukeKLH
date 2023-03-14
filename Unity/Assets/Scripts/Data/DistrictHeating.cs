
/******************************************************************************
 * File        : DistrictHeating.cs
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DUKE.KLHData {

    /// <summary>
    /// A heating method that acts as the baseline heating method in this project.
    /// </summary>
    [CreateAssetMenu(fileName = "New DistrictHeating", menuName = "ScriptableObjects/HybridDevices/New DistrictHeating", order = 0)]
    public class DistrictHeating : HeatingMethod
    {

        protected override int CalculateInvestmentCost ()
        {
            
            // installation costs from Granlund Excel
            int installCost = 5000;

            // deviceCost from curves from Granlund Excel multiplied by power
            int deviceCost = (int)(CalculateCostPerKWFromCurve(TotalPower)*TotalPower);

            // set baseline cost for small buildings
            if (deviceCost < 20000 ) { deviceCost = 20000; }
            
            return initialInvestmentCost = installCost + deviceCost;
        }

        protected override int CalculateAnnualDistrictEnergyCost ()
        {
            return base.CalculateAnnualDistrictEnergyCost();
        }

        protected override int CalculateRenewableEnergyProduced()
        {
            return 0;
        }


        public override int CalculateLongtermRenewableEnergyCost(int _yearcount) 
        {
            return 0;
        }

    } /// End of Class


} /// End of Namespace