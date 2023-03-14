/******************************************************************************
 * File        : InverseMeshCollision.cs
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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DUKE {


    /// <summary>
    /// Invert the attached MeshCollider's mesh.
    /// Used for defining areas with BlockMovement layer to prevent the user from floating through walls in desktop mode.
    /// NOTE: Remember to uncheck "Convex" from MeshCollider or the direction of normals does not matter.
    /// </summary>
    [RequireComponent(typeof(MeshCollider))]
    public class InverseMeshCollision : MonoBehaviour
    {
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            MeshCollider mCol = GetComponent<MeshCollider>();
            Mesh m = MeshUtility.CopyMesh(mCol.sharedMesh);

            if (null != m) {

                int[] tris = m.triangles;

                Array.Reverse(tris);

                m.triangles = tris;
                mCol.sharedMesh = m;
            }      
        }


    } /// End of Class


} /// End of Namespace