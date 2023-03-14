/******************************************************************************
 * File        : LookAtPlayer.cs
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


namespace DUKE{


    /// <summary>
    /// Turn the Transform to face Player.
    /// </summary>
    public class LookAtPlayer : MonoBehaviour
    {
        [SerializeField] bool invertDirection = false;
        [SerializeField] bool lockX = true;
        [SerializeField, Range(0, 1f)] float smoothingAmount = 0.5f;



        /// <summary>
        /// TRUE if <paramref name="smoothingAmount"/> value is higher than 0.
        /// </summary>
        public bool Smoothing 
        { 
            get { return smoothingAmount > 0f; } 
        }



        void Update()
        {
            Vector3 point = Vector3.zero;

            if (Smoothing) 
            {
                point = Vector3.Lerp(
                    transform.position + (invertDirection ? -transform.forward : transform.forward), 
                    KLHManager.ActiveCamera.position,
                    1f - (Mathf.Clamp(smoothingAmount, 0.001f, 0.999f)));
            } 
            else
            {
                point = KLHManager.ActiveCamera.position;
            }   

            if (invertDirection) 
            {
                Vector3 localPoint = transform.InverseTransformPoint(point);
                Vector3 localInvertedPoint = localPoint * -1f;
                point = transform.TransformPoint(localInvertedPoint);             
            }

            if (lockX)
            {
                point.y = transform.position.y;
            }

            transform.LookAt(point, Vector3.up);    
        }


    } /// End of Class


} /// End of Namespace