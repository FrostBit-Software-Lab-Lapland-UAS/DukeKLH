/******************************************************************************
 * File        : GraphDataset.cs
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
using DUKE.UI;
using DUKE.KLHData;


namespace DUKE.UI {


    /// <summary>
    /// Data form for Graph visualisation.
    /// </summary>
    [System.Serializable]
    public class GraphDataset : Dataset 
    {
        #region Variables

        [SerializeField] string[] dataColumnNames;
        [SerializeField] AnimationCurve debugCurve;

        #endregion


        #region Constructors

        /// <summary>
        /// Empty constructor of <typeparamref name="GraphDataset"/>.
        /// </summary>
        public GraphDataset () { }
        
        /// <summary>
        /// Constructor of <typeparamref name="GraphDataset"/>.
        /// </summary>
        public GraphDataset (List<DataPoint> _data, string _unit, string[] _columnNames = null, AnimationCurve _debugCurve = null)
        {
            data = _data;
            dataColumnNames = _columnNames;
            debugCurve = _debugCurve;
            Unit = _unit;           
        }

        #endregion


    } /// End of Class


} /// End of Namespace