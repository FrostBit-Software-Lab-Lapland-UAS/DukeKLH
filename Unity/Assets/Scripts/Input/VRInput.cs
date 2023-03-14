/******************************************************************************
 * File        : VRInput.cs
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
using UnityEngine.InputSystem;


namespace DUKE.Controls {

    /// <summary>
    /// VR interaction buttons.
    /// </summary>
    public enum VRInteractionButton 
    {
        Highlight ,
        Trigger,
        Grip,
        Primary2DClick,
        Primary2DTouch
    }

    /// <summary>
    /// Input source devices.
    /// </summary>
    public enum InputSource 
    {
        VR_Right,
        VR_Left,
        Headset,
        Mouse
    }


    /// <summary>
    /// Attached to a VR controller object in hierarchy. 
    /// VRInput receives input data through InputActions and converts them into Commands. 
    /// Commands are then sent forward and processed or discarded depending on the target object.
    /// Use DesktopInput for desktop functionality.
    /// </summary>
    public class VRInput : Input 
    {
        #region Variables

        #region InputAction Variables

        InputAction primaryButton;
        InputAction primaryTouch;
        InputAction secondaryButton;
        InputAction secondaryTouch;
        InputAction primary2DAxisClick;
        InputAction primary2DAxisTouch;
        InputAction primary2DAxisValue;
        InputAction secondary2DAxisClick;
        InputAction secondary2DAxisTouch;
        InputAction secondary2DAxisValue;
        InputAction triggerPress;
        InputAction triggerTouch;
        InputAction triggerValue;
        InputAction gripPress;
        InputAction gripValue;
        InputAction pointerPosition;
        InputAction pointerRotation;
        InputAction gripPosition;
        InputAction gripRotation;
        InputAction menu;
        InputAction haptic;

        #endregion
        #region Settings Variables

        [Header("Input Action Settings")]
        [SerializeField] InputActionAsset inputActions;
        [SerializeField] InputSource hand;
        bool referencesSet = false;
        Transform visibleElements;
        [SerializeField] int idleTimer = 10;
        Coroutine idleCoroutine;


        [Space(10f)]
        [Header("Button Settings")]
        [SerializeField] float triggerHeldTreshold = 0.5f;


        [Space(10f)]
        [Header("Raycast Settings")]
        RaycastHit rHit = new RaycastHit();
        RaycastHit bHit = new RaycastHit();
        Vector3 oldHitPos;


        [Space(10f)]
        [Header("Touch Settings")]
        [SerializeField] float touchRadius;
        [SerializeField] LayerMask touchHitLayers;
        Transform touchPointOrigin;
        Vector3 oldTouchPos;
        List<Transform> touchTransforms = new List<Transform>();


        [Space(10f)]
        [Header("Smoothing Settings")]
        [SerializeField] bool smoothPosition = false;
        [SerializeField] bool smoothRotation = false;
        [SerializeField] int smoothPositionsCount = 3;
        [SerializeField] int smoothRotationsCount = 3;
        [SerializeField] Vector3[] smoothPositions;
        [SerializeField] Quaternion[] smoothRotations;
        [SerializeField] bool resetSmoothArrays = false;

        #if UNITY_EDITOR
        [Space(10f), SerializeField] bool drawGizmos = false;
        #endif

        #endregion

        #endregion


        #region Properties

        /// <summary>
        /// <typeparamref name="Transform"/> this <typeparamref name="VRInput"/> is overlapping with.
        /// </summary>
        public Transform OverlapTarget 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// <typeparamref name="Transform"/> this <typeparamref name="VRInput"/> is pointing at with <paramref name="PointerRay"/>.
        /// </summary>
        public override Transform PointerTarget 
        { 
            get { return rHit.transform; } 
        }

        /// <summary>
        /// <typeparamref name="Ray"/> through which <typeparamref name="Interactables"/> are detected.
        /// </summary>
        public override Ray PointerRay 
        { 
            get { return new Ray(transform.position, transform.forward); } 
        }

        /// <summary>
        /// <typeparamref name="Hit"/> information of <paramref name="PointerRay"/>.
        /// </summary>
        public override RaycastHit Hit 
        { 
            get { return rHit; } 
        }

        /// <summary>
        /// TRUE when <paramref name="PointerRay"/> is blocked by a blocking layer.
        /// </summary>
        public bool RayIsBlocked
        {
            get;
            protected set;
        }

        /// <summary>
        /// World position of the controller object.
        /// </summary>
        public Vector3 ControllerPosition 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// World position delta of the controller object since last frame.
        /// </summary>
        public Vector3 ControllerPositionDelta 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// Rotation of the controller object.
        /// </summary>
        public Quaternion ControllerRotation 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// Rotation delta of the controller object since last frame.
        /// </summary>
        public Quaternion ControllerRotationDelta 
        { 
            get; 
            private set; 
        }
     
        /// <summary>
        /// World position of the pointer (controller position).
        /// </summary>
        public Vector3 PointerPosition 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// World position delta of the pointer (controller position) since last frame.
        /// </summary>
        public Vector3 PointerPositionDelta 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// Rotation of the pointer (controller rotation).
        /// </summary>
        public Quaternion PointerRotation 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// Rotation delta of the pointer (controller rotation) since last frame.
        /// </summary>
        public Quaternion PointerRotationDelta 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Last known position of <paramref name="Hit"/>.<paramref name="point"/>.
        /// </summary>
        public Vector3 RayPointPosition  
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// Position delta of <paramref name="RayPointPosition"/> since last frame.
        /// </summary>
        public Vector3 RayPointPositionDelta 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// World position of <paramref name="touchPointOrigin"/>.
        /// </summary>
        public Vector3 TouchPointPosition 
        { 
            get { return touchPointOrigin.position; } 
        }
        
        /// <summary>
        /// World position delta of <paramref name="touchPointOrigin"/> since last frame.
        /// </summary>
        public Vector3 TouchPointPositionDelta 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// Current value of primary 2D axis.
        /// </summary>
        public Vector2 Primary2DAxisValue 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// TRUE when primary interaction button (<paramref name="TriggerHeld"/>) is being held.
        /// </summary>
        public override bool PrimaryInteractionHeld 
        { 
            get { return TriggerHeld; } 
        }
        
        /// <summary>
        /// TRUE when trigger button is being held all the way.
        /// </summary>
        public bool TriggerPressed 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// TRUE when trigger button is being held over <paramref name="triggerHeldTreshold"/> value.
        /// </summary>
        public bool TriggerHeld 
        { 
            get { return TriggerValue >= triggerHeldTreshold; } 
        }
        
        /// <summary>
        /// TRUE when grip button is being pressed down.
        /// </summary>
        public bool GripPressed 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// TRUE when menu button is being pressed down.
        /// </summary>
        public bool MenuPressed 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// TRUE when primary button is being pressed down.
        /// </summary>
        public bool PrimaryButtonPressed 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// TRUE when primary button is being touched.
        /// </summary>
        public bool PrimaryButtonTouched 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// TRUE when primary 2D axis is being pressed down.
        /// </summary>
        public bool Primary2DAxisPressed 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// TRUE when primary 2D ais is being touched.
        /// </summary>
        public bool Primary2DAxisTouched 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// Current trigger button value.
        /// </summary>
        public float TriggerValue 
        { 
            get; 
            private set; 
        }
        
        /// <summary>
        /// Current distance of <paramref name="PointerRay"/>.
        /// </summary>
        public float RayLength 
        { 
            get;
            protected set;
        }
        
        /// <summary>
        /// Current radius of the touch area originating from <paramref name="touchPointOrigin"/>.
        /// </summary>
        public float TouchRadius 
        {
            get { return touchRadius; } 
        }

        #endregion


        #region Events

        /// <summary>
        /// Called when trigger button is pressed down.
        /// </summary>
        public Action TriggerDown;

        /// <summary>
        /// Called when trigger button is released.
        /// </summary>
        public Action TriggerUp;

        /// <summary>
        /// Called when primary 2D axis is pressed down.
        /// </summary>
        public Action Primary2DAxisDown;

        /// <summary>
        /// Called when primary 2D axis is released.
        /// </summary>
        public Action Primary2DAxisUp;

        #endregion


        #region Methods

        #region MonoBehaviour Methods    

        void Awake ()
        {
            if ((int)hand > 1) hand = InputSource.VR_Right;

            FindReferences();

            smoothPositions = new Vector3[smoothPositionsCount];
            smoothRotations = new Quaternion[smoothRotationsCount];
        }
    
        void Update ()
        {
            if (resetSmoothArrays) 
            {
                resetSmoothArrays = false;
                smoothPositions = new Vector3[smoothPositionsCount];
                smoothRotations = new Quaternion[smoothRotationsCount];
            }

            if (!InputActive) { return; }

            FindHighestPriorityTarget();
        }
        
        void OnEnable ()
        {
            if (!inputActions.enabled) 
            { 
                inputActions.Enable(); 
            }

            SubscribeToInput();

            if (null == idleCoroutine) 
            { 
                idleCoroutine = StartCoroutine(IdleTimeoutLoop()); 
            }
        }
        
        void OnDisable ()
        {
            UnsubscribeFromInput();

            if (inputActions.enabled) 
            {
                inputActions.Disable();
            }

            StopAllCoroutines();
            idleCoroutine = null;
        }
        
        #if UNITY_EDITOR
        void OnDrawGizmos ()
        {
            if (!drawGizmos) { return; }

            if (null == touchPointOrigin) 
            {
                touchPointOrigin = transform.Find("TouchPoint Origin");
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(touchPointOrigin.position, touchRadius);

            Gizmos.color = new Color(1f, 0.5f, 0.1f, 1f);
            Gizmos.DrawLine(transform.position, transform.position + PointerRay.direction * rayDistance);
        }
        #endif

        #endregion
        #region Event Handling Methods

        /// <summary>
        /// Find references to input events.
        /// </summary>
        void FindReferences ()
        {
            primaryButton = inputActions.actionMaps[(int)hand].FindAction("PrimaryButton");
            primaryTouch = inputActions.actionMaps[(int)hand].FindAction("PrimaryTouch");
            secondaryButton = inputActions.actionMaps[(int)hand].FindAction("SecondaryButton");
            secondaryTouch = inputActions.actionMaps[(int)hand].FindAction("SecondaryTouch");

            primary2DAxisClick = inputActions.actionMaps[(int)hand].FindAction("Primary2DAxisClick");
            primary2DAxisTouch = inputActions.actionMaps[(int)hand].FindAction("Primary2DAxisTouch");
            primary2DAxisValue = inputActions.actionMaps[(int)hand].FindAction("Primary2DAxis");
            secondary2DAxisClick = inputActions.actionMaps[(int)hand].FindAction("Secondary2DAxisClick");
            secondary2DAxisTouch = inputActions.actionMaps[(int)hand].FindAction("Secondary2DAxisTouch");
            secondary2DAxisValue = inputActions.actionMaps[(int)hand].FindAction("secondary2DAxis");

            triggerPress = inputActions.actionMaps[(int)hand].FindAction("TriggerPress");
            triggerTouch = inputActions.actionMaps[(int)hand].FindAction("TriggerTouch");
            triggerValue = inputActions.actionMaps[(int)hand].FindAction("Trigger");

            gripPress = inputActions.actionMaps[(int)hand].FindAction("GripPress");
            gripValue = inputActions.actionMaps[(int)hand].FindAction("Grip");

            pointerPosition = inputActions.actionMaps[(int)hand].FindAction("PointerPosition");
            pointerRotation = inputActions.actionMaps[(int)hand].FindAction("PointerRotation");
            gripPosition = inputActions.actionMaps[(int)hand].FindAction("GripPosition");
            gripRotation = inputActions.actionMaps[(int)hand].FindAction("GripRotation");

            menu = inputActions.actionMaps[(int)hand].FindAction("Menu");
            haptic = inputActions.actionMaps[(int)hand].FindAction("Haptic");


            touchPointOrigin = transform.Find("TouchPoint Origin");
            visibleElements = transform.Find("VisibleElements");

            referencesSet = true;
        }
        
        /// <summary>
        /// Subscribe to input events.
        /// </summary>
        void SubscribeToInput ()
        {
            if (!referencesSet) 
            {
                FindReferences();
            }

            triggerPress.started += OnTriggerPressed;
            triggerPress.canceled += OnTriggerPressed;
            triggerValue.performed += UpdateTriggerValue;
            triggerValue.canceled += UpdateTriggerValue;
            primaryButton.started += OnPrimaryButtonPressed;
            primaryButton.canceled += OnPrimaryButtonPressed;
            gripPress.started += OnGripPressed;
            gripPress.canceled += OnGripPressed;

            primary2DAxisClick.started += OnPrimary2DAxisPressed;
            primary2DAxisClick.canceled += OnPrimary2DAxisPressed;
            primary2DAxisTouch.performed += OnPrimary2DAxisTouched;
            primary2DAxisTouch.canceled += OnPrimary2DAxisTouched;
            primary2DAxisValue.performed += UpdatePrimary2DAxisValue;
            primary2DAxisValue.canceled += UpdatePrimary2DAxisValue;

            pointerPosition.performed += UpdatePointerPosition;
            pointerRotation.performed += UpdatePointerRotation;
            gripPosition.performed += UpdateControllerPosition;
            gripRotation.performed += UpdateControllerRotation;
        }
        
        /// <summary>
        /// Unsubscribe from input events.
        /// </summary>
        void UnsubscribeFromInput ()
        {
            triggerPress.started -= OnTriggerPressed;
            triggerPress.canceled -= OnTriggerPressed;
            triggerValue.performed -= UpdateTriggerValue;
            triggerValue.canceled -= UpdateTriggerValue;
            primaryButton.started -= OnPrimaryButtonPressed;
            primaryButton.canceled -= OnPrimaryButtonPressed;
            gripPress.started -= OnGripPressed;
            gripPress.canceled -= OnGripPressed;

            primary2DAxisClick.started -= OnPrimary2DAxisPressed;
            primary2DAxisClick.canceled -= OnPrimary2DAxisPressed;
            primary2DAxisTouch.performed -= OnPrimary2DAxisTouched;
            primary2DAxisTouch.canceled -= OnPrimary2DAxisTouched;
            primary2DAxisValue.performed -= UpdatePrimary2DAxisValue;
            primary2DAxisValue.canceled -= UpdatePrimary2DAxisValue;

            pointerPosition.performed -= UpdatePointerPosition;
            pointerRotation.performed -= UpdatePointerRotation;
            gripPosition.performed -= UpdateControllerPosition;
            gripRotation.performed -= UpdateControllerRotation;
        }
        
        /// <summary>
        /// Check if the controller has not moved recently and hide it in the game until it starts moving again.
        /// </summary>
        IEnumerator IdleTimeoutLoop ()
        {
            Vector3 prevControllerPosition = ControllerPosition;
            int secondsBeforeDisabled = idleTimer;

            while (isActiveAndEnabled) 
            {
                if ( Vector3.Distance(prevControllerPosition, ControllerPosition) < 0.02f ) 
                {
                    secondsBeforeDisabled -= 1;

                    if (secondsBeforeDisabled == 0) 
                    {     
                        #if UNITY_EDITOR
                        Debug.LogWarning("Controller ("+hand.ToString()+") disabled.");
                        #endif
                                          
                        SetInputActive(false);
                    }
                } 
                else 
                {
                    prevControllerPosition = ControllerPosition;
                    secondsBeforeDisabled = idleTimer;
                    SetInputActive(true);
                }

                yield return new WaitForSeconds(1);
            }
        }
        
        /// <summary>
        /// Set the controller activity.
        /// </summary>
        /// <param name="_active">TRUE if this instance should be set active.</param>
        void SetInputActive (bool _active)
        {
            InputActive = _active;
            visibleElements.gameObject.SetActive(InputActive);
        }



        /// <summary>
        /// Called when triggerPressed is started or canceled.
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void OnTriggerPressed (InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Started) 
            {
                TriggerPressed = true;
                TriggerDown?.Invoke();
            } 
            else if (_context.phase == InputActionPhase.Canceled) 
            {
                TriggerPressed = false;
                TriggerUp?.Invoke();
            }
        }
        
        /// <summary>
        /// Called when triggerPressed is started or canceled;
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void OnGripPressed (InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Started)       { GripPressed = true; } 
            else if (_context.phase == InputActionPhase.Canceled) { GripPressed = false; }
        }
        
        /// <summary>
        /// Called when triggerValue is performed or canceled.
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void UpdateTriggerValue (InputAction.CallbackContext _context)
        {
            TriggerValue = triggerValue.ReadValue<float>();
        }
        
        /// <summary>
        /// Called when pointerPosition is performed.
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void UpdatePointerPosition (InputAction.CallbackContext _context)
        {
            Vector3 oldPos = PointerPosition;
            PointerPosition = _context.ReadValue<Vector3>();
            PointerPositionDelta = PointerPosition - oldPos;
        }
               
        /// <summary>
        /// Called when pointerRotation is performed.
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void UpdatePointerRotation (InputAction.CallbackContext _context)
        {
            Quaternion oldRot = PointerRotation;
            Quaternion newRot = _context.ReadValue<Quaternion>();

            ControllerRotation = smoothRotation ? CalculateSmoothRotation(newRot) : newRot;
            PointerRotationDelta = PointerRotation * Quaternion.Inverse(oldRot);
            transform.localRotation = ControllerRotation;
        }
        
        

        /// <summary>
        /// Called when controllerPosition is performed.
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void UpdateControllerPosition (InputAction.CallbackContext _context)
        {
            Vector3 oldPos = ControllerPosition;
            Vector3 newPos = _context.ReadValue<Vector3>();

            ControllerPosition = smoothPosition ? CalculateSmoothPosition(newPos) : newPos;
            ControllerPositionDelta = ControllerPosition - oldPos;
            RayPointPosition = (rHit.distance > 0) ? rHit.point : RayPointPosition;
        
            transform.localPosition = ControllerPosition;

            TouchPointPositionDelta = touchPointOrigin.position - oldTouchPos;
            RayPointPositionDelta = RayPointPosition - oldHitPos;

            oldTouchPos = touchPointOrigin.position;
            oldHitPos = RayPointPosition;
        }
        
        /// <summary>
        /// Called when pointerRotation is performed.
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void UpdateControllerRotation (InputAction.CallbackContext _context)
        {
            Quaternion oldRot = ControllerRotation;
            ControllerRotation = _context.ReadValue<Quaternion>();
            ControllerRotationDelta = ControllerRotation * Quaternion.Inverse(oldRot);
        }
        
        /// <summary>
        /// Called when primaryButton is pressed down or released.       
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void OnPrimaryButtonPressed (InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Started) 
            {
                PrimaryButtonPressed = true;
            }
            else if (_context.phase == InputActionPhase.Canceled) 
            {
                PrimaryButtonPressed = false;
            }
        }
        
        /// <summary>
        /// Called when primary2DAxis is pressed down or released.
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void OnPrimary2DAxisPressed (InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Started) 
            {
                Primary2DAxisPressed = true;
                Primary2DAxisDown?.Invoke();
            } 
            else if (_context.phase == InputActionPhase.Canceled) 
            {
                Primary2DAxisPressed = false;
                Primary2DAxisUp?.Invoke();
            }
        }
        
        /// <summary>
        /// Called when primary2DAxis is touched.
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void OnPrimary2DAxisTouched (InputAction.CallbackContext _context)
        {
            if(_context.phase == InputActionPhase.Performed)
            {
                Primary2DAxisTouched = true; 
            }                    
            else if (_context.phase == InputActionPhase.Canceled)
            {
               Primary2DAxisTouched = false;
            }
        }
        
        /// <summary>
        /// Called when primary2DAxis is performed.
        /// </summary>
        /// <param name="_context"><typeparamref name="InputContext"/> instance containing information about input  commands.</param>
        void UpdatePrimary2DAxisValue (InputAction.CallbackContext _context)
        {
            Primary2DAxisValue = _context.ReadValue<Vector2>();
        }

        #endregion
        #region Smoothing Methods

        /// <summary>
        /// Calculate a smoothed position.
        /// </summary>
        /// <param name="_newPosition">Newest position to be added to the smoothing list.</param>
        Vector3 CalculateSmoothPosition (Vector3 _newPosition)
        {
            Vector3 totalVector = Vector3.zero;

            if (null == smoothPositions)            { smoothPositions = new Vector3[smoothPositionsCount]; }
            else if (smoothPositions.Length == 0)   { smoothPositions = new Vector3[smoothPositionsCount]; }

            for(int i = smoothPositions.Length - 1; i >= 0; i--) 
            {
                if (i == 0) { smoothPositions[i] = _newPosition; }
                else        { smoothPositions[i] = smoothPositions[i - 1]; }

                totalVector += smoothPositions[i];
            }

            return totalVector / smoothPositionsCount;
        }
        
        /// <summary>
        /// Calculate a smoothed rotation.
        /// </summary>
        /// <param name="_newRotation">Newest rotation to be added to the smoothing list.</param>
        /// <returns></returns>
        Quaternion CalculateSmoothRotation (Quaternion _newRotation)
        {
            if (null == smoothRotations)          { smoothRotations = new Quaternion[smoothRotationsCount]; } 
            else if (smoothRotations.Length == 0) { smoothRotations = new Quaternion[smoothRotationsCount]; }

            for (int i = smoothRotations.Length - 1; i >= 0; i--) 
            {
                if (i == 0) { smoothRotations[i] = _newRotation; }
                else        { smoothRotations[i] = smoothRotations[i - 1]; }
            }

            return AverageQuaternion(smoothRotations);
        }
        
        /// <summary>
        /// Calculate the average rotation.
        /// Source: https://forum.unity.com/threads/average-quaternions.86898/. hgdebarba, Jul 21, 2017.
        /// </summary>
        /// <param name="_quaternions">An array of rotations from which the average is calculated.</param>
        /// <returns>Average rotation.</returns>
        Quaternion AverageQuaternion (Quaternion[] _quaternions)
        {
            Quaternion averageQuaternion = _quaternions[0];
            float weight;

            for (int i = 1; i < _quaternions.Length; i++) 
            {
                weight = 1f / (i + 1);
                averageQuaternion = Quaternion.Slerp(averageQuaternion, _quaternions[i], weight);
            }

            return averageQuaternion;
        }

        #endregion
        #region Targeting, Raycast and Spherecast Methods

        /// <summary>
        /// Find the highest priority <typeparamref name="Transform"/> that is currently being pointed at or hovered over.
        /// </summary>
        void FindHighestPriorityTarget ()
        {
            PointerRaycast();
            OverlapSphereFromTouchPoint();

            Transform target;

            if (CanInteract)
            {
                if (null != OverlapTarget) 
                { 
                    target = OverlapTarget; 
                    currentInteractionMode = InteractionMode.Overlap; 
                }  
                else if (null != PointerTarget) 
                { 
                    target = PointerTarget;  
                    currentInteractionMode = InteractionMode.Raycast;    
                }  
                else 
                { 
                    target = null;
                    currentInteractionMode = (InteractionMode)0;
                }

                SetTargetTransform(target);
            }
        }
        
        /// <summary>
        /// Raycast and record hit information.
        /// </summary>
        void PointerRaycast ()
        {
            float dist = rayDistance;
            RayIsBlocked = false;

            if (Physics.Raycast(PointerRay, out bHit, rayDistance, rayBlockLayers)) 
            {
                dist = bHit.distance;
                RayIsBlocked = true;
            }
            else
            {
                bHit = new RaycastHit();
            }
            
            if (Physics.Raycast(PointerRay, out rHit, rayDistance, rayHitLayers)) 
            {
                if (rHit.distance <= (dist + 0.01f))
                {
                    RayIsBlocked = false;
                    dist = rHit.distance;
                }
                else
                {
                    rHit = new RaycastHit();
                }
            }
            else
            {
                rHit = new RaycastHit();
            }

            RayLength = dist - 0.1f;
        }
        
        /// <summary>
        /// Gather a list of colliders within the radius from touchPointOrigin and return the closest one's transform by using FindClosestOverlapToTouchPoint().
        /// </summary>
        void OverlapSphereFromTouchPoint ()
        {
            Collider[] cols = Physics.OverlapSphere(touchPointOrigin.position, touchRadius);

            if (null == cols) { OverlapTarget = null; }

            touchTransforms.Clear();

            foreach (Collider col in cols) 
            {
                if (touchHitLayers == (touchHitLayers | (1 << col.gameObject.layer)) && !touchTransforms.Contains(col.transform)) 
                { 
                    touchTransforms.Add(col.transform); 
                }
            }

            OverlapTarget = FindClosestOverlapToTouchPoint();
        }
        
        /// <summary>
        /// Find the object closest to the touchPointOrigin from touchTransforms-list.
        /// </summary>
        /// <returns></returns>
        Transform FindClosestOverlapToTouchPoint ()
        {
            float dist = Mathf.Infinity;
            Transform closest = null;

            for (int i = 0; i < touchTransforms.Count; i++) 
            {
                Transform t = touchTransforms[i];
                float d = Vector3.Distance(t.position, touchPointOrigin.position);

                if (d < dist) 
                {
                    dist = d;
                    closest = t;
                }
            }

            return closest;
        }

        #endregion
        
        #endregion


    } /// End of Namespace


} /// End of Class