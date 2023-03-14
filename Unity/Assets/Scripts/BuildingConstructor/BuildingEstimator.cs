/******************************************************************************
 * File        : BuildingEstimator.cs
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
using DUKE.KLHData;


namespace DUKE {


    /// <summary>
    /// Estimation of a building's dimensions based on total heat consumption.
    /// </summary>
    [System.Serializable]
    public struct BuildingEstimation 
    {
        /// <summary>
        /// TRUE if this estimation is valid.
        /// </summary>
        public bool valid;

        /// <summary>
        /// Total energy consumption in kWh.
        /// </summary>
        public int totalConsumption;

        /// <summary>
        /// Y-axis length of the <typeparamref name="Building"/>.
        /// </summary>
        public int depth;

        /// <summary>
        /// X-axis length of the <typeparamref name="Building"/>.
        /// </summary>
        public int length;

        /// <summary>
        /// Maximum floor count of the <typeparamref name="Building"/>.
        /// </summary>
        public int floorCount;

        /// <summary>
        /// Height of a single floor in meters.
        /// </summary>
        public float storyHeight;

        /// <summary>
        /// <typeparamref name="BuildingZone"/> of the <typeparamref name="Building"/>.
        /// </summary>
        public BuildingZone zone;

        /// <summary>
        /// Construction era of the <typeparamref name="Building"/>.
        /// </summary>
        public int era;



        /// <summary>
        /// Constructor of <typeparamref name="BuildingEstimation"/>.
        /// </summary>
        /// <param name="_valid">TRUE if the estimation is valid and usable.</param>
        /// <param name="_consumption">Total calculated consumption of the estimation.</param>
        /// <param name="_depth">Depth of the estimated building.</param>
        /// <param name="_length">Length of the estimated building.</param>
        /// <param name="_floorCount">Floor count of the estimated building.</param>
        /// <param name="_storyHeight">Height of a single floor.</param>
        /// <param name="_zone">Selected BuildingZone used in calculating the estimation.</param>
        /// <param name="_era">Selected era used in calculating the estimation.</param>
        public BuildingEstimation (bool _valid, int _consumption, int _depth, int _length, 
        int _floorCount, float _storyHeight, BuildingZone _zone, int _era)
        {
            valid = _valid;
            totalConsumption = _consumption;
            depth = _depth;
            length = _length;
            floorCount = _floorCount;
            storyHeight = _storyHeight;
            zone = _zone;
            era = _era;
        }
    
    
    } /// End of Struct



    /// <summary>
    /// Calculate an approximate shape of a building based on provided parameters.
    /// </summary>
    public static class BuildingEstimator
    {
        /// <summary>
        /// Form an estimation of a Building's length and depth based on given parameters.
        /// </summary>
        /// <param name="_targetKWH">Given energy consumption the algorithm attempt to reach.</param>
        /// <param name="_storyCount">The number of stories.</param>
        /// <param name="_constructionYear">The year the Building was built. Used in determining the amount of energy lost through conduction.</param>
        /// <param name="_zone">Temperature zone of the Building. Used in determinining the average temperature.</param>
        /// <param name="_startDepth">Initial depth of the Building. Default value = 12m.</param>
        /// <param name="_maxDepth">Maxmimum allowed depth of the Building. Default value = 25m.</param>
        /// <param name="_startLength">Initial length of the Building. Default value = 0m.</param>
        /// <param name="_maxLength">Maximum allowed length of the Building Default value = 100m.</param>
        /// <param name="_storyHeight">The height of a single floor/story. Default value = 3m.</param>
        /// <param name="_airReplacementRate">The rate at which air is replaced. Default value = 0.1.</param>
        /// <returns>BuildingEstimation containing the Building's estimated depth, width, consumption etc.</returns>
        static BuildingEstimation CalculateBuildingDimensions (int _targetKWH, int _storyCount, int _constructionYear, BuildingZone _zone,
        int _startDepth = 12, int _maxDepth = 25, int _startLength = 0, int _maxLength = 100, float _storyHeight = 3.0f, float _airReplacementRate = 0.1f)
        {
            bool calculationComplete = false;
            int prevKWH = 0;
            int length = 0;
            int depth = _startDepth;
            int fSafe = 10000;

            /// Loop until length reaches maximum or consumption goes over the target.
            while (length < _maxLength && !calculationComplete && fSafe > 0) 
            {
                length++;

                int newKWH = GetTotalConsumption(length, depth, _storyCount, _storyHeight, _constructionYear, _zone, _airReplacementRate);

                if (newKWH > _targetKWH) 
                {
                    int overDiff = newKWH - _targetKWH;
                    int underDiff = _targetKWH - prevKWH;

                    if (overDiff > underDiff) { length--; }

                    calculationComplete = true;
                    prevKWH = newKWH;
                    break;
                }

                prevKWH = newKWH;

                fSafe--;
                if (fSafe <= 0) 
                {             
                    #if UNITY_EDITOR
                    Debug.LogError("BuildingEstimator.CalculateBuildingDimensions(): Failsafe failed!"); 
                    #endif

                    break; 
                }
            }

            /// Loop if length reached maximum without consumption going over the target.
            while (depth < _maxDepth && !calculationComplete && fSafe > 0) 
            {
                depth++;

                int newKWH = GetTotalConsumption(length, depth, _storyCount, _storyHeight, _constructionYear, _zone, _airReplacementRate);

                if (newKWH > _targetKWH) 
                {
                    break;
                }

                prevKWH = newKWH;

                fSafe--;

                if (fSafe <= 0) 
                { 
                    #if UNITY_EDITOR
                    Debug.LogError("BuildingEstimator.CalculateBuildingDimensions(): Failsafe failed!"); 
                    #endif
                    
                    break; 
                }
            }

            /// Check if maximum depth was reached without going over the limit.
            if (depth == _maxDepth && prevKWH < _targetKWH) 
            {
                /// Return an invalid estimation.
                return new BuildingEstimation (false, prevKWH, depth, length, _storyCount, _storyHeight, _zone, KLHMath.YearToEra(_constructionYear));
            }

            /// Loop and reduce length until consumption goes below the target. 
            /// Compare that value to previous length and choose the one with smaller difference to the target.
            while (length > 0 && !calculationComplete && fSafe > 0) 
            {
                length--;

                int newKWH = GetTotalConsumption(length, depth, _storyCount, _storyHeight, _constructionYear, _zone, _airReplacementRate);

                if (newKWH < _targetKWH) 
                {
                    int overDiff = prevKWH - _targetKWH;
                    int underDiff = _targetKWH - newKWH;

                    if (overDiff < underDiff) { length++; }

                    calculationComplete = true;
                    prevKWH = newKWH;
                    break;
                }

                fSafe--;

                if (fSafe <= 0) 
                {   
                    #if UNITY_EDITOR
                    Debug.LogError("BuildingEstimator.CalculateBuildingDimensions(): Failsafe failed!"); 
                    #endif
                    
                    break; 
                }
            }

            /// Return a valid estimation.
            return new BuildingEstimation (true, prevKWH, depth, length, _storyCount, _storyHeight, _zone, KLHMath.YearToEra(_constructionYear));
        }

        /// <summary>
        /// Calculate the total energy consumption of a building with the specified parameters.
        /// </summary>
        /// <param name="_length">Length of the building in meters.</param>
        /// <param name="_depth">Depth of the building in meters.</param>
        /// <param name="_storyCount">The number of floor/stories.</param>
        /// <param name="_storyHeight">The height of a single floor/story.</param>
        /// <param name="_constructionYear">The year the Building was built. Used in determining the amount of energy lost through conduction.</param>
        /// <param name="_zone">Temperature zone of the Building. Used in determinining the average temperature.</param>
        /// <param name="_airReplacementRate">The rate at which air is replaced.</param>
        /// <returns>Total annual energy consumption in kWh.</returns>
        static int GetTotalConsumption (int _length, int _depth, int _storyCount, float _storyHeight, int _constructionYear, BuildingZone _zone, float _airReplacementRate)
        {
            int floorArea = _length * _depth;
            int wallArea = Mathf.RoundToInt((2 * _length * _storyHeight * _storyCount) + (2 * _depth * _storyHeight * _storyCount));
            int volume = Mathf.RoundToInt(_length * _depth * _storyHeight * _storyCount);
            int grossArea = floorArea * _storyCount;

            int era = KLHMath.YearToEra(_constructionYear);
            float[] uValues = KLHMath.UValuesByYear(era);
            float tempDiff = KLHMath.TemperatureDifference(_zone);

            int conduction = KLHMath.HeatLossThroughConduction(uValues[0], uValues[1], uValues[2], wallArea, floorArea, tempDiff);
            int ventilation = KLHMath.HeatLossThroughVentilation(volume, 1, 1, 0, tempDiff);
            int replacement = KLHMath.HeatLossThroughReplacementAir(volume, tempDiff, _airReplacementRate);
            int water = KLHMath.HeatLossThroughWaterHeating(KLHMath.ConsumptionOfHeatedWaterByGrossArea(grossArea));
            int externalSources = KLHMath.HeatGainFromTotalHeatLoad(grossArea, conduction, ventilation, replacement, tempDiff, StructureComposition.Medium);

            return conduction + ventilation + replacement + water - externalSources;
        }

        /// <summary>
        /// Update the dimensions of <paramref name="_building"/>.
        /// </summary>
        /// <param name="_building">The Building instance with updated dimensions.</param>
        /// <param name="_targetKWH">Given energy consumption the algorithm attempt to reach.</param>
        /// <param name="_storyCount">The number of stories.</param>
        /// <param name="_constructionYear">The year the Building was built. Used in determining the amount of energy lost through conduction.</param>
        /// <param name="_zone">Temperature zone of the Building. Used in determinining the average temperature.</param>
        /// <param name="_startDepth">Initial depth of the Building. Default value = 12m.</param>
        /// <param name="_maxDepth">Maxmimum allowed depth of the Building. Default value = 25m.</param>
        /// <param name="_startLength">Initial length of the Building. Default value = 0m.</param>
        /// <param name="_maxLength">Maximum allowed length of the Building Default value = 100m.</param>
        /// <param name="_storyHeight">The height of a single floor/story. Default value = 3m.</param>
        /// <param name="_airReplacementRate">The rate at which air is replaced. Default value = 0.1.</param>
        public static void UpdateBuildingGrid (Building _building, int _targetKWH, int _storyCount, int _constructionYear, BuildingZone _zone, 
        int _startDepth = 12, int _maxDepth = 25, int _startLength = 0, int _maxLength = 100, float _storyHeight = 3.0f, float _airReplacementRate = 0.1f)
        {
            DrawGrid newGrid = new DrawGrid(BuildingDrawer.MaxGridDimensions.x, BuildingDrawer.MaxGridDimensions.y);
            BuildingEstimation newEstimation = CalculateBuildingDimensions(_targetKWH, _storyCount, _constructionYear, _zone, _startDepth, _maxDepth, _startLength, _maxLength, _storyHeight, _airReplacementRate);

            if (newEstimation.valid) 
            {
                /// Calculate the start position in order to place the building to the center of the grid.
                int startX = newGrid.Width / 2 - newEstimation.length / 2;
                int startY = newGrid.Depth / 2 - newEstimation.depth / 2;

                for (int x = startX; x < startX + newEstimation.length; x++) 
                {
                    for (int y = startY; y < startY + newEstimation.depth; y++) 
                    {
                        newGrid.Cells[x, y].Value = _storyCount;
                    }
                }

                _building.DrawGrid = newGrid;
            } 
            else 
            {
                #if UNITY_EDITOR
                Debug.LogError("BuildingEstimator.UpdateBuildingGrid(): The estimated building was not valid. Check the parameters.");
                #endif
            }
        }


    } /// End of Class


} /// End of Namespace