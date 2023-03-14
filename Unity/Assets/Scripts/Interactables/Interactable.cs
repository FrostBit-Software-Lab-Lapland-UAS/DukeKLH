/******************************************************************************
 * File        : Interactable.cs
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
using System.Collections.Generic;
using UnityEngine;


namespace DUKE.Controls {


    /// <summary>
    /// Coordinate axis flags.
    /// </summary>
    [System.Flags]
    public enum AxisFlags {
        X = (1 << 0),
        Y = (1 << 1),
        Z = (1 << 2)
    }

    /// <summary>
    /// Coordinate selection.
    /// </summary>
    public enum WorldOrLocal {
        World,
        Local
    }

    /// <summary>
    /// Interaction mode flags.
    /// </summary>
    [System.Flags]
    public enum InteractionMode {
        Raycast = (1 << 0),
        Overlap = (1 << 1)
    }


    /// <summary>
    /// Base class for all interaction classes for VR and desktop.
    /// </summary>
    public class Interactable : MonoBehaviour 
    {
        #region Variables

        /// <summary>
        /// List of all <typeparamref name="Inputs"/> that are currently pointing at or hovering over this instance.
        /// </summary>
        [SerializeField] protected List<Input> hoveringInputs;

        /// <summary>
        /// TRUE when interaction should be enabled.
        /// </summary>
        [SerializeField] protected bool isInteractable = true;

        /// <summary>
        /// <typeparamref name="Transform"/> which is affected by the interaction. If left as NULL, this <typeparamref name="Transform"/> is used.
        /// </summary>
        [SerializeField] protected Transform interactableTransform;
        
        /// <summary>
        /// <typeparamref name="VRInteractionButton"/> used for this interaction.
        /// </summary>
        [SerializeField] protected VRInteractionButton vrInteractionButton = VRInteractionButton.Trigger;
        
        /// <summary>
        /// <typeparamref name="DesktopInteractionModifier"/> flags used for this interaction.
        /// </summary>
        [SerializeField] protected DesktopInteractionModifier desktopInteractionModifierFlags;
        
        /// <summary>
        /// Currently selected <typeparamref name="InteractionMode"/> flags. 
        /// </summary>
        [SerializeField] protected InteractionMode interactionMode = InteractionMode.Raycast;
        
        #if UNITY_EDITOR
        /// <summary>
        /// TRUE if debug visuals should be drawn. 
        /// </summary>
        [SerializeField] protected bool drawDebug;
        #endif

        
        /// <summary>
        /// TRUE when update loop should be active.
        /// </summary>
        [SerializeField, HideInInspector] protected bool updateEnabled = false;
        
        /// <summary>
        /// <typeparamref name="InteractionMode"/> of the <paramref name="source"/>.
        /// </summary>
        [SerializeField, HideInInspector] protected InteractionMode sourceInteractionMode;
        
        /// <summary>
        /// <typeparamref name="Input"/> that is currently bound to this <typeparamref name="Interactable"/>.
        /// </summary>
        [SerializeField, HideInInspector] protected Input source;
        
        /// <summary>
        /// <paramref name="source"/> cast as <typeparamref name="VRInput"/>.
        /// </summary>
        [SerializeField, HideInInspector] protected VRInput vrInput;
        
        /// <summary>
        /// <paramref name="source"/> cast as <typeparamref name="DesktopInput"/>.
        /// </summary>
        [SerializeField, HideInInspector] protected DesktopInput mkbInput;
        
        /// <summary>
        /// Configuration info for this type of <typeparamref name="Interactable"/>.
        /// </summary>
        [SerializeField, HideInInspector] protected string configurationInfo;

        #endregion


        #region Properties

        /// <summary>
        /// Current <typeparamref name="Input"/> instance.
        /// </summary>
        public Input Source
        {
            get { return source; }
        }

        /// <summary>
        /// <paramref name="Source"/> cast as <typeparamref name="VRInput"/> (null if this <typeparamref name="Input"/> is not <typeparamref name="VRInput"/>).
        /// </summary>
        public VRInput VRInput
        {
            get { return vrInput; }
        }

        /// <summary>
        /// <paramref name="Source"/> cast as <typeparamref name="DesktopInput"/> (null if this <typeparamref name="Input"/> is not <typeparamref name="DesktopInput"/>).
        /// </summary>
        public DesktopInput DesktopInput
        {
            get { return DesktopInput; }
        }

        /// <summary>
        /// <typeparamref name="Transform"/> which is affected by the interaction. If left as NULL, this <typeparamref name="Transform"/> is used.
        /// </summary>
        protected Transform InteractableTransform 
        { 
            get { return interactableTransform != null ? interactableTransform : transform; } 
        }

        /// <summary>
        /// TRUE when interaction should be enabled.
        /// </summary>
        public bool IsInteractable 
        { 
            get { return isInteractable; } 
            set { isInteractable = value; } 
        }

        /// <summary>
        /// TRUE when this instance is currently interacting with <paramref name="source"/>.
        /// </summary>
        public bool IsInteracting 
        {
            get { return null != source && isInteractable; } 
        }

        /// <summary>
        /// Configuration info for this type of <typeparamref name="Interactable"/>.
        /// </summary>
        public string ConfigInfo 
        { 
            get { return configurationInfo; } 
            set { configurationInfo = value; } 
        }

        protected InteractionMode SourceInteractionMode
        {
            get { return null == source ? (InteractionMode.Raycast) : source.currentInteractionMode; }
        }

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        protected virtual void Awake ()
        {
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            hoveringInputs = new List<Input>();
            updateEnabled = true;
        }

        protected virtual void Start()
        {
            if (!InteractableTransform.TryGetComponent(out Collider col))  
            {
                if (InteractableTransform.TryGetComponent(out DUKE.UI.CurvedUIElement cUIElement)) 
                {
                    if (cUIElement.CurvedMesh.vertexCount >= 3 && InteractableTransform.localScale != Vector3.zero) 
                    {
                        InteractableTransform.gameObject.AddComponent<MeshCollider>().sharedMesh = cUIElement.CurvedMesh;
                    }
                }
            }
        }

        protected virtual void OnEnable() 
        { 
            hoveringInputs.Clear();
        }

        protected virtual void OnDisable ()
        {      
            ForceEndInteraction();
        }

        protected virtual void Update ()
        {
            if (!IsInteractable) { return; }

            if (source)
            {
                if (ValidateInput(source))
                {
                    if (source.IsVR)            { VRInteractionUpdate(); } 
                    else if (source.IsDesktop)  { DesktopInteractionUpdate(); }
                }
                else
                {
                    EndInteraction();
                }
            }
            else
            {
                foreach (Input i in hoveringInputs)
                {
                    if (ValidateInput(i) && i.CanInteract)
                    {
                        BeginInteraction(i);
                    }
                }
            }
        }

        #endregion
        #region Public Virtual Methods

        /// <summary>
        /// Add an <typeparamref name="Input"/> instance to <paramref name="hoveringInputs"/> list.
        /// </summary>
        /// <param name="_input"><typeparamref name="Input"/> instance to be added.</param>
        public virtual void AddInput (Input _input)
        {
            if (!hoveringInputs.Contains(_input))
            {
                hoveringInputs.Add(_input);
            }
        }

        /// <summary>
        /// Remove a previously added <typeparamref name="Input"/> instance from <paramref name="hoveringInputs"/> list.
        /// </summary>
        /// <param name="_input"><typeparamref name="Input"/> instance to be removed.</param>
        public virtual void RemoveInput (Input _input)
        {
            if (hoveringInputs.Contains(_input))
            {
                hoveringInputs.Remove(_input);
            }
        }



        /// <summary>
        /// Enable the interaction.
        /// </summary>
        /// <param name="_source">Source <typeparamref name="Input"/> that called this method.</param>
        public virtual void BeginInteraction (DUKE.Controls.Input _source) 
        {
            if (IsInteracting || !IsInteractable) { return; }

            source = _source;
            source.LinkInteractable(this);
            
            if (source.IsDesktop) { mkbInput = (DesktopInput)source; }
            else if (source.IsVR) { vrInput = (VRInput)source; }

            sourceInteractionMode = source.currentInteractionMode;         
        }

        /// <summary>
        /// Enable the interaction without linking this instance to <paramref name="_source"/>.
        /// </summary>
        /// <param name="_source">Source <typeparamref name="Input"/> that called this method.</param>
        /// <param name="_link">TRUE if <paramref name="_source"/> should be linked to this instance.</param>
        public virtual void BeginInteraction (DUKE.Controls.Input _source, bool _link) 
        {
            if (IsInteracting || !IsInteractable) { return; }

            source = _source;

            if (_link) { source.LinkInteractable(this); }
            
            if (source.IsDesktop) { mkbInput = (DesktopInput)source; }
            else if (source.IsVR) { vrInput = (VRInput)source; }

            sourceInteractionMode = source.currentInteractionMode;         
        }




        
        /// <summary>
        /// End the interaction immediately.
        /// </summary>
        public virtual void ForceEndInteraction ()
        {
            if (null != source) { EndInteraction(); }
        }

        /// <summary>
        /// Check if this instance has the correct <paramref name="_mode"/>.
        /// </summary>
        /// <param name="_mode">specified <typeparamref name="InteractionMode"/>.</param>
        /// <returns>True if the provided InteractionMode corresponds to this <typeparamref name="Interactable"/>'s <typeparamref name="InteractionMode"/> flags.</returns>
        public virtual bool HasInteractionMode (InteractionMode _mode)
        {
            return (_mode != 0 && interactionMode.HasFlag(_mode));
        }
        
        #endregion
        #region Protected Virtual Methods

        /// <summary>
        /// Returns TRUE when the corret input or input combination is active.
        /// </summary>
        /// <param name="_input"><typeparamref name="Input"/> instance to check.</param>
        /// <returns>TRUE if the correct input combination for this instance is entered.</returns>
        public virtual bool ValidateInput (Input _input)
        {         
            if (_input.IsDesktop) { return ValidateDesktopInput((DesktopInput)_input); }
            else if (_input.IsVR) { return ValidateVRInput((VRInput)_input); } 
            else                  { return false; }     
        }

        #endregion
        #region Protected Virtual Methods

        /// <summary>
        /// Let <paramref name="source"/> <typeparamref name="Input"/> know this interaction should end.
        /// </summary>
        protected virtual void EndInteraction ()
        {
            if (null != source)
            {
                source.UnlinkInteractable(this);
                source = null;
            }
        }

        /// <summary>
        /// Returns TRUE when the corret input or input combination is active.
        /// </summary>
        /// <param name="_input"><typeparamref name="VRInput"/> instance to check.</param>
        /// <returns>TRUE if the correct input combination for this instance is entered.</returns>
        protected virtual bool ValidateVRInput (VRInput _vr) 
        {
            return vrInteractionButton switch 
            {
                VRInteractionButton.Grip => _vr.GripPressed,
                VRInteractionButton.Highlight => _vr.Hit.transform == InteractableTransform,
                VRInteractionButton.Trigger => _vr.TriggerPressed,
                VRInteractionButton.Primary2DClick => _vr.Primary2DAxisPressed,
                VRInteractionButton.Primary2DTouch => _vr.Primary2DAxisTouched,
                _ => false
            };
        }

        /// <summary>
        /// Returns TRUE when the corret input or input combination is active.
        /// </summary>
        /// <param name="_input"><typeparamref name="DesktopInput"/> instance to check.</param>
        /// <returns>TRUE if the correct input combination for this instance is entered.</returns>
        protected virtual bool ValidateDesktopInput (DesktopInput _dt) 
        { 
            if (desktopInteractionModifierFlags.HasFlag(DesktopInteractionModifier.LeftShift) != _dt.LeftShiftPressed)  { return false; }
            if (desktopInteractionModifierFlags.HasFlag(DesktopInteractionModifier.LeftControl) != _dt.LeftCtrlPressed) { return false; }
            if (desktopInteractionModifierFlags.HasFlag(DesktopInteractionModifier.LeftAlt) != _dt.LeftAltPressed)      { return false; }

            return _dt.LMBHeld;
        }

        /// <summary>
        /// Update loop for VR.
        /// </summary>
        protected virtual void VRInteractionUpdate () { }
       
        /// <summary>
        /// Update loop for desktop.
        /// </summary>
        protected virtual void DesktopInteractionUpdate () { }

        /// <summary>
        /// Add listeners to relevant Events.
        /// </summary>
        protected virtual void SubscribeToEvents() { }

        /// <summary>
        /// Remove previously added listeners.
        /// </summary>
        protected virtual void UnsubscribeFromEvents() { }

        #endregion
        #region Utility Methods

        /// <summary>
        /// Get axis multipliers based on AxisFlags.
        /// </summary>
        /// <param name="_flags"><typeparamref name="AxisFlags"/> selection of allowed axes.</param>
        /// <returns>Axis multiplier (1.0f or 0f per axis).</returns>
        protected Vector3 GetAxisMultiplier (AxisFlags _flags)
        {
            return (int)_flags switch 
            {
                1 =>  new Vector3(1, 0, 0),
                2 =>  new Vector3(0, 1, 0),
                3 =>  new Vector3(1, 1, 0),
                4 =>  new Vector3(0, 0, 1),
                5 =>  new Vector3(1, 0, 1),
                6 =>  new Vector3(0, 1, 1),
                0 =>  new Vector3(0, 0, 0),
                -1 => new Vector3(1, 1, 1),
                _ =>  new Vector3(1, 1, 1)
            };
        }
    
        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace