/******************************************************************************
 * File        : Turntable.cs
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
    /// Spin the object around an axis.
    /// </summary>
    public class Turntable : MonoBehaviour
    {
        /// <summary>
        /// The amount of degrees the object rotates per second (clockwise).
        /// </summary>
        public float degreesPerSecond = 10f;

        /// <summary>
        /// Axii through which the rotation happens.
        /// </summary>
        public AxisFlags axis = AxisFlags.Y;



        private void Update ()
        {
            float deltaAngle = degreesPerSecond * Time.deltaTime;
            Vector3 eulerDelta = Vector3.zero;

            if (axis.HasFlag(AxisFlags.X)) { eulerDelta.x += deltaAngle; }
            if (axis.HasFlag(AxisFlags.Y)) { eulerDelta.y += deltaAngle; }
            if (axis.HasFlag(AxisFlags.Z)) { eulerDelta.z += deltaAngle; }

            transform.rotation *= Quaternion.Euler(eulerDelta);
        }


    } /// End of Class


} /// End of Namespace