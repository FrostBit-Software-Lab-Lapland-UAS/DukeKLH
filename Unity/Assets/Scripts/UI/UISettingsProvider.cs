/******************************************************************************
 * File        : UISettingsProvider.cs
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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DUKE.UI {


    /// <summary>
    /// Add to an UI element to define the visual aspects of its children.
    /// </summary>
    public class UISettingsProvider : MonoBehaviour
    {
        [SerializeField] UIPaletteSO uiPalette;



        /// <summary>
        /// Current <typeparamref name="UIPaletteSO"/> that is provided to every <typeparamref name="UISettingsElement"/> below this object in hierarchy.
        /// </summary>
        public UIPaletteSO UIPalette { get { return null == uiPalette 
            ? Resources.Load("ScriptableObjects/UISettings/DefaultSettings") as UIPaletteSO
            : uiPalette; } }



        /// <summary>
        /// Called from editorscripts. 
        /// Update the visuals of every object containing UISettingsElement 
        /// that is below this object in hierarchy.
        /// </summary>
        public void SettingsChanged ()
        {
            UISettingsElement[] elements = GetComponentsInChildren<UISettingsElement>(true);

            foreach (UISettingsElement element in elements) {

                element.SetVisuals();
            }
        }


    } /// End of Class


} /// End of Namespace