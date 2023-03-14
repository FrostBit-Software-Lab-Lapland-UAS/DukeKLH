/******************************************************************************
 * File        : Highlightable.cs
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
using UnityEngine.UI;
using UnityEngine.Events;
using DUKE.UI;
using DUKE.KLHData;


namespace DUKE.Controls {


    /// <summary>
    /// Highlight mode flags.
    /// </summary>
    [System.Flags]
    public enum HighlightMode 
    {
        UnityEvent = (1 << 0),
        Material = (1 << 1),
        Scale = (1 << 2),
        GraphText = (1 << 4),
        Tooltip = (1 << 8)
    }  


    /// <summary>
    /// Receive information about being pointed.
    /// </summary>
    public class Highlightable : Interactable 
    {
        #region Variables

        [SerializeField] HighlightMode highlightMode;
        [SerializeField] UnityEvent OnHighlightBegin;
        [SerializeField] UnityEvent OnHighlightEnd;


        [SerializeField] Material originalMaterial;
        [SerializeField] Material highlightMaterial;
        [SerializeField] Vector3 originalScale;
        [SerializeField] Vector3 highlightScale;
        [SerializeField] bool materialsSet = false;

        #endregion


        #region Properties
        
        /// <summary>
        /// Currently selected <typeparamref name="HighlightMode"/> flags.
        /// </summary>
        public HighlightMode Mode 
        { 
            get { return highlightMode; } 
            set { highlightMode = value; } 
        }

        /// <summary>
        /// TRUE when the object has <typeparamref name="Image"/> component attached.
        /// </summary>
        public bool IsUI 
        { 
            get { return GetComponent<Image>() != null; } 
        }

        /// <summary>
        /// TRUE when the object has <typeparamref name="CurvedUIElement"/> component attached.
        /// </summary>
        public bool IsCurvedUI 
        { 
            get { return IsUI && GetComponent<CurvedUIElement>() != null; } 
        }

        /// <summary>
        /// TRUE when <paramref name="Mode"/> has <typeparamref name="UnityEvent"/> flag.
        /// </summary>
        public bool UsingUnityEvents 
        { 
            get { return highlightMode.HasFlag(HighlightMode.UnityEvent); } 
        }

        /// <summary>
        /// TRUE when <paramref name="Mode"/> has <typeparamref name="UnityEvent"/> flag.
        /// </summary>
        public bool UsingMaterial 
        { 
            get { return highlightMode.HasFlag(HighlightMode.Material); } 
        }
        
        /// <summary>
        /// TRUE when <paramref name="Mode"/> has <typeparamref name=""/> flag.
        /// </summary>
        public bool UsingScale 
        { 
            get { return highlightMode.HasFlag(HighlightMode.Scale); } 
        }
        
        /// <summary>
        /// TRUE when <paramref name="Mode"/> has <typeparamref name="GraphText"/> flag.
        /// </summary>
        public bool UsingGraphText 
        { 
            get { return highlightMode.HasFlag(HighlightMode.GraphText); } 
        }
        
        /// <summary>
        /// TRUE when <paramref name="Mode"/> has <typeparamref name="Tooltip"/> flag.
        /// </summary>
        public bool UsingTooltip 
        { 
            get { return highlightMode.HasFlag(HighlightMode.Tooltip); } 
        }

        #endregion


        #region Methods

        #region Override Methods

        public override void AddInput(Input _input)
        {
            base.AddInput(_input);
        }

        /// <summary>
        /// Enable interaction if <paramref name="HasInteractionMode"/> returns TRUE.
        /// </summary>
        /// <param name="_source">Source <typeparamref name="Input"/> that called this method.</param>
        public override void BeginInteraction (Input _source)
        {
            base.BeginInteraction(_source, false);
            SetHighlight(true);
        }
        
        /// <summary>
        /// Let <paramref name="source"/> <typeparamref name="Input"/> know this interaction should end.
        /// </summary>
        protected override void EndInteraction ()
        {
            SetHighlight(false);
            base.EndInteraction();
        }

        /// <summary>
        /// Validate input commands.
        /// </summary>
        /// <param name="_input"><typeparamref name="Input"/> instance that is providing the input commands.</param>
        /// <returns>TRUE if the correct input command is used.</returns>
        public override bool ValidateInput (Input _input)
        {
            return _input.TargetTransform == InteractableTransform;
        }

        /// <summary>
        /// Update loop for VR.
        /// </summary>
        protected override void VRInteractionUpdate ()
        {
            base.VRInteractionUpdate();
            if (source.TargetTransform != transform) { EndInteraction(); }
        }
        
        /// <summary>
        /// Update loop for desktop.
        /// </summary>
        protected override void DesktopInteractionUpdate ()
        {
            base.DesktopInteractionUpdate();
            if (source.TargetTransform != transform) { EndInteraction(); }
        }

        #endregion
        #region Highlight Methods

        /// <summary>
        /// Set highlight on / off.
        /// </summary>
        /// <param name="_on">Whether the higlight should be toggled on or off.</param>
        void SetHighlight (bool _on)
        {
            /// Go through every flag and process the selected modifications.
            if (highlightMode.HasFlag(HighlightMode.UnityEvent)) 
            {
                if (_on && null != OnHighlightBegin)        { OnHighlightBegin?.Invoke(); }
                else if (!_on && null != OnHighlightEnd)    { OnHighlightEnd?.Invoke(); }
            }

            if (highlightMode.HasFlag(HighlightMode.Material)) 
            {
                if (!materialsSet) { SetupMaterials(); }

                if (_on)    { SwapMaterial(highlightMaterial); } 
                else        { SwapMaterial(originalMaterial); }
            }

            if (highlightMode.HasFlag(HighlightMode.Scale)) 
            {
                if (_on)    { ChangeScale(highlightScale); } 
                else        { ChangeScale(originalScale); }
            }

            if (highlightMode.HasFlag(HighlightMode.GraphText)) 
            {
                if (null != InteractableTransform.parent) 
                {
                    if (_on) 
                    {
                        DataPointObject dpo = InteractableTransform.parent.GetComponent<DataPointObject>();
                        DataPoint dp = dpo.DataPoint;
                        GraphArea area = transform.GetComponentInParent<GraphArea>();
                        Graph parentGraph = dpo.transform.GetComponentInParent<Graph>();
                        GraphInfo gInfo = parentGraph.GetGraphInfo(dpo.DataPoint.Index);           
                        Vector2 angles = parentGraph.GraphArea.CurvedUIBase.GetAnglesOfWorldPoint(InteractableTransform.position + (Vector3.up * 0.5f * (InteractableTransform.localScale.y / 1000f)));
                        Vector3 localPos = parentGraph.GraphArea.CurvedUIBase.GetLocalPositionFromAngles(angles) * 1000;
                                             
                        KLHManager.VisualisationUI.ToggleGraphInfoPanel(true, gInfo);
                        KLHManager.VisualisationUI.GraphInfoPanel.localPosition = new Vector3(
                            localPos.x, 
                            localPos.y, 
                            KLHManager.VisualisationUI.GraphInfoPanel.localPosition.z);
                    } 
                    else 
                    {
                        KLHManager.VisualisationUI.ToggleGraphInfoPanel(false);
                    }          
                }         
            }

            if (highlightMode.HasFlag(HighlightMode.Tooltip)) 
            {
                TooltipSettings tooltipSettings = InteractableTransform.GetComponent<TooltipSettings>();

                if (_on)    { tooltipSettings.CreateTooltip(); } 
                else        { tooltipSettings.DeleteTooltip(); }
            }
        }
        
        /// <summary>
        /// Create the necessary materials and references.
        /// </summary>
        void SetupMaterials ()
        {
            if (null == highlightMaterial) 
            {
                highlightMaterial = new Material(Resources.Load("Materials/EmissiveColor") as Material);
                highlightMaterial.color = highlightMaterial.color = Color.white;
            }

            if (null == originalMaterial) 
            {
                if (InteractableTransform.TryGetComponent(out MeshRenderer mRend))  { originalMaterial = mRend.material; } 
                else if (InteractableTransform.TryGetComponent(out Image img))      { originalMaterial = img.material; }
            }

            materialsSet = true;
        }
        
        /// <summary>
        /// change the <typeparamref name="Material"/> of the <typeparamref name="Image"/> or <typeparamref name="MeshRenderer"/> component of this object.
        /// </summary>
        /// <param name="_mat"><typeparamref name="Material"/> to switch to.</param>
        void SwapMaterial (Material _mat)
        {
            if (null == _mat) 
            {
                _mat = highlightMaterial = new Material(Resources.Load("Materials/EmissiveColor") as Material);
                _mat.color = highlightMaterial.color = Color.white;
            }

            if (InteractableTransform.TryGetComponent(out MeshRenderer mRend)) 
            {
                if (Application.isPlaying)  { mRend.sharedMaterial = _mat; }
                else                        { mRend.material = _mat; }        
            } 
            else if (InteractableTransform.TryGetComponent(out Image img)) 
            {
                img.material = _mat;
            }
        }
        
        /// <summary>
        /// Change the object's scale.
        /// </summary>
        /// <param name="_scale">New scale.</param>
        void ChangeScale (Vector3 _scale)
        {
            InteractableTransform.localScale = _scale;
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace