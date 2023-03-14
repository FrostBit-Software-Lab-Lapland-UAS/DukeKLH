/******************************************************************************
 * File        : IndicatorLineObject.cs
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
    /// Show a 3D wire cube when the user is drawing on the BuildingDrawer plane.
    /// </summary>
    public class IndicatorLineObject : MonoBehaviour
    {
        #region Variables

        [SerializeField] Transform[] cubeObjects;
        Material material;

        #endregion


        #region Methods

        void Start ()
        {
            SetMaterial();
        }

        /// <summary>
        /// Update the shape of the indicator object by scaling each Cube object to form rectangular frames.
        /// </summary>
        public void UpdateShape (Vector3 _scale, float _thickness)
        {
            for (int i = 0; i < cubeObjects.Length; i++) 
            {
                Transform cube = cubeObjects[i];
                float xPos = 0f;
                float yPos = 0f;
                float zPos = 0f;
                float length = 0f;

                if (cube.name.Contains("X")) 
                { 
                    length = _scale.x;
                    yPos = cube.name.Contains("0") ? 0f : _scale.y;
                    zPos = cube.name.Contains("A") ? 0f : _scale.z;
                } 
                else if (cube.name.Contains("Y")) 
                { 
                    length = _scale.y;
                    xPos = cube.name.Contains("0") ? 0f : _scale.x;
                    zPos = cube.name.Contains("A") ? 0f : _scale.z;
                }  
                else if (cube.name.Contains("Z")) 
                {
                    length = _scale.z;
                    xPos = cube.name.Contains("0") ? 0f : _scale.x;
                    yPos = cube.name.Contains("A") ? 0f : _scale.y;
                }

                cube.localPosition = new Vector3(xPos, yPos, zPos);
                cube.localScale = new Vector3( _thickness, length, _thickness);
            }
        }

        /// <summary>
        /// Set the <typeparamref name="Color"/> of Cubes.
        /// </summary>
        /// <param name="_color"></param>
        public void SetColor (Color _color)
        {
            SetMaterial();
            material.SetColor("_Color", _color);
        }

        protected void SetMaterial ()
        {
            if (null != material) { return; }

            material = Instantiate(Resources.Load("Materials/EmissiveColor") as Material);
            material.SetFloat("_Intensity", 1f);

            foreach (MeshRenderer mRend in transform.GetComponentsInChildren<MeshRenderer>()) 
            {
                mRend.material = material;
            }
        }

        /// <summary>
        /// Scale each Cube to zero.
        /// </summary>
        public void HideShape ()
        {
            for (int i = 0; i < cubeObjects.Length; i++) 
            {
                Transform cube = cubeObjects[i];

                cube.localPosition = Vector3.zero;
                cube.localScale = Vector3.zero;
            }
        }

        #endregion


    } /// End of Class


} /// End of Namespace