/******************************************************************************
 * File        : GraphPoint.cs
 * Version     : 1.0
 * Author      : Miika Puljuj�rvi (miika.puljujarvi@lapinamk.fi)
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
    /// A single data point on a Graph. 
    /// Holds a reference to the GraphArea it is part of to access positional and range information.
    /// </summary>
    [System.Serializable]
    public class DataPoint 
    {
        #region Variables

        [SerializeField] string name;
        [SerializeField] float value;
        [SerializeField] int index;
        [SerializeField] string extraInfo;

        #endregion


        #region Properties

        /// <summary>
        /// Name of this <typeparamref name="DataPoint"/>.
        /// </summary>
        public string Name 
        { 
            get { return name; } 
        }

        /// <summary>
        /// Value of this <typeparamref name="DataPoint"/>.
        /// </summary>
        public float Value 
        { 
            get { return value; } 
        }

        /// <summary>
        /// Index of this <typeparamref name="DataPoint"/> within a <typeparamref name="Dataset"/> or <typeparamref name="GraphDataset"/>. 
        /// </summary>
        public int Index 
        { 
            get { return index; } 
        }

        /// <summary>
        /// Additional information of this <typeparamref name="DataPoint"/> in string format. This property is used primarily in providing information through <typeparamref name="GraphInfo"/>.
        /// </summary>
        public string ExtraInfo
        { 
            get { return extraInfo;}
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Empty constructor of <typeparamref name="DataPoint"/>.
        /// </summary>
        public DataPoint () { }

        /// <summary>
        /// Constructor of <typeparamref name="DataPoint"/>.
        /// </summary>
        public DataPoint (string _name, float _value, int _index)
        {
            name = _name;
            value = _value;
            index = _index;
        }

        /// <summary>
        /// Constructor of <typeparamref name="DataPoint"/>.
        /// </summary>
         public DataPoint (string _name, float _value, int _index, string _extraInfo)
        {
            name = _name;
            value = _value;
            index = _index;
            extraInfo = _extraInfo;
        }

        #endregion


    } /// End of Class


} /// End of Namespace