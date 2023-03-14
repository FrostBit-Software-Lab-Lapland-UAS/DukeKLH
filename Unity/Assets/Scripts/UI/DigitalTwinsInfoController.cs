/******************************************************************************
 * File        : DigitalTwinsInfoController.cs
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
using DUKE.Controls;


namespace DUKE {


    /// <summary>
    /// Control the toggling of info panels in Digital Twins scene.
    /// Used through Highlightables' UnityEvents.
    /// </summary>
    public class DigitalTwinsInfoController : MonoBehaviour
    {
        #region Variables

        [SerializeField] Clickable[] clickables;
        [SerializeField] GameObject[] infoPanels;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        void OnEnable()
        {
            ResetInfo();
        }

        #endregion
        #region Info Panel Methods

        /// <summary>
        /// Toggle info panels and Clickables on or off depending on the <paramref name="_index"/>.
        /// </summary>
        /// <param name="_index">Index of the Clickable and GameObject to be toggled.</param>
        public void ToggleInfo (int _index)
        {
            bool on = !clickables[_index].IsToggled;

            for (int i = 0; i < clickables.Length; i++) {

                bool active = _index == i && on;

                clickables[i].SetToggled(active, true);
                infoPanels[i].SetActive(active);

                clickables[i].GetComponent<MeshRenderer>().material.SetInt("_Selected", active ? 1 : 0);
                //clickables[i].GetComponentInChildren<TMPro.TextMeshPro>().color = active ? Color.white : Color.black;
            }
        }

        /// <summary>
        /// Toggle all Clickables and info panels off.
        /// </summary>
        public void ResetInfo ()
        {
            for (int i = 0; i < clickables.Length; i++) {

                clickables[i].SetToggled(false, true);
                infoPanels[i].SetActive(false);
            }
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace