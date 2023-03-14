/******************************************************************************
 * File        : Input.cs
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


namespace DUKE.Controls {


    /// <summary>
    /// Base class for VRInput and DesktopInput. 
    /// </summary>
    public class Input : MonoBehaviour 
    {
        #region Variables

        /// <summary>
        /// Currently targeted <typeparamref name="Transform"/>.
        /// </summary>
        [SerializeField] protected Transform targetTransform;

        /// <summary>
        /// Currently linked <typeparamref name="Interactable"/>.
        /// </summary>
        [SerializeField] protected Interactable currentInteractionTarget;

        /// <summary>
        /// Current <typeparamref name="InteractionMode"/>.
        /// </summary>
        public InteractionMode currentInteractionMode;

        /// <summary>
        /// Distance of <paramref name="PointerRay"/>.
        /// </summary>
        [SerializeField] protected float rayDistance;

        /// <summary>
        /// <typeparamref name="LayerMask"/> which registers <paramref name="PointerRay"/>.
        /// </summary>
        [SerializeField] protected LayerMask rayHitLayers;
        
        /// <summary>
        /// <typeparamref name="LayerMask"/> which blocks <paramref name="PointerRay"/>.
        /// </summary>
        [SerializeField] protected LayerMask rayBlockLayers;

        #endregion


        #region Properties

        /// <summary>
        /// <typeparamref name="Transform"/> of the currently targeted object.
        /// </summary>
        public Transform TargetTransform
        { 
            get { return targetTransform; }
            private set { targetTransform = value; } 
        }

        /// <summary>
        /// TRUE if <paramref name="activeInteractables"/> is not empty.
        /// </summary>
        public Interactable CurrentInteractable 
        { 
            get { return currentInteractionTarget; }
            set { currentInteractionTarget = value; } 
        }

        public bool CanInteract
        {
            get { return null == CurrentInteractable; }
        }

        /// <summary>
        /// <typeparamref name="Transform"/> of the object pointed at through <typeparamref name="Raycast"/>.
        /// </summary>
        public virtual Transform PointerTarget 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// <typeparamref name="Ray"/> through which <typeparamref name="Interactables"/> are detected.
        /// </summary>
        public virtual Ray PointerRay 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// <typeparamref name="Hit"/> information of <paramref name="PointerRay"/>.
        /// </summary>
        public virtual RaycastHit Hit 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// TRUE if primary interaction button is being held.
        /// </summary>
        public virtual bool PrimaryInteractionHeld 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// TRUE if this <typeparamref name="Input"/> should send and receive information.
        /// </summary>
        public virtual bool InputActive 
        { 
            get; 
            protected set; 
        } = false;

        /// <summary>
        /// TRUE if this <typeparamref name="Input"/> is of type <typeparamref name="VRInput"/>.
        /// </summary>
        public bool IsVR 
        { 
            get { return GetType() == typeof(VRInput); } 
        }

        /// <summary>
        /// TRUE if this <typeparamref name="Input"/> is of type <typeparamref name="DesktopInput"/>.
        /// </summary>
        public bool IsDesktop 
        { 
            get { return GetType() == typeof(DesktopInput); } 
        }

        #endregion


        #region Events

        /// <summary>
        /// Sent when <paramref name="TargetTransform"/> is changed.
        /// </summary>
        public Action<Transform> OnTargetTransformChanged;

        /// <summary>
        /// Called when this instance becomes linked to an <typeparamref name="Interactable"/>.
        /// </summary>
        public Action<Interactable> OnInteractionLinked;

        /// <summary>
        /// Called when this instance is released from an <typeparamref name="Interactable"/>.
        /// </summary>
        public Action<Interactable> OnInteractionReleased;

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Link <paramref name="_interactable"/> to this instance.
        /// Linking reserves this instance so that other <typeparamref name="Interactables"/> can't access it.
        /// </summary>
        /// <param name="_interactable"><typeparamref name="Interactable"/> instance to be linked to.</param>
        public virtual void LinkInteractable (Interactable _interactable)
        {
            CurrentInteractable = _interactable;   
            OnInteractionLinked?.Invoke(_interactable);
        }

        /// <summary>
        /// Unlink <paramref name="_interactable"/> from this instance.
        /// Unlinking allows this instance to be accessed by other <typeparamref name="Interactables"/>.
        /// </summary>
        /// <param name="_interactable"><typeparamref name="Interactable"/> instance to be released from.</param>
        public virtual void UnlinkInteractable (Interactable _interactable) 
        {
            if (null != _interactable && CurrentInteractable == _interactable)
            {
                CurrentInteractable = null;
                OnInteractionReleased?.Invoke(_interactable);
            }
        }
 
        /// <summary>
        /// Reset the list of interactables and null 'currentInteractionTarget'.
        /// </summary>
        public void ResetInteractables ()
        {
            if (null != CurrentInteractable)
            {
                CurrentInteractable.ForceEndInteraction();
                UnlinkInteractable(CurrentInteractable);  
            }
            
            currentInteractionMode = 0;
        }
        
        /// <summary>
        /// Set this <typeparamref name="Input"/> inactive.
        /// </summary>
        public virtual void DisableInput ()
        {
            ResetInteractables();
            InputActive = false;
        }

        /// <summary>
        /// Set this Input active.
        /// </summary>
        public virtual void EnableInput () 
        {
            InputActive = true;
        }

        #endregion
        #region Protected Methods

        /// <summary>
        /// Set <paramref name="TargetTransform"/> and send pertinent <typeparamref name="Actions"/>.
        /// </summary>
        /// <param name="_newTransform"><typeparamref name="Transform"/> that will become <paramref name="TargetTransform"/>.</param>
        protected virtual void SetTargetTransform (Transform _newTransform)
        {
            if (null != TargetTransform)
            {
                if (TargetTransform != _newTransform)
                {
                    Interactable[] interactables = TargetTransform.GetComponents<Interactable>();

                    foreach (Interactable i in interactables)
                    {
                        i.RemoveInput(this);
                    }
                }
            }

            TargetTransform = _newTransform;

            if (null != TargetTransform)
            {
                Interactable[] interactables = TargetTransform.GetComponents<Interactable>();

                foreach (Interactable i in interactables)
                {
                    if (i.HasInteractionMode(currentInteractionMode))
                    {
                        i.AddInput(this);
                    }
                }
            }
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace