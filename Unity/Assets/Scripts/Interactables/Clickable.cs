/******************************************************************************
 * File        : Clickable.cs
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
using UnityEngine.UI;
using UnityEngine.Events;
using DUKE.UI;


namespace DUKE.Controls {


    public enum ClickAudio
    {
        Standard,
        Toggle,
        Teleport,
        None
    }


    /// <summary>
    /// Receives clicks and activations.
    /// </summary>
    public class Clickable : Interactable 
    {
        #region Variables

        /// <summary><typeparamref name="UnityEvent"/> which is activated when a click is pressed down over this instace.</summary>
        [Space(20f)]
        [SerializeField] protected UnityEvent onClickStarted;

        /// <summary><typeparamref name="UnityEvent"/> which is activated when a click is released over this instance.</summary>
        [SerializeField] protected UnityEvent onClick;

        [SerializeField] protected ClickAudio clickAudio;

        /// <summary>
        /// <typeparamref name="CurvedUIElement"/> component attached to this object.
        /// </summary>
        [SerializeField, HideInInspector] protected CurvedUIElement cUIElement;

        /// <summary>
        /// TRUE when update events have been subscribed to.
        /// </summary>
        [SerializeField, HideInInspector] protected bool updateEventSubscribedTo = false;

        /// <summary>
        /// TRUE when a click has been started over this instance.
        /// </summary>
        [SerializeField, HideInInspector] protected bool clickStarted = false;

        /// <summary>
        /// Collider of this instance.
        /// </summary>
        [SerializeField, HideInInspector] protected Collider col;

        /// <summary>
        /// Base color of the instance.
        /// </summary>
        [SerializeField, HideInInspector] protected Color baseColor = Color.white;

        /// <summary>
        /// Toggled color of this instance.
        /// </summary>
        [SerializeField, HideInInspector] protected Color toggleColor = Color.green;

        /// <summary>
        /// Currently active <typeparamref name="UIPaletteSO"/>.
        /// </summary>
        [SerializeField, HideInInspector] protected UIPaletteSO uiPalette;

        /// <summary>
        /// Currently active <typeparamref name="UISettingsElement"/>.
        /// </summary>
        [SerializeField, HideInInspector] protected UISettingsElement uiSettings;



        /// <summary>
        /// TRUE registers a click only if it started and ended within the same instance.
        /// </summary>
        [Header("Button Settings")]
        [SerializeField] protected bool requireClickStartedToActivate = true;

        /// <summary>
        /// TRUE allows this instance to be toggleable.
        /// </summary>
        [SerializeField] protected bool isToggleable = false;

        /// <summary>
        /// TRUE when this instance is toggled on.
        /// </summary>
        [SerializeField] protected bool isToggled = false;
        
        /// <summary>
        /// TRUE allows the color to be changed on toggling.
        /// </summary>
        [SerializeField] protected bool changeColorOnToggle = false;
        
        /// <summary>
        /// TRUE marks this instance as a part of a group.
        /// </summary>
        [SerializeField] protected bool isPartOfGroup = false;
        
        /// <summary>
        /// TRUE allows the group be toggled off completely. 
        /// FALSE prevents the last toggled <typeparamref name="Clickable"/> in the group from being toggled off.
        /// </summary>
        [SerializeField] protected bool groupCanBeOff = false;
        
        /// <summary>
        /// TRUE allows multiple <typeparamref name="Clickables"/> in the group to be toggled on simultaneously.
        /// FALSE toggles every other <typeparamref name="Clickable"/> off when one is toggled on.
        /// </summary>
        [SerializeField] protected bool isMultiToggleableInGroup = false;
        
        /// <summary>
        /// TRUE when color have been set.
        /// </summary>
        [SerializeField, HideInInspector] bool colorsSet = false;

        #endregion


        #region Properties

        /// <summary>
        /// <typeparamref name="UISettingsElement"/> component attached to <paramref name="InteractableTransform"/>.
        /// </summary>
        public UISettingsElement UISettings 
        { 
            get { return InteractableTransform.GetComponent<UISettingsElement>(); } 
        }
        
        /// <summary>
        /// <typeparamref name="Image"/> component attached to <paramref name="InteractableTransform"/>.
        /// </summary>
        public Image Image
        { 
            get {return InteractableTransform.GetComponent<Image>(); } 
        }

        /// <summary>
        /// <typeparamref name="CurvedUIElement"/> component attached to <paramref name="InteractableTransform"/>.
        /// </summary>
        public CurvedUIElement Curved
        { 
            get { return InteractableTransform.GetComponent<CurvedUIElement>(); } 
        }
        
        /// <summary>
        /// TRUE when <paramref name="Image"/> is not NULL.
        /// </summary>
        public bool HasImage 
        {
            get { return null != Image; } 
        }

        /// <summary>
        /// TRUE when <paramref name="Curved"/> is not NULL.
        /// </summary>
        public bool IsCurved 
        { 
            get { return null != Curved; } 
        }

        /// <summary>
        /// TRUE allows this instance to be toggleable. 
        /// </summary>
        public bool IsToggleable 
        { 
            get { return isToggleable; } 
        }

        /// <summary>
        /// TRUE when this instance is toggled on.
        /// </summary>
        public bool IsToggled 
        { 
            get { return isToggled; } 
        }
       
        /// <summary>
        /// TRUE allows the color to be changed on toggling.
        /// </summary>
        public bool ChangeColorOnToggle 
        { 
            get { return changeColorOnToggle; } 
        }

        /// <summary>
        /// TRUE marks this instance as a part of a group.
        /// </summary>
        public bool IsPartOfGroup 
        { 
            get { return isPartOfGroup; } 
        }

        /// <summary>
        /// TRUE allows multiple <typeparamref name="Clickables"/> in the group to be toggled on simultaneously. FALSE toggles every other <typeparamref name="Clickable"/> off when one is toggled on.
        /// </summary>
        public bool IsMultiToggleableInGroup 
        { 
            get { return isMultiToggleableInGroup; } 
        }

        /// <summary>
        /// TRUE allows the group to be toggled off completely. FALSE prevents the last toggled <typeparamref name="Clickable"/> in the group from being toggled off. 
        /// </summary>
        public bool GroupCanBeOff 
        { 
            get { return groupCanBeOff; } 
        }
           
        /// <summary>
        /// Current color of the instance.
        /// </summary>
        public Color Color 
        { 
            get { return ChangeColorOnToggle 
                ? IsToggled ? toggleColor : baseColor
                : HasImage ? Image.color : Color.clear; 
            } 
        }

        #endregion


        #region Events

        /// <summary>
        /// Called when this instance is toggled on or off.
        /// </summary>
        public Action<bool> Toggled;

        #endregion


        #region Methods

        #region Public Methods

        public override void AddInput(Input _input)
        {
            if ((!ValidateInput(_input) && requireClickStartedToActivate) || !requireClickStartedToActivate)
            {
                base.AddInput(_input);
            }
        }

        /// <summary>
        /// Begin the interaction and add listeners to <paramref name="_source"/>-specific events.
        /// </summary>
        /// <param name="_source"><typeparamref name="Input"/> which is interacting with this instance.</param>
        public override void BeginInteraction (Input _source)
        {
            base.BeginInteraction(_source);

            ClickStarted();
        }

        /// <summary>
        /// Log message with Debug.Log(<paramref name="_s"/>).
        /// </summary>
        /// <param name="_s"></param>
        public virtual void LogMessage (string _s)
        {
            #if UNITY_EDITOR
            Debug.Log(_s);
            #endif
        }

        /// <summary>
        /// Set the Clickable's 'IsToggled' to true or false if IsToggleable has been defined as true.
        /// </summary>
        /// <param name="_on">Desired state of IsToggled.</param>
        public virtual void SetToggled (bool _on, bool _ignoreRestrictions = false)
        {
            if (_on == true || (_on == false && CanBeToggledOff()) || _on == false && _ignoreRestrictions) 
            {
                isToggled = _on;
                Toggled?.Invoke(_on);
                SetImageColor();
            }
        }

        /// <summary>
        /// Update UISettingsSO from which to read color information.
        /// </summary>
        /// <param name="_newSettings"></param>
        public void SetUISettings (UISettingsElement _newSettings)
        {
            uiSettings = _newSettings;
            uiPalette = _newSettings.UIPalette;
        }

        /// <summary>
        /// Create a BoxCollider for the UI element if it doesn't have one yet.
        /// This method automates a tedious part of the manual UI setup, 
        /// but is not the perfect solution for complex element shapes etc.
        /// DO NOT USE WITH LAYOUT GROUPS! Greyed-out rect fields count as 0.
        /// </summary>
        public virtual void SetupColliderForUIElement ()
        {
            /// Mesh can't be created if the scale is zero.
            if (InteractableTransform.localScale == Vector3.zero) { return; }

            /// Check if this Clickable is part of UI:
            if (!InteractableTransform.TryGetComponent(out col) && InteractableTransform.TryGetComponent(out RectTransform rTrn)) {

                /// Check if this Clickable is part of CurvedUI:
                if (HasImage && InteractableTransform.TryGetComponent(out CurvedUIElement cObj)) 
                {
                    /// If MeshCollider is NOT found:
                    if (!InteractableTransform.TryGetComponent(out MeshCollider mCol)) 
                    {
                        mCol = InteractableTransform.gameObject.AddComponent<MeshCollider>();
                        Mesh m = MeshUtility.CopyMesh(cObj.CurvedMesh); //cObj.CurvedMesh;
                        m.Optimize();

                        mCol.sharedMesh = m;
                        col = mCol;
                    }
                } 
                else 
                {
                    BoxCollider bCol = InteractableTransform.gameObject.AddComponent<BoxCollider>();
                    bCol.size = new Vector3(rTrn.rect.width, rTrn.rect.height, 1f);
                    col = bCol;
                }
            }

            SubscribeToEvents();
        }

        #endregion
        #region MonoBehaviour Methods

        protected override void Start ()
        {
            base.Start();

            if (!InteractableTransform.TryGetComponent(out col) )    { SetupColliderForUIElement(); }

            if (HasImage && !colorsSet) {
                
                colorsSet = true;

                if (null == UISettings) {

                    toggleColor = (Resources.Load("ScriptableObjects/UISettings/DefaultSettings") as UIPaletteSO).AccentColorA;
                    baseColor = Image.color;

                } else {

                    toggleColor = UISettings.UIPalette.AccentColorA;
                    baseColor = UISettings.UIColorMode == UIColorMode.Off 
                        ? Image.color
                        : UISettings.Color;
                }

                SetImageColor();
            } 
        }

        protected override void OnEnable ()
        {
            SubscribeToEvents();                   
        }

        protected override void OnDisable ()
        {
            base.OnDisable();

            UnsubscribeFromEvents();
        }

        #endregion
        #region Override Methods

        /// <summary>
        /// Update loop for VR.
        /// </summary>
        protected override void VRInteractionUpdate ()
        {
            base.VRInteractionUpdate();

            if (sourceInteractionMode == InteractionMode.Raycast) 
            {
                if (null == vrInput.PointerTarget)              { EndInteraction(); } 
                else if (vrInput.PointerTarget != transform)    { EndInteraction(); }
            } 
            else if (sourceInteractionMode == InteractionMode.Overlap) 
            {
                if (null == vrInput.OverlapTarget)              { EndInteraction(); } 
                else if (vrInput.OverlapTarget != transform)    { EndInteraction(); }
            }
        }

        /// <summary>
        /// Update loop for desktop.
        /// </summary>
        protected override void DesktopInteractionUpdate ()
        {
            base.DesktopInteractionUpdate();

            if (null == mkbInput.PointerTarget)           { EndInteraction(); } 
            else if (mkbInput.PointerTarget != transform) { EndInteraction(); }
        }

        /// <summary>
        /// Let source controller know this interaction should end.
        /// </summary>
        protected override void EndInteraction ()
        {
            if (source.IsDesktop)
            {
                if (!mkbInput.LMBHeld) { ClickEnded(); }
            }
            else if (source.IsVR)
            {
                if (!vrInput.TriggerPressed) { ClickEnded(); }
            }

            clickStarted = false;
            base.EndInteraction();
        }

        #endregion
        #region Setup Methods

        /// <summary>
        /// Add listeners to relevant events.
        /// </summary>
        protected override void SubscribeToEvents()
        {
            if (!IsCurved)               { return; }
            if (updateEventSubscribedTo) { return; }

            if (null == cUIElement) 
            { 
                cUIElement = InteractableTransform.GetComponent<CurvedUIElement>(); 
            }

            cUIElement.CurvedMeshUpdated += UpdateCollisionMesh;
            updateEventSubscribedTo = true;
        }

        /// <summary>
        /// Remove previously added listeners.
        /// </summary>
        protected override void UnsubscribeFromEvents()
        {
            if (!IsCurved)                { return; }
            if (!updateEventSubscribedTo) { return; }

            cUIElement.CurvedMeshUpdated -= UpdateCollisionMesh;
            updateEventSubscribedTo = false;
        }

        /// <summary>
        /// A new click started.
        /// </summary>
        protected virtual void ClickStarted () 
        {
            clickStarted = true;
            PlayAudio(clickStarted);
            onClickStarted?.Invoke();
        }

        /// <summary>
        /// End the current click and invoke <paramref name="OnClick"/>.
        /// </summary>
        protected virtual void ClickEnded () 
        {
            if ((clickStarted && requireClickStartedToActivate) || !requireClickStartedToActivate) 
            {
                clickStarted = false;

                if (HasImage && ChangeColorOnToggle) 
                {
                    if (isToggleable)   { SetToggled(!isToggled); } 
                    else                { Image.color = baseColor; }

                    if (isPartOfGroup)  { UpdateGroup(); }
                }
                
                PlayAudio(clickStarted);
                onClick.Invoke();

            }
        }

     
        /// <summary>
        /// Get the up-to-date curved Mesh from CurvedUIElement.
        /// </summary>
        protected virtual void UpdateCollisionMesh (Mesh _mesh)
        {
            bool anyScaleZero = (InteractableTransform.localScale.x <= 0f || InteractableTransform.localScale.y <= 0f || InteractableTransform.localScale.z <= 0f);

            if (!IsCurved || null == col || _mesh.vertexCount < 3 || anyScaleZero)
            {
                col.enabled = false;
            } 
            else
            {
                col.enabled = true;
                MeshCollider mCol = col as MeshCollider;

                if (null != mCol) 
                { 
                    mCol.sharedMesh = _mesh;
                }            
            }
        }

        /// <summary>
        /// Change the color of this Clickable's Image component if it has one.
        /// </summary>
        protected virtual void SetImageColor ()
        {          
            if (HasImage && ChangeColorOnToggle) 
            {
                Image.color = IsToggled ? toggleColor : baseColor;
            }
        }

        /// <summary>
        /// Play audio when a click is started or ended.
        /// </summary>
        /// <param name="_clickStarted">TRUE when the click is started.</param>
        protected virtual void PlayAudio (bool _clickStarted)
        {
            if (_clickStarted)
            {
                switch (clickAudio)
                {
                    default:
                    case ClickAudio.Standard:   AudioController.PlayClickStart(); break;
                    case ClickAudio.Toggle:     break;
                    case ClickAudio.Teleport:   break;             
                }
            }
            else 
            {
                switch (clickAudio)
                {
                    default:
                    case ClickAudio.Standard:   AudioController.PlayClickEnd(); break;
                    case ClickAudio.Toggle:     AudioController.PlayToggle(); break;
                    case ClickAudio.Teleport:   AudioController.PlayTeleport(); break;             
                }
            }    
        }

        #endregion
        #region Toggle Methods

        /// <summary>
        /// Update the toggled state of every Clickable in the same group as this Clickable.
        /// </summary>
        protected virtual void UpdateGroup ()
        {
            foreach (Clickable c in GetGroup()) 
            {
                if (c == this)                   { continue; }
                if (!c.isMultiToggleableInGroup) { c.SetToggled(false, true); }
            }
        }
        
        /// <summary>
        /// Determine whether this Clickable can be turned off based on other Clickables' IsToggled state in the group 
        /// and whether GroupCanBeTurnedOff is true or false.
        /// </summary>
        /// <returns>True if toggling off is allowed.</returns>
        protected virtual bool CanBeToggledOff ()
        {
            if (!groupCanBeOff)
            {
                bool lastActive = true;

                foreach (Clickable c in GetGroup()) 
                {
                    if (c.IsToggled) { lastActive = false; }
                }

                return !lastActive;
            }

            return true;
        }
        
        /// <summary>
        /// Find every other Clickable that has the same parent than this and is also part of a group.
        /// </summary>
        /// <returns>Array of Clickables that share the same parent with this Clickable and have been set as part of a group.</returns>
        protected virtual Clickable[] GetGroup ()
        {
            List<Clickable> clickables = new List<Clickable>();

            foreach (Transform child in InteractableTransform.parent) 
            {
                Clickable c;
                if (child.TryGetComponent(out c)) 
                {
                    if (this != c && c.isPartOfGroup) { clickables.Add(c); }
                }
            }

            return clickables.ToArray();
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace