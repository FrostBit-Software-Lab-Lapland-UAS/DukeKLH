/******************************************************************************
 * File        : DataPointObject.cs
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


using UnityEngine;
using DUKE.KLHData;
using DUKE.UI;


namespace DUKE {

    /// <summary>
    /// Flags for selecting data from Graph.
    /// </summary>
    [System.Flags]
    public enum GraphDataSelection {
        Name    = (1 << 0),
        Value   = (1 << 1),
        Unit    = (1 << 2),
        Percent = (1 << 4)
    }



    /// <summary>
    /// Contains a single DataPoint. This component is mainly used for displaying Graph data information.
    /// </summary>
    public class DataPointObject : MonoBehaviour
    {
        #region Variables

        [SerializeField] GraphDataSelection selectedData;
        [SerializeField] DataPoint dataPoint;
        [SerializeField] Graph parentGraph;

        #endregion


        #region Properties

        /// <summary>
        /// Actual data of this object.
        /// </summary>
        public DataPoint DataPoint { 
            get { return dataPoint; }
            set { dataPoint = value; } }
        
        /// <summary>
        /// "Parent" <typeparamref name="Graph"/> of this object.
        /// </summary>
        public Graph Graph { 
            get { return parentGraph; } 
            set { parentGraph = value; } }
        
        /// <summary>
        /// Name of the data.
        /// </summary>
        public string DataName { 
            get { return Graph.name; } }
        
        /// <summary>
        /// Value of the data.
        /// </summary>
        public float Value { 
            get { return DataPoint.Value; } }
        
        /// <summary>
        /// Value of the data rounded to int.
        /// </summary>
        /// <value></value>
        public int ValueInt { 
            get { return Mathf.RoundToInt(DataPoint.Value); } }
        
        /// <summary>
        /// Value of the data converted to percentage of the total value of the associated <typeparamref name="GraphArea"/>.
        /// </summary>
        /// <value></value>
        public float Percent { 
            get { return CalculatePercent(); } }
        
        /// <summary>
        /// <paramref name="Percent"/> converted to string with " %" added.
        /// </summary>
        /// <value></value>
        public string PercentString { 
            get { return (Percent * 100).ToString() + " %"; } }

        /// <summary>
        /// Data selection of the data.
        /// </summary>
        public GraphDataSelection SelectedData {
            get { return selectedData; }
            set { selectedData = value; }}

        #endregion


        #region Methods

        /// <summary>
        /// Provide selected data as a string.
        /// </summary>
        public string DataLogString ()
        {
            string s = "";

            if (SelectedData.HasFlag(GraphDataSelection.Name))       { s += DataName; }
            if (SelectedData.HasFlag(GraphDataSelection.Value))      { s += s == "" ? ValueInt.ToString() : "\n"+ ValueInt.ToString(); }
            if (SelectedData.HasFlag(GraphDataSelection.Unit))       { s += selectedData.HasFlag(GraphDataSelection.Value) ? " "+Graph.Dataset.Unit.ToString() : (s == "" ? Graph.Dataset.Unit.ToString() : "\n"+Graph.Dataset.Unit.ToString());  }
            if (SelectedData.HasFlag(GraphDataSelection.Percent))    { s += s == "" ? Percent.ToString() : "\n"+Percent.ToString(); }

            return s;
        }

        /// <summary>
        /// Calculate the percentage of this point compared to all points with identical index within the same GraphArea. 
        /// (One bar compared against the whole stack.)
        /// </summary>
        /// <returns></returns>
        float CalculatePercent ()
        {
            //float total = Graph.GraphArea.TotalValueAtIndex(DataPoint.Index);
            float total = parentGraph.GraphArea.DataValueRange.y;

            return (float)System.Math.Round(DataPoint.Value / total, 4);
        }

        #endregion


    } /// End of Class


} /// End of Namespace