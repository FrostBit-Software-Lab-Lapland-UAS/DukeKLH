/******************************************************************************
 * File        : Movable.cs
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
using System;


namespace DUKE.Controls {


    /// <summary>
    /// Allows moving of the specified Transform.
    /// </summary>
    public class Movable : Interactable 
    {
        #region Variables

        #region General Variables

        [Space(10f)]
        [SerializeField] AxisFlags freeAxii;
        [SerializeField] WorldOrLocal axisOrientation;


        [Space(10f)]
        [SerializeField] bool limitMovementDistance = false;
        [SerializeField] Vector3 movementLimits;    /// Allowed movement distance per axis.
        [SerializeField] Vector3 limitRatios;       /// Current position on movementLimits per axis.
        [SerializeField] Vector3 allowedMovToPosDir;
        [SerializeField] Vector3 allowedMovToNegDir;
        [SerializeField, HideInInspector] Vector3 centerPosition;    /// Beginning position of the object.

        #endregion
        #region VRInput Variables

        [SerializeField, HideInInspector] bool waitingForInput = true;
        [SerializeField, HideInInspector] Vector3 pointerPos;
        [SerializeField, HideInInspector] Vector3 prevPos;
        [SerializeField, HideInInspector] float rayLength;

        #endregion
        #region Desktop Variables

        [SerializeField, HideInInspector] Vector3 clickPoint;
        [SerializeField, HideInInspector] Vector3 transformLocalClickPoint;
        [SerializeField, HideInInspector] Vector3 localCenterOffsetFromCamera;
        [SerializeField, HideInInspector] Ray prevPointerRay;
        [SerializeField, HideInInspector] float distanceToClickPoint;

        #endregion
        
        #endregion


        #region Properties

        /// <summary>
        /// TRUE when <paramref name="axisOrientation"/> is set to <typeparamref name="WorldOrLocal"/>.<typeparamref name="Local"/>.
        /// </summary>
        public bool OrientationIsLocal { 
            get { return axisOrientation == WorldOrLocal.Local; } }

        /// <summary>
        /// Per-axis multiplier of 0 or 1, used for locking axii.
        /// </summary>
        public Vector3 AxisMultiplier { 
            get { return GetAxisMultiplier(freeAxii); } }

        /// <summary>
        /// TRUE when allowed movement should have limits.
        /// </summary>
        public bool LimitMovement {
             get { return limitMovementDistance; } }

        /// <summary>
        /// Minimum point of movement limits, defining a corner of the area.
        /// </summary>
        protected Vector3 MinPoint {  
            get { return Vector3.zero
                - movementLimits.x / 2f *Vector3.right
                - movementLimits.y / 2f * Vector3.up
                - movementLimits.z / 2f * Vector3.forward; }  }

        /// <summary>
        /// Maximum point of movement limits, defining a corner of the area.
        /// </summary>
        protected Vector3 MaxPoint { 
            get { return Vector3.zero
                + movementLimits.x / 2f * Vector3.right
                + movementLimits.y / 2f * Vector3.up
                + movementLimits.z / 2f * Vector3.forward; } }

        /// <summary>
        /// Half of the distance of movement limits for each axis.
        /// </summary>
        protected Vector3 MovementLimitsHalf { 
            get { return movementLimits / 2f; } }

        protected bool IsOverlap
        {
            get { return source.IsVR && vrInput.currentInteractionMode == InteractionMode.Overlap; }
        }

        protected bool IsRaycast
        {
            get { return !IsOverlap; }
        }

        #endregion


        #region Events

        /// <summary>
        /// Event informing about position change. V3 = updated position.
        /// </summary>
        public Action<Vector3> PositionChanged;

        /// <summary>
        /// Event informing about limit ratios changing. V3 = updated ratios.
        /// </summary>
        public Action<Vector3> RatiosChanged;

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Called from inspector's button created in Movable_editor.
        /// </summary>
        public void RefreshMovementLimits ()
        {
            RecalculateCenterPosition();
        }

        public override void AddInput(Input _input)
        {
            // Skip adding the input if the interaction button is already held.
            if(!ValidateInput(_input)) 
            {
                base.AddInput(_input);
            }
        }

        #endregion
        #region MonoBehaviour Methods

        protected override void Start ()
        {
            base.Start();
            
            RecalculateCenterPosition();
        }

        #if UNITY_EDITOR
        void OnDrawGizmos ()
        {
            if (!Application.isPlaying)     { return; }
            if (!drawDebug)                 { return; }

            Gizmos.color = new Color(1, 0, 1, 1);
            Gizmos.DrawWireCube(prevPos, Vector3.one * 0.25f);

            if (limitMovementDistance) 
            {
                /// Draw boundaries:
                Gizmos.color = Color.yellow;

                if (OrientationIsLocal) 
                {
                    Gizmos.DrawLine(GetBoundaryPoint(0, 0, 0, true), GetBoundaryPoint(1, 0, 0, true));
                    Gizmos.DrawLine(GetBoundaryPoint(0, 1, 0, true), GetBoundaryPoint(1, 1, 0, true));
                    Gizmos.DrawLine(GetBoundaryPoint(0, 1, 1, true), GetBoundaryPoint(1, 1, 1, true));
                    Gizmos.DrawLine(GetBoundaryPoint(0, 0, 1, true), GetBoundaryPoint(1, 0, 1, true));

                    Gizmos.DrawLine(GetBoundaryPoint(0, 0, 0, true), GetBoundaryPoint(0, 1, 0, true));
                    Gizmos.DrawLine(GetBoundaryPoint(1, 0, 0, true), GetBoundaryPoint(1, 1, 0, true));
                    Gizmos.DrawLine(GetBoundaryPoint(1, 0, 1, true), GetBoundaryPoint(1, 1, 1, true));
                    Gizmos.DrawLine(GetBoundaryPoint(0, 0, 1, true), GetBoundaryPoint(0, 1, 1, true));

                    Gizmos.DrawLine(GetBoundaryPoint(0, 0, 0, true), GetBoundaryPoint(0, 0, 1, true));
                    Gizmos.DrawLine(GetBoundaryPoint(1, 0, 0, true), GetBoundaryPoint(1, 0, 1, true));
                    Gizmos.DrawLine(GetBoundaryPoint(1, 1, 0, true), GetBoundaryPoint(1, 1, 1, true));
                    Gizmos.DrawLine(GetBoundaryPoint(0, 1, 0, true), GetBoundaryPoint(0, 1, 1, true));
                } 
                else 
                {
                    Gizmos.DrawWireCube(centerPosition, movementLimits);
                }

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(centerPosition, 0.1f);

                Vector3 minP = GetBoundaryPoint(0, 0, 0, true);
                Vector3 maxP = GetBoundaryPoint(1, 1, 1, true);

                Gizmos.color = new Color(0.05f, 0.05f, 0.05f, 1f);
                Gizmos.DrawSphere(minP, 0.05f);

                Gizmos.color = new Color(1f, 1f, 1f, 1f);
                Gizmos.DrawSphere(maxP, 0.05f);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(minP, maxP);
            }
        }
        #endif
        
        #endregion
        #region Override Methods

        /// <summary>
        /// Enable interaction if <typeparamref name="HasInteractionMode"/> returns TRUE.
        /// </summary>
        /// <param name="_source"></param>
        public override void BeginInteraction (Input _source)
        {
            base.BeginInteraction(_source);

            if (_source.IsVR)
            {
                sourceInteractionMode = vrInput.currentInteractionMode;
                pointerPos = prevPos = sourceInteractionMode == InteractionMode.Overlap 
                    ? vrInput.TouchPointPosition 
                    : vrInput.Hit.point;
                rayLength = vrInput.Hit.distance;
            } 
            else if (_source.IsDesktop) 
            {
                pointerPos = prevPos = mkbInput.Hit.point;
                rayLength = mkbInput.Hit.distance;
            }
        }
        
        /// <summary>
        /// Update loop for VR.
        /// </summary>
        protected override void VRInteractionUpdate ()
        {
            base.VRInteractionUpdate();

            if (VRInteractionButtonHeld()) 
            {
                if (IsOverlap)
                {
                    pointerPos = vrInput.TouchPointPosition;
                    MoveObjectWithDelta(pointerPos - prevPos);
                    prevPos = pointerPos;                  
                }
                else
                {  
                    pointerPos = vrInput.transform.position + rayLength * vrInput.PointerRay.direction;
                    MoveObjectWithDelta(pointerPos - prevPos);
                    prevPos = pointerPos;
                }
            } 
            else 
            {
                waitingForInput = true;

                if (sourceInteractionMode == InteractionMode.Raycast) 
                {
                    if (null == vrInput.PointerTarget)             { EndInteraction(); } 
                    else if (vrInput.PointerTarget != transform)   { EndInteraction(); }
                } 
                else if (sourceInteractionMode == InteractionMode.Overlap) 
                {
                    if (null == vrInput.OverlapTarget)             { EndInteraction(); } 
                    else if (vrInput.OverlapTarget != transform)   { EndInteraction(); }
                }
            }
        }
        
        /// <summary>
        /// Update loop for desktop.
        /// </summary>
        protected override void DesktopInteractionUpdate ()
        {
            base.DesktopInteractionUpdate();

            if (mkbInput.LMBHeld && DesktopModifiersPressed()) 
            {
                if (mkbInput.RMBHeld) 
                {
                    if (!limitMovementDistance) 
                    { 
                        InteractableTransform.position = mkbInput.Camera.transform.TransformPoint(localCenterOffsetFromCamera); 
                    }

                    waitingForInput = true;
                } 
                else 
                {
                    if (waitingForInput) 
                    {
                        waitingForInput = false;
                        prevPos = mkbInput.MousePointPosition;
                        clickPoint = mkbInput.MousePointPosition;
                        transformLocalClickPoint = transform.InverseTransformPoint(clickPoint);
                        localCenterOffsetFromCamera = mkbInput.Camera.transform.InverseTransformPoint(transform.position);
                    }

                    Vector3 localStartPoint = mkbInput.Camera.transform.InverseTransformPoint(prevPos);
                    Vector3 localPointerRayDirection = mkbInput.Camera.transform.InverseTransformDirection(mkbInput.PointerRay.direction);

                    /// We are now in Camera's local coordinate system.

                    float depthDistance = localStartPoint.z;
                    Vector3 hrzRayDirection = new Vector3(localPointerRayDirection.x, 0f, localPointerRayDirection.z);
                    Vector3 vrtRayDirection = new Vector3(0f, localPointerRayDirection.y, localPointerRayDirection.z);

                    float signedXRad = Vector3.SignedAngle(Vector3.forward, hrzRayDirection, Vector3.up) * Mathf.Deg2Rad;
                    float signedYRad = Vector3.SignedAngle(Vector3.forward, vrtRayDirection, Vector3.left) * Mathf.Deg2Rad;

                    Vector3 localEndPoint = new Vector3(
                        Mathf.Tan(signedXRad) * depthDistance,
                        Mathf.Tan(signedYRad) * depthDistance,
                        depthDistance);

                    /// We are no longer in Camera's local coordinate system.

                    Vector3 worldEndPoint = mkbInput.Camera.transform.TransformPoint(localEndPoint);
                    Vector3 deltaV3 = worldEndPoint - prevPos;

                    MoveObjectWithDelta(deltaV3);

                    prevPos = worldEndPoint;
                }
            } 
            else 
            {
                waitingForInput = true;

                if (null == mkbInput.PointerTarget)           { EndInteraction(); } 
                else if (mkbInput.PointerTarget != transform) { EndInteraction(); }
            }
        }

        #endregion
        #region Movement Methods

        /// <summary>
        /// Move the object within setting limits.
        /// </summary>
        protected virtual void MoveObjectWithDelta (Vector3 _rawDelta)
        {
            Vector3 trueDelta = _rawDelta;
            Vector3 prevRatios = limitRatios;

            if (axisOrientation == WorldOrLocal.World) 
            {
                trueDelta = new Vector3(
                    _rawDelta.x * AxisMultiplier.x,
                    _rawDelta.y * AxisMultiplier.y,
                    _rawDelta.z * AxisMultiplier.z
                );
            } 
            else 
            {
                trueDelta =
                    InteractableTransform.right * AxisMultiplier.x * InteractableTransform.InverseTransformDirection(_rawDelta).x +
                    InteractableTransform.up * AxisMultiplier.y * InteractableTransform.InverseTransformDirection(_rawDelta).y +
                    InteractableTransform.forward * AxisMultiplier.z * InteractableTransform.InverseTransformDirection(_rawDelta).z;
            }

            if (limitMovementDistance)
            {
                bool usingLocalCoordinates = axisOrientation == WorldOrLocal.Local;

                Vector3 deltaVector = (usingLocalCoordinates)
                    ? InteractableTransform.InverseTransformDirection(trueDelta)
                    : trueDelta;

                Vector3 lossyScale = usingLocalCoordinates ? InteractableTransform.lossyScale : Vector3.one;

                float limitedX = deltaVector.x;
                float limitedY = deltaVector.y;
                float limitedZ = deltaVector.z;

                if (freeAxii.HasFlag(AxisFlags.X))
                {
                    limitedX = (deltaVector.x < 0 
                        ? Mathf.Max(deltaVector.x, -allowedMovToNegDir.x) 
                        : Mathf.Min(deltaVector.x, allowedMovToPosDir.x));                         
                }

                if (freeAxii.HasFlag(AxisFlags.Y))
                {
                    limitedY = (deltaVector.y < 0 
                        ? Mathf.Max(deltaVector.y, -allowedMovToNegDir.y) 
                        : Mathf.Min(deltaVector.y, allowedMovToPosDir.y)); 
                }

                if (freeAxii.HasFlag(AxisFlags.Z))
                {
                    limitedZ = (deltaVector.z < 0 
                        ? Mathf.Max(deltaVector.z, -allowedMovToNegDir.z) 
                        : Mathf.Min(deltaVector.z, allowedMovToPosDir.z));
                }

                Vector3 limitedVector = new Vector3(limitedX, limitedY, limitedZ);

                allowedMovToNegDir += limitedVector;
                allowedMovToPosDir -= limitedVector;

                trueDelta = usingLocalCoordinates ? InteractableTransform.TransformDirection(limitedVector) : limitedVector;
            }

            InteractableTransform.position += trueDelta;
            PositionChanged?.Invoke(InteractableTransform.position);
        }

        /// <summary>
        /// Calculate the center position of the limit area.
        /// </summary>
        void RecalculateCenterPosition ()
        {         
            Vector3 v = new Vector3(
                Mathf.Lerp(MaxPoint.x, MinPoint.x, limitRatios.x),
                Mathf.Lerp(MaxPoint.y, MinPoint.y, limitRatios.y),
                Mathf.Lerp(MaxPoint.z, MinPoint.z, limitRatios.z)
            );

            if (OrientationIsLocal) 
            { 
                v = InteractableTransform.TransformDirection(v); 
            }

            centerPosition = transform.position + v;
        }

        /// <summary>
        /// Get a corner point of the limit area.
        /// </summary>
        /// <param name="_x">0 for minimum point, otherwise for maximum point.</param>
        /// <param name="_y">0 for minimum point, otherwise for maximum point.</param>
        /// <param name="_z">0 for minimum point, otherwise for maximum point.</param>
        /// <param name="_addCenterPosition">Whether to get the point in world or local coordinates.</param>
        /// <returns>Corner point of the limit area.</returns>
        Vector3 GetBoundaryPoint (int _x, int _y, int _z, bool _addCenterPosition = false)
        {
            Vector3 v = new Vector3(
                _x == 0 ? MinPoint.x : MaxPoint.x,
                _y == 0 ? MinPoint.y : MaxPoint.y,
                _z == 0 ? MinPoint.z : MaxPoint.z);

            if (OrientationIsLocal) { v = InteractableTransform.TransformDirection(v); }
            if (_addCenterPosition) { v += centerPosition; }

            return v;
        }

        /// <summary>
        /// Get a Vector3 offset from limit area center.
        /// </summary>
        /// <returns>Offset vector from the limit area's center point.</returns>
        Vector3 GetLocalOffsetFromCenter ()
        {
            Vector3 v = InteractableTransform.InverseTransformPoint(centerPosition);
            v = new Vector3(
                v.x * transform.localScale.x,
                v.y * transform.localScale.y,
                v.z * transform.localScale.z);

            return v;
        }

        #endregion
        #region Button Methods
        
        /// <summary>
        /// Check if interaction button is held in VR.
        /// </summary>
        /// <returns></returns>
        bool VRInteractionButtonHeld ()
        {
            if (null == vrInput) { return false; }

            if ((vrInteractionButton == VRInteractionButton.Trigger && vrInput.TriggerHeld) || (vrInteractionButton == VRInteractionButton.Grip && vrInput.GripPressed))  
            {            
                return true; 
            }

            return false;
        }
        
        /// <summary>
        /// Check if interaction button is held in desktop mode.
        /// </summary>
        /// <returns></returns>
        bool DesktopModifiersPressed ()
        {
            bool flags = true;

            if (!Keyboard.current.leftAltKey.isPressed && desktopInteractionModifierFlags.HasFlag(DesktopInteractionModifier.LeftAlt)) 
            {
                flags = false;
            }
            
            if (!Keyboard.current.leftShiftKey.isPressed && desktopInteractionModifierFlags.HasFlag(DesktopInteractionModifier.LeftShift)) 
            {
                flags = false;
            }

            if (!Keyboard.current.leftCtrlKey.isPressed && desktopInteractionModifierFlags.HasFlag(DesktopInteractionModifier.LeftControl)) 
            {
                flags = false;
            }

            return flags;
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace