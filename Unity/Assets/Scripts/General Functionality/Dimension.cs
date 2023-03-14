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
using TMPro;


namespace DUKE {

    /// <summary>
    /// Draw a dimension line on an axis and display its length.
    /// </summary>
    public class Dimension : MonoBehaviour
    {
        #region Variables

        [SerializeField] Transform baseTransform;
        [SerializeField] Transform pointA, pointB;
        [SerializeField] SnapAxis axis;
        [SerializeField] LineRenderer line;
        [SerializeField] TextMeshPro text; 
        [SerializeField] Vector3 lineOffset = Vector3.up * 0.025f;
        [SerializeField] Vector3 textOffset = Vector3.up * 0.03f;


        [Header("Visual Settings")]
        [SerializeField] Material material;
        Material mInstance;
        [SerializeField] Color color;
        [SerializeField] float lineWidth;
        [SerializeField] float endPointSize = 0.01f;

        #endregion



        #region Methods

        /// <summary>
        /// Update the visuals.
        /// </summary>
        /// <param name="_dimensions">Dimensions from which a single axis is used.</param>
        public void UpdateDimension (Vector3 _dimensions)
        {
            #region Set material

            if (null == mInstance)
            {
                mInstance = new Material(material);
                line.material = mInstance;
                pointA.GetComponent<MeshRenderer>().material = mInstance;
                pointB.GetComponent<MeshRenderer>().material = mInstance;
            }

            mInstance.SetColor("_Color", color);

            #endregion

            #region Set line

            Vector3 direction = axis switch
            {
                SnapAxis.X => Vector3.right,
                SnapAxis.Y => Vector3.up,
                SnapAxis.Z => Vector3.forward,
                _ => Vector3.one
            };

            float distance = axis switch
            {
                SnapAxis.X => _dimensions.x,
                SnapAxis.Y => _dimensions.y,
                SnapAxis.Z => _dimensions.z,
                _ => 0f
            };

            line.positionCount = 2;
            line.startWidth = line.endWidth = lineWidth;
            line.SetPosition(0, lineOffset);
            line.SetPosition(1, lineOffset + (direction * distance));  

            pointA.localPosition = line.GetPosition(0);
            pointB.localPosition = line.GetPosition(1);
            pointA.localScale = Vector3.one * endPointSize;
            pointB.localScale = Vector3.one * endPointSize;

            #endregion

            #region Set text

            Vector3 lookFwd = axis switch
            {
                SnapAxis.X => Vector3.down,
                SnapAxis.Y => Vector3.forward,
                SnapAxis.Z => Vector3.down,
                _ => Vector3.one
            };

            Vector3 lookUp = axis switch
            {
                SnapAxis.X => Vector3.forward,
                SnapAxis.Y => Vector3.left,
                SnapAxis.Z => Vector3.left,
                _ => Vector3.one
            };

            Vector3 scaledDimension = _dimensions * BuildingDrawer.BuildingScale;
      
            text.text = axis switch
            {
                SnapAxis.X => KLHManager.FormatFloatToString(scaledDimension.x, 0) + " m",
                SnapAxis.Y => KLHManager.FormatFloatToString(scaledDimension.y, 1) + " m",
                SnapAxis.Z => KLHManager.FormatFloatToString(scaledDimension.z, 0) + " m",
                _ => "INVALID"
            };
       
            text.transform.localPosition = textOffset + (direction * distance / 2f);
            text.transform.LookAt(text.transform.position + lookFwd, lookUp);

            #endregion
        }

        #endregion


    } /// End of Class


} /// End of Namespace