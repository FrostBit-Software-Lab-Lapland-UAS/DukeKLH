/******************************************************************************
 * File        : DesktopPlayerMovement.cs
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


using System.Collections.Generic;
using UnityEngine;


namespace DUKE.Controls {


    /// <summary>
    /// Attached to Player (Desktop) object in hierarchy.
    /// Controls the movement of player when in desktop mode using Rigidbody.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class DesktopPlayerMovement : MonoBehaviour 
    {
        #region Variables

        #region General Variables

        DesktopInput source;
        Transform camTransform;
        Rigidbody rb;

        /// <summary>
        /// Movement speed of the player.
        /// </summary>
        public float movementSpeed;

        /// <summary>
        /// Multiplier to <paramref name="movementSpeed"/> for vertical movement.
        /// </summary>
        public float verticalMovementMultiplier = 0.5f;

        /// <summary>
        /// Rotation speed of the player.
        /// </summary>
        public float cameraSpeed;

        /// <summary>
        /// TRUE when X-axis is inverted.
        /// </summary>
        public bool invertX = false;
        
        /// <summary>
        /// TRUE when Y-axis is inverted.
        /// </summary>
        public bool invertY = false;

        /// <summary>
        /// Current rotation around X-axis (vertical camera movement axis);
        /// </summary>
        float xRotation = 0;


        #endregion
        #region Smoothing Variables

        [SerializeField] int movementAcceleration = 10;
        float movementAccelerationRatio;
        Vector3 latestKnownKeyboardWorldMovement;

        #endregion

        #endregion
        

        #region Methods

        #region MonoBehaviour Methods

        private void Awake ()
        {
            if (!TryGetComponent(out rb))   
                { rb = transform.gameObject.AddComponent<Rigidbody>(); }

            camTransform = GetComponentInChildren<Camera>().transform;
            source = GetComponent<DesktopInput>();
        }
        
        void FixedUpdate ()
        {
            if (source.KeyboardWorldMovement != Vector3.zero) 
            {
                latestKnownKeyboardWorldMovement = source.KeyboardWorldMovement;

                AdjustMovementAcceleration(true);
                MovePlayer(latestKnownKeyboardWorldMovement);
            } 
            else if (movementAccelerationRatio > 0f) 
            {
                AdjustMovementAcceleration(false);
                MovePlayer(latestKnownKeyboardWorldMovement);
            }

            if (source.RMBHeld) { RotatePlayer(); }
        }

        #endregion
        #region Movement and Rotation Methods

        /// <summary>
        /// Move player's Rigidbody component with <paramref name="_movementDelta"/>.
        /// </summary>
        /// <param name="_movementDelta"></param>
        void MovePlayer (Vector3 _movementDelta)
        {
            Vector3 dir =
                camTransform.right * _movementDelta.x +
                camTransform.up * _movementDelta.y +
                camTransform.forward * _movementDelta.z;

            dir = dir.normalized;
            dir += camTransform.up * dir.y * (verticalMovementMultiplier - 1f);

            rb.MovePosition(transform.position + dir * Time.fixedDeltaTime * movementSpeed * movementAccelerationRatio);
        }

        /// <summary>
        /// Rotate the player.
        /// </summary>
        void RotatePlayer ()
        {
            Vector2 delta = UnityEngine.InputSystem.Mouse.current.delta.ReadValue() / 15.0f;
            float mouseX = (invertX ? -delta.x : delta.x) * cameraSpeed;
            float mouseY = (invertY ? -delta.y : delta.y) * cameraSpeed;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            camTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);       
        }
        
        /// <summary>
        /// Control the acceleration value to produce smooth movement.
        /// </summary>
        /// <param name="_increace">Increace or decreace acceleration?</param>
        void AdjustMovementAcceleration (bool _increace)
        {
            if (_increace)  { movementAccelerationRatio += Time.fixedDeltaTime * movementAcceleration; }
            else            { movementAccelerationRatio -= Time.fixedDeltaTime * movementAcceleration; }

            movementAccelerationRatio = Mathf.Clamp(movementAccelerationRatio, 0f, 1f);
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace