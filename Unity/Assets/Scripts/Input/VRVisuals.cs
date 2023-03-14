/******************************************************************************
 * File        : VRVisuals.cs
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


namespace DUKE.Controls
{


    public class VRVisuals : MonoBehaviour
    {
        #region Variables

        [SerializeField] GameObject htcVisuals;
        [SerializeField] GameObject oculusVisuals;

        #endregion


        #region Properties

        /// <summary>
        /// Current <typeparamref name="DeviceManufacturer"/>.
        /// </summary>
        protected DeviceManufacturer Manufacturer
        {
            get;
            set;
        } = DeviceManufacturer.Unknown;

        /// <summary>
        /// TRUE when <paramref name="Manufacturer"/> is HTC.
        /// </summary>
        public bool IsHTC
        {
            get { return Manufacturer == DeviceManufacturer.HTC; }
        }

        /// <summary>
        /// TRUE when <paramref name="Manufacturer"/> is Oculus.
        /// </summary>
        public bool IsOculus
        {
            get { return Manufacturer == DeviceManufacturer.Oculus; }
        }

        #endregion


        #region Methods

        #region MonoBehaviour

        void OnEnable()
        {
            DetectInputDevices.DeviceManufacturerChanged += SetControllerVisualsByManufacturer;
            SetControllerVisualsByManufacturer(DetectInputDevices.Manufacturer);
        }

        void OnDisable()
        {
            DetectInputDevices.DeviceManufacturerChanged -= SetControllerVisualsByManufacturer;
        }

        #endregion
        #region Private Methods

        void SetControllerVisualsByManufacturer (DeviceManufacturer _newManufacturer)
        {
            if (_newManufacturer != DeviceManufacturer.Unknown)
            {
                Manufacturer = _newManufacturer;

                htcVisuals.SetActive(Manufacturer == DeviceManufacturer.HTC);
                oculusVisuals.SetActive(Manufacturer == DeviceManufacturer.Oculus);          
            }
        }

        #endregion

        #endregion



    } /// End of Class


} /// End of Namespace