/******************************************************************************
 * File        : VRHeadset.cs
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
using UnityEngine.InputSystem;


namespace DUKE.Controls {


    /// <summary>
    /// Move the headset.
    /// </summary>
    public class VRHeadset : Input {

        #region Variables

        /// <summary>
        /// <typeparamref name="InputActionAsset"/> containing the used input.
        /// </summary>
        public InputActionAsset inputActions;
        

        InputAction headPosition;
        InputAction headRotation;

        #endregion


        #region Properties

        /// <summary>
        /// Position of the head.
        /// </summary>
        public Vector3 HeadPosition { 
            get; 
            private set; }

        /// <summary>
        /// Rotation of the head.
        /// </summary>
        public Quaternion HeadRotation { 
            get; 
            private set; }

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        private void OnEnable ()
        {
            FindReferences();
            SubscribeToInput();
        }

        private void OnDisable ()
        {
            UnsubscribeFromInput();
        }

        #endregion
        #region Event Handling 

        /// <summary>
        /// Find the required references.
        /// </summary>
        void FindReferences ()
        {
            if(null == headPosition)    headPosition = inputActions.actionMaps[2].FindAction("hmdPosition");
            if (null == headRotation)   headRotation = inputActions.actionMaps[2].FindAction("hmdRotation");
        }
       
        /// <summary>
        /// Add listeners to pertinent Events.
        /// </summary>
        void SubscribeToInput ()
        {
            FindReferences();

            InputSystem.onAfterUpdate += UpdateCallback;

            headPosition.performed += UpdateHeadPosition;
            headPosition.canceled += UpdateHeadPosition;
            headRotation.performed += UpdateHeadRotation;
            headRotation.canceled += UpdateHeadRotation;
        }

        /// <summary>
        /// Remove previously added listeners.
        /// </summary>
        void UnsubscribeFromInput ()
        {
            InputSystem.onAfterUpdate -= UpdateCallback;

            headPosition.performed -= UpdateHeadPosition;
            headPosition.canceled -= UpdateHeadPosition;
            headRotation.performed -= UpdateHeadRotation;
            headRotation.canceled -= UpdateHeadRotation;
        }

        /// <summary>
        /// Called when InputSystem's onAfterUpdate is sent. This way position and rotation are updated every loop as well as before render.
        /// </summary>
        void UpdateCallback ()
        {
            transform.localPosition = HeadPosition;
            transform.localRotation = HeadRotation;
        }

        /// <summary>
        /// Called when headPosition is performed or canceled.
        /// </summary>
        /// <param name="_context"></param>
        void UpdateHeadPosition (InputAction.CallbackContext _context)
        {
            HeadPosition = _context.ReadValue<Vector3>();
        }

        /// <summary>
        /// Called when headRotation is performed or canceled.
        /// </summary>
        /// <param name="_context"></param>
        void UpdateHeadRotation (InputAction.CallbackContext _context)
        {
            HeadRotation = _context.ReadValue<Quaternion>();
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace