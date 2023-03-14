/******************************************************************************
 * File        : FPSCounter.cs
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
    /// Calculate current FPS and display it with TextMeshPro.
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class FPSCounter : MonoBehaviour
    {
        #region Variables

        [SerializeField] float updateInterval;
        TextMeshPro textObj;
        Coroutine ongoingCoroutine;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        void OnEnable () 
        {
            textObj = GetComponent<TextMeshPro>();

            if (null == ongoingCoroutine) { 
                
                updateInterval = Mathf.Max(updateInterval, 0.05f);
                ongoingCoroutine = StartCoroutine(ShowFPS(updateInterval)); 
            }
        }

        void OnDisable()
        {
            StopAllCoroutines();
            ongoingCoroutine = null;
        }

        #endregion
        #region FPS Updating Methods

        /// <summary>
        /// Get a color dependent on <paramref name="_fps"/>. 
        /// </summary>
        /// <param name="_fps">Current framerate with which the proper color is determined.</param>
        /// <returns></returns>
        Color GetColor (int _fps) {

            if (_fps < 45)      { return Color.Lerp(Color.red, Color.yellow, (_fps / 45f)); } 
            else if (_fps < 90) { return Color.Lerp(Color.yellow, Color.green, (_fps / 90f)); } 
            else                { return Color.Lerp(Color.green, Color.cyan, (_fps / 120f)); }
        }

        /// <summary>
        /// Display the current framerate and update it with <paramref name="_interval"/>-sized increments.
        /// </summary>
        /// <param name="_interval"></param>
        /// <returns></returns>
        IEnumerator ShowFPS (float _interval) 
        {
            float timer = _interval;
            float fps = 0f;
            int count = 0;

            while (true) {
            
                timer -= Time.deltaTime;
                fps += (1f / Time.deltaTime);
                count++;

                if (timer <= 0f) {

                    int val = (int)(fps/count);

                    textObj.color = GetColor(val);
                    textObj.text = "FPS: " + val.ToString();

                    timer = _interval;
                    fps = 0f;
                    count = 0;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        #endregion
        
        #endregion
        

    } /// End of Class


} /// End of Namespace