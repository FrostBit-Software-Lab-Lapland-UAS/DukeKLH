/******************************************************************************
 * File        : MeshUtility.cs
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
    /// Static class containing useful Mesh methods.
    /// </summary>
    public static class MeshUtility
    {

        /// <summary>
        /// Offset each vertex of a Mesh to shift the origin point.
        /// </summary>
        /// <param name="_originalMesh">The mesh in question.</param>
        /// <param name="_offset">Delta position of origin point.</param>
        public static void OffsetMeshOrigin (Mesh _originalMesh, Vector3 _offset)
        {
            Vector3[] verts = _originalMesh.vertices;

            for(int i = 0; i < verts.Length; i++) {

                verts[i] += _offset;
            }

            _originalMesh.vertices = verts;
        }

        /// <summary>
        /// Create a copy of <paramref name="_originalMesh"/>. 
        /// </summary>
        /// <param name="_originalMesh">The mesh to be copied.</param>
        /// <returns>A new instance of the provided mesh.</returns>
        public static Mesh CopyMesh (Mesh _originalMesh)
        {
            Mesh newMesh = new Mesh();
            newMesh.vertices = _originalMesh.vertices;
            newMesh.triangles = _originalMesh.triangles;
            newMesh.normals = _originalMesh.normals;
            newMesh.tangents = _originalMesh.tangents;
            newMesh.uv = _originalMesh.uv;
            newMesh.uv2 = _originalMesh.uv2;
            newMesh.bounds = _originalMesh.bounds;

            return newMesh;
        }


    } /// End of Class


} /// End of Namespace