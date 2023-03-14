/******************************************************************************
 * File        : ZoneMap.cs
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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DUKE.Controls;
using DUKE.KLHData;


namespace DUKE.UI {


    /// <summary>
    /// Control various aspects of the map element.
    /// </summary>
    [RequireComponent(typeof(AreaSelector))]
    public class ZoneMap : MonoBehaviour 
    {
        #region Variables

        [Space(25f)]
        [SerializeField] BuildingZone zone = BuildingZone.IV;
        [SerializeField] Image img;
        [SerializeField] Color col;
        [SerializeField] Color[] zoneColors = new Color[4];
        [SerializeField] int colorVarianceTolerance = 2;
        int prevIndex = -1;

        #endregion


        #region Events

        /// <summary>
        /// Called when <paramref name="zone"/> is updated.
        /// </summary>
        public Action<BuildingZone> ZoneSelected;

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Select a BuildingZone.
        /// </summary>
        /// <param name="_index">Index of enum selection.</param>
        public void SelectZone (int _index, bool _invokeEvent = true)
        {
            KLHManager.Building.Zone = zone = (BuildingZone)_index;
            ZoneSelected?.Invoke(zone);            
        }

        /// <summary>
        /// Set zone and update visuals.
        /// </summary>
        /// <param name="_zone"></param>
        public void SetZoneSelected (BuildingZone _zone)
        {
            zone = _zone;
            UpdateVisuals();
        }

        /// <summary>
        /// Toggle the selection visuals for each area depending on selected BuildingZone.
        /// </summary>
        public void UpdateVisuals ()
        {
            int index = (int)KLHManager.Building.Zone;
            transform.Find("Highlight Areas").GetChild(0).gameObject.SetActive(index == 0);
            transform.Find("Highlight Areas").GetChild(1).gameObject.SetActive(index == 1);
            transform.Find("Highlight Areas").GetChild(2).gameObject.SetActive(index == 2);
            transform.Find("Highlight Areas").GetChild(3).gameObject.SetActive(index == 3);
        }

        #endregion
        #region MonoBehaviour Methods

        void OnEnable ()
        {
            GetComponent<AreaSelector>().OnAreaClicked += CalculateColorOfCoordinates;
            ZoneSelected += SetZoneSelected;

            if (null == img) { 

                img = GetComponent<Image>(); 
            }  

            UpdateVisuals();
        }

        void OnDisable ()
        {
            GetComponent<AreaSelector>().OnAreaClicked -= CalculateColorOfCoordinates;
            ZoneSelected -= SetZoneSelected;
        }

        #endregion
        #region Color Comparison Methods

        /// <summary>
        /// Read the Color of a clicked pixel. 
        /// Select a BuildingZone if the color matches the one specified in zoneColors.
        /// </summary>
        /// <param name="_ratio">Ratio of the Image's Rect between 0...1 on X and Y axii.</param>
        void CalculateColorOfCoordinates (Vector2 _ratio)
        {
            Color col = ColorReader.GetColor(img, _ratio);

            /// Proceed only if the color's alpha is 1 (ignore transparent background).
            if (col.a == 1) { 
                
                int index = CompareColors(col); 

                if (index >= 0 && index < 4 && index != prevIndex) { 
                        
                    prevIndex = index;
                    SelectZone(index); 
                }
            }
        }

        /// <summary>
        /// Compare the provided Color against zoneColors and return the index of the closest match.
        /// If a match cannot be found within tolerance, return -1.
        /// </summary>
        /// <param name="_color"></param>
        /// <returns></returns>
        int CompareColors (Color _color)
        {
            Color.RGBToHSV(_color, out float hue, out float saturation, out float value);

            int hueInt = Mathf.RoundToInt(hue * 360f);
            int satInt = Mathf.RoundToInt(saturation * 100f);
            int valInt = Mathf.RoundToInt(value * 100f);

            for (int i = 0; i < zoneColors.Length; i++) {

                Color.RGBToHSV(zoneColors[i], out float zHhue, out float zSaturation, out float zValue);

                int zHueInt = Mathf.RoundToInt(zHhue * 360f);
                int zSatInt = Mathf.RoundToInt(zSaturation * 100f);
                int zValInt = Mathf.RoundToInt(zValue * 100f);

                int hueDiff = hueInt - zHueInt;
                int satDiff = satInt - zSatInt;
                int valDiff = valInt - zValInt;

                if (Mathf.Abs(hueDiff) < colorVarianceTolerance
                    && Mathf.Abs(satDiff) < colorVarianceTolerance
                    && Mathf.Abs(valDiff) < colorVarianceTolerance) {

                    return i;
                }
            }

            return -1;
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace