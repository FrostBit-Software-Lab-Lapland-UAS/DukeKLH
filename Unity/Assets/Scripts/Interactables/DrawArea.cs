/******************************************************************************
 * File        : DrawArea.cs
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
using UnityEngine.Events;
using DUKE;


namespace DUKE.Controls {


    public class DrawArea : Interactable
    {
        #region Variables

        [SerializeField] UnityEvent OnDrawingStarted;
        [SerializeField] UnityEvent OnDrawingOngoing;
        [SerializeField] UnityEvent OnDrawingEnded;

        #endregion


        #region Events

        public Action DrawingStarted;
        public Action DrawingOngoing;
        public Action DrawingEnded;

        #endregion


        #region Methods

        /// <summary>
        /// Enable interaction if <paramref name="HasInteractionMode"/> returns TRUE.
        /// </summary>
        /// <param name="_source">Source <typeparamref name="Input"/> that called this method.</param>
        public override void BeginInteraction (Input _source)
        {
            base.BeginInteraction(_source);

            DrawingStarted?.Invoke();
            OnDrawingStarted?.Invoke();
        }

        /// <summary>
        /// Update loop for VR.
        /// </summary>
        protected override void VRInteractionUpdate ()
        {
            base.VRInteractionUpdate();

            //if (vrInput.TargetTransform == InteractableTransform)
            //{
                DrawingOngoing?.Invoke();
                OnDrawingOngoing?.Invoke();
           //}
        }

        /// <summary>
        /// Update loop for desktop.
        /// </summary>
        protected override void DesktopInteractionUpdate ()
        {
            base.DesktopInteractionUpdate();

            //if (mkbInput.TargetTransform == InteractableTransform)
            //{
                DrawingOngoing?.Invoke();
                OnDrawingOngoing?.Invoke();
            //}
        }

        /// <summary>
        /// Let <paramref name="source"/> <typeparamref name="Input"/> know this interaction should end.
        /// </summary>
        protected override void EndInteraction ()
        {
            DrawingEnded?.Invoke();
            OnDrawingEnded?.Invoke();
            
            base.EndInteraction();
        }

        #endregion


    } /// End of Class


} /// End of Namespace