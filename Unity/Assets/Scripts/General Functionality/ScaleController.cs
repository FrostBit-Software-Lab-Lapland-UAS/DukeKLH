/******************************************************************************
 * File        : ScaleController.cs
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


    public class ScaleController : MonoBehaviour
    {
        #region Variables

        [SerializeField] bool zeroOnStart = false;
        [SerializeField] bool activateOnstart = false;
        [SerializeField] Vector3 targetScale = Vector3.one;
        [SerializeField] float duration = 0f;
        [SerializeField] float delay = 0f;

        [SerializeField] AnimationCurve animationCurve;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        private void Start ()
        {
            if (zeroOnStart)
            {
                transform.localScale = Vector3.one * 0f;
            }

            if (animationCurve.keys.Length == 0) 
            {
                animationCurve.AddKey(0,0);
                animationCurve.AddKey(1,1);
            }

            if (activateOnstart) 
            {       
                ChangeScale(targetScale, duration, delay); 
            }
        }

        #endregion
        #region Scale Methods

        /// <summary>
        /// Change the scale of the transform with provided parameters.
        /// </summary>
        /// <param name="_newScale">Target scale of the object.</param>
        /// <param name="_duration">Duration of the animation in seconds.</param>
        /// <param name="_delay">Delay to start the animation in seconds.</param>
        public void ChangeScale (Vector3 _newScale, float _duration = 1f, float _delay = 0f)
        {
            StartCoroutine(ChangeScaleCoroutine(_newScale, _duration, _delay));
        }

        /// <summary>
        /// Change the scale of the transform with provided parameters.
        /// </summary>
        /// <param name="_newScale">Target scale of the object.</param>
        /// <param name="_duration">Duration of the animation in seconds.</param>
        /// <param name="_delay">Delay before starting the animation in seconds.</param>
        public void ChangeScale (float _newScale, float _duration = 1f, float _delay = 0f)
        {
            StartCoroutine(ChangeScaleCoroutine(Vector3.one * _newScale, _duration, _delay));
        }

        /// <summary>
        /// Animate the scale change.
        /// </summary>
        /// <param name="_newScale">Target scale of the object.</param>
        /// <param name="_duration">Duration of the animation in seconds.</param>
        /// <param name="_delay">Delay before starting the animation in seconds.</param>
        /// <returns></returns>
        IEnumerator ChangeScaleCoroutine (Vector3 _newScale, float _duration, float _delay)
        {
            float ratio = 0f;
            float duration = (_duration == 0f ? 0.001f : _duration); /// Avoid division by zero.
            Vector3 startScale = transform.localScale;

            if (_delay > 0f) { yield return new WaitForSeconds(_delay); }

            while (ratio < 1f) {

                ratio = Mathf.Clamp01(ratio + Time.deltaTime / _duration);
                transform.localScale = _newScale * animationCurve.Evaluate(ratio);

                yield return new WaitForEndOfFrame();
            }

            /// Retroactively build a collider if a Clickable is found.
            if (transform.TryGetComponent(out DUKE.Controls.Clickable clickable)) {

                clickable.SetupColliderForUIElement();
            }
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace