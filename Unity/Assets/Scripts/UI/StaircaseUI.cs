/******************************************************************************
 * File        : StaircaseUI.cs
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
using TMPro;


namespace DUKE.UI {


    public class StaircaseUI : MonoBehaviour
    {
        #region Variables

        [SerializeField] Transform staircaseButtonsParent;
        [SerializeField] TextMeshProUGUI warningTextObj;
        [SerializeField] TextMeshProUGUI staircaseCountObj;

        #endregion


        #region Methods

        void OnEnable()
        {
            SelectStaircaseCount(KLHManager.Building.StaircaseCount);

            KLHManager.BuildingChanged += UpdateStaircaseStates;

            warningTextObj.gameObject.SetActive(false);    
            warningTextObj.gameObject.SetActive(true);    
        }

        void OnDisable()
        {
            KLHManager.BuildingChanged -= UpdateStaircaseStates;
        }



        /// <summary>
        /// Update button states manually when active <typeparamref name="Building"/> is changed.
        /// </summary>
        void UpdateStaircaseStates ()
        {
            SelectStaircaseCount(KLHManager.Building.StaircaseCount);

            for (int i = 0; i < staircaseButtonsParent.childCount; i++) {

                staircaseButtonsParent.GetChild(i).GetComponent<Clickable>().SetToggled(i + 1 == KLHManager.Building.StaircaseCount, true);
            }
        }

        /// <summary>
        /// Set active Building's StaircaseCount and update info text.
        /// </summary>
        /// <param name="_count">New number of staircases.</param>
        public void SelectStaircaseCount (int _count) 
        {
            KLHManager.Building.StaircaseCount = _count;

            staircaseCountObj.text = _count.ToString();
        }

        #endregion


    } /// End of Class


} /// End of Namespace