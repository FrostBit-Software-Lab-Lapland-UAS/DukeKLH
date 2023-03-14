/******************************************************************************
 * File        : ObjectDimensions.cs
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
using UnityEngine.UI;
using TMPro;


namespace DUKE {

    /// <summary>
    /// Selected source of the dimension. Implemented only partially.
    /// </summary>
    public enum DimensionMeasurementSource 
    {
        ObjectScale,
        MeshVertexPosition, /// Not implemented
        Custom              /// Not implemented
    }



    /// <summary>
    /// Display object's dimensions with lines and labels.
    /// </summary>
    public class ObjectDimensions : MonoBehaviour
    {
        #region Variables

        [SerializeField] List<DimensionLineControl> dimensionLines = new List<DimensionLineControl>();

        Vector3 minPoint;
        Vector3 maxPoint;

        

        #endregion


        #region Methods

        void OnDrawGizmos ()
        {
            Gizmos.color = new Color(0.75f, 0f, 1f, 1f);
            Gizmos.DrawCube(transform.position + minPoint, Vector3.one * 0.1f);

            Gizmos.color = new Color(0f, 1f, 0.75f, 1f);
            Gizmos.DrawSphere(transform.position + maxPoint, 0.065f);
        }

        /// <summary>
        /// Update the values of <typeparamref name="dimensionLines"/>.
        /// </summary>
        /// <param name="_delta">Total delta between start and end point.</param>
        public void UpdateDimensions(Vector3 _delta)
        {
            float heightAdjustment = 0.05f;
            Vector3 minPoint = Vector3.up * heightAdjustment;
            Vector3 maxPoint = new Vector3(
                Mathf.Abs(_delta.x),
                Mathf.Abs(_delta.y) + heightAdjustment,
                Mathf.Abs(_delta.z)
            );

            foreach(DimensionLineControl dlc in dimensionLines) 
            {
                dlc.SetScale(BuildingDrawer.BuildingScale);
                dlc.UpdateDimension(transform, minPoint, maxPoint);
            }
        }

        #endregion


    } /// End of Class



    /// <summary>
    /// Define the placement of a Dimension.
    /// </summary>
    [System.Serializable]
    public class DimensionLineControl 
    {
        #region Variables

        [SerializeField] DimensionLine dimension;
        [SerializeField] SnapAxis dimensionAxis;
        [SerializeField] SnapAxis offsetAxis;
        [SerializeField] float offsetOnAxis = 0.5f;

        #endregion


        #region Methods

        /// <summary>
        /// Update the values of the attached <typeparamref name="DimensionLine"/>. 
        /// </summary>
        /// <param name="_parent">Parent <typeparamref name="Transform"/> used to find base location.</param>
        /// <param name="_minPoint">Minimum position of the dimension.</param>
        /// <param name="_maxPoint">Maximum point of the dimension.</param>
        public void UpdateDimension (Transform _parent, Vector3 _minPoint, Vector3 _maxPoint)
        {
            /*
            Vector3 posA = _parent.position;
            Vector3 posB = posA;
            Vector3 delta = _maxPoint - _minPoint;
            float scale = BuildingDrawer.BuildingScale;

            Vector3 offset = new Vector3(
                offsetAxis.HasFlag(SnapAxis.X) ? 1f : 0f,
                offsetAxis.HasFlag(SnapAxis.Y) ? 1f : 0f,
                offsetAxis.HasFlag(SnapAxis.Z) ? 1f : 0f
            ) * offsetOnAxis;

            if (dimensionAxis.HasFlag(SnapAxis.X)) { posB += new Vector3(delta.x, 0f, 0f); }
            if (dimensionAxis.HasFlag(SnapAxis.Y)) { posB += new Vector3(0f, delta.y, 0f); }
            if (dimensionAxis.HasFlag(SnapAxis.Z)) { posB += new Vector3(0f, 0f, delta.z); }  
      
            posA += offset;
            posB += offset;

            dimension.SetPositions(posA, posB);
            */

            Vector3 delta = (_maxPoint - _minPoint);
            Vector3 offset = new Vector3(
                offsetAxis.HasFlag(SnapAxis.X) ? 1f : 0f,
                offsetAxis.HasFlag(SnapAxis.Y) ? 1f : 0f,
                offsetAxis.HasFlag(SnapAxis.Z) ? 1f : 0f
            ) * offsetOnAxis;

            Vector3 posA = offset;
            Vector3 posB = new Vector3(
                dimensionAxis.HasFlag(SnapAxis.X) ? delta.x : 0f,
                dimensionAxis.HasFlag(SnapAxis.Y) ? delta.y : 0f,
                dimensionAxis.HasFlag(SnapAxis.Z) ? delta.z : 0f
            ) + offset;

            dimension.SetPositions(posA, posB);
        }

        /// <summary>
        /// Set the building scale of the dimension.
        /// </summary>
        /// <param name="_scale">New scale to be set.</param>
        public void SetScale (float _scale)
        {
            dimension.BuildingScale = _scale;
        }

        #endregion


    } /// End of Class


} /// End of Namespace