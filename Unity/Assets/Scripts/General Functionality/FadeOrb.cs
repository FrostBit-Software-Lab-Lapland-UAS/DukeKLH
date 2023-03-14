/******************************************************************************
 * File        : FadeOrb.cs
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


namespace DUKE {


    /// <summary>
    /// Reads KLHManager's FadeValue and sets the opacity of an orb based on it.
    /// </summary>
    public class FadeOrb : MonoBehaviour
    {
        MeshRenderer mRend;


        #region Methods

        void OnEnable()
        {
            mRend = GetComponent<MeshRenderer>();
        }   

        void Update()
        {
            UpdateFadeOpacity(KLHManager.FadeValue);    
        }



        /// <summary>
        /// Set the opacity of the MeshRenderer's material.
        /// </summary>
        /// <param name="_opacity"></param>
        void UpdateFadeOpacity (float _opacity) 
        {
            mRend.material.SetFloat("_Fade", _opacity); 
        }

        #endregion

        
    } /// End of Class


} /// End of Namespace