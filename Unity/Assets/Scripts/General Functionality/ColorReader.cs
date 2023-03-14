/******************************************************************************
 * File        : ColorReader.cs
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


namespace DUKE {


    /// <summary>
    /// Read the color of a texture asset at the specified point.
    /// </summary>
    public static class ColorReader
    {
        /// <summary>
        /// Get the color of <param>_img</param> at the specified <param>_coordinates</param>.
        /// </summary>
        /// <param name="_img">Selected texture.</param>
        /// <param name="_coordinates">Specified coordinates.</param>
        /// <returns></returns>
        public static Color GetColor (Image _img, Vector2Int _coordinates)
        {
            Texture2D tex = (Texture2D)_img.mainTexture;
            return tex.GetPixel(_coordinates.x, _coordinates.y);
        }
        
        /// <summary>
        /// Get the color of <param>_img</param> at the specified <param>_ratio</param>.
        /// </summary>
        /// <param name="_img">Selected texture.</param>
        /// <param name="_ratio">Specified ratio between min and max coordinates.</param>
        /// <returns></returns>
        public static Color GetColor (Image _img, Vector2 _ratio)
        {
            Texture2D tex = (Texture2D)_img.mainTexture;
            Vector2Int coordinates = new Vector2Int(
                Mathf.RoundToInt(tex.width * _ratio.x), 
                Mathf.RoundToInt(tex.height * _ratio.y));

            return tex.GetPixel(coordinates.x, coordinates.y);
        }


    } /// End of Class


} /// End of Namespace