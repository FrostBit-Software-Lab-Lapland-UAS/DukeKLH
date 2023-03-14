/******************************************************************************
 * File        : DrawCell.cs
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
    /// A single Cell of a Grid.
    /// </summary>
    [System.Serializable]
    public class DrawCell
    {
        #region Variables

        [SerializeField] int x;
        [SerializeField] int y;
        [SerializeField] int value;

        #endregion


        #region Properties

        /// <summary>
        /// X-axis index.
        /// </summary>
        public int X 
        { 
            get { return x; } 
        }
        
        /// <summary>
        /// Y-axis index.
        /// </summary>
        public int Y 
        { 
            get { return y; } 
        }
        
        /// <summary>
        /// Integer value.
        /// </summary>
        public int Value 
        { 
            get { return value; } 
            set { this.value = value; } 
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Empty constructor of <typeparamref name="DrawCell"/>.
        /// </summary>
        public DrawCell () { }

        /// <summary>
        /// Constructor of <typeparamref name="DrawCell"/>.
        /// </summary>
        /// <param name="_x">Grid coordinate point of this instance on X-axis.</param>
        /// <param name="_y">Grid coordinate point of this instance on Y-axis.</param>
        /// <param name="_value">Height value of the instance.</param>
        public DrawCell (int _x, int _y, int _value)
        {
            x = _x;
            y = _y;
            value = _value;
        }

        #endregion


    } /// End of Class


} /// End of Namespace