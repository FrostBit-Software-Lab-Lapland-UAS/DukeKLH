/******************************************************************************
 * File        : VRThumbDial.cs
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
using TMPro;


namespace DUKE.Controls {

    /// <summary>
    /// Different modes for thumb dial. Implemented only partially.
    /// </summary>
    public enum ThumbDialMode {
        Numeric09,
        NumericSlider,  /// Not implemented
        Color           /// Not implemented
    }


    /// <summary>
    /// Allows input of data with a VR controller's primary 2D axis. 
    /// Also adds visuals to the controller. 
    /// </summary>
    [RequireComponent(typeof(VRInput))]
    public class VRThumbDial : MonoBehaviour 
    {
        #region Variables

        #region General Variables

        #if UNITY_EDITOR
        [SerializeField] bool drawDebug = true;
        #endif

        [SerializeField] Transform visualBase;
        [SerializeField] Transform outerSelector;
        [SerializeField] Transform innerSelector;
        [SerializeField] Transform iconsParent;
        [SerializeField] VRInput source;
        [SerializeField] ThumbDialMode mode;
        [SerializeField] TextMeshProUGUI[] numeric09Objects;
        int selectedNumber;
        bool isActive = false;

        #endregion
        #region Visual Settings Variables
        
        [Space(10f)]
        [SerializeField] float radius;
        [SerializeField] int numberOfLabels;
        [SerializeField] Color normalColor = Color.white;
        [SerializeField] Color highlightColor = Color.cyan;
        [SerializeField] float outerRingTreshold = 0.8f;
        [SerializeField] int selectedRing;    /// Inner = 0, outer = 1.
        [SerializeField] int selectedIndex;   /// inner 0-3, outer 0-9 clockwise from top.
        
        #endregion
        #region Position and Rotation Variables

        [SerializeField] VRVisuals vrVisualsInstance;
        [SerializeField] Vector3 htcPosition;
        [SerializeField] Quaternion htcRotation;
        [SerializeField] Vector3 oculusPosition;
        [SerializeField] Quaternion oculusRotation;

        #endregion
        
        #endregion


        #region Properties

        /// <summary>
        /// Currently selected number.
        /// </summary>
        protected int SelectedNumber 
        { 
            get { return selectedNumber; } 
            set { selectedNumber = value; } 
        }
         
        /// <summary>
        /// Parent <typeparamref name="Transform"/> of visual elements.
        /// </summary>
        protected Transform VisualBase 
        { 
            get { return visualBase; } 
        }
        
        /// <summary>
        /// TRUE when inner ring is selected.
        /// </summary>
        protected bool InnerRingSelected 
        { 
            get { return selectedRing == 0; } 
        }
        
        /// <summary>
        /// TRUE when outer ring is selected.
        /// </summary>
        protected bool OuterRingSelected 
        { 
            get { return selectedRing == 1; } 
        }

        #endregion


        #region Events

        /// <summary>
        /// Called when a number is selected.
        /// </summary>
        public Action<int> NumberSelected;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        private void OnEnable ()
        {
            source = GetComponent<VRInput>();
            SetNumberPositions(radius);
            vrVisualsInstance = GetComponent<VRVisuals>();
            
            source.Primary2DAxisDown += SendNumber;
            KLHManager.ModeChanged += Deactivate;

            if (vrVisualsInstance.IsOculus)
            {
                visualBase.localPosition = oculusPosition;
                visualBase.localRotation = oculusRotation;         
            }
            else if (vrVisualsInstance.IsHTC)
            {
                visualBase.localPosition = htcPosition;
                visualBase.localRotation = htcRotation;
            }
        }
        
        private void OnDisable ()
        {
            source.Primary2DAxisDown -= SendNumber;
            KLHManager.ModeChanged -= Deactivate;
        }
        
        void Update ()
        {
            if (!isActive) { return; }

            if (source.Primary2DAxisTouched)
            {
                selectedRing = source.Primary2DAxisValue.magnitude > outerRingTreshold ? 1 : 0;
                selectedIndex = selectedRing == 0 
                    ? GetSelectedIndex(Calculate2DAxisAngle(), 4) 
                    : GetSelectedIndex(Calculate2DAxisAngle(), numeric09Objects.Length
                );

                SetSelector(true);  
            } 
            else 
            {
                selectedIndex = selectedRing = -1;
                SetSelector(false);
            }

            UpdateIconColors();
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos ()
        {
            if (!drawDebug) { return; }

            for (int i = 0; i < numberOfLabels; i++) 
            {
                float angle = 360f / numberOfLabels * i;
                Vector3 pos = VisualBase.position + Quaternion.AngleAxis(angle, VisualBase.up) * (VisualBase.forward * radius);

                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(pos, 0.01f);
            }
        }
        #endif

        #endregion
        #region Setup Methods

        /// <summary>
        /// Deactivate this instance.
        /// </summary>
        void Deactivate ()
        {
            SetDialActive(false);
        }

        /// <summary>
        /// Place number icons on the disc.
        /// </summary>
        /// <param name="_radius">Radius of the disc.</param>
        public void SetNumberPositions (float _radius)
        {
            for (int i = 0; i < numeric09Objects.Length; i++) 
            {
                float angle = 360f / numberOfLabels * i;
                Vector3 pos = VisualBase.position + Quaternion.AngleAxis(angle, VisualBase.up) * (VisualBase.forward * radius);
                numeric09Objects[i].transform.position = pos;
            }
        }
        
        /// <summary>
        /// Set the dial active or inactive.
        /// </summary>
        /// <param name="_on">TRUE when the dial should be active.</param>
        public void SetDialActive (bool _on)
        {
            visualBase.gameObject.SetActive(_on);
            isActive = _on;
        }

        #endregion
        #region Selection Methods

        /// <summary>
        /// Send an event of the selected number.
        /// </summary>
        void SendNumber()
        {
            int num = InnerRingSelected ? (selectedIndex + 1) * -1 : selectedIndex;
            SelectedNumber = num;
            outerSelector.localEulerAngles = new Vector3(0f, GetAngleOfIndex(SelectedNumber, numeric09Objects.Length), 0f);

            NumberSelected?.Invoke(SelectedNumber);
        }
        
        /// <summary>
        /// Calculate the angle of touch on the controller's primary 2D axis.
        /// </summary> 
        /// <returns>Current angle of the controller's primary 2D axis.</returns>
        float Calculate2DAxisAngle ()
        {
            float touchAngle = Vector2.SignedAngle(Vector2.up, source.Primary2DAxisValue.normalized) * -1;
            return touchAngle < 0 ? touchAngle += 360f : touchAngle; 
        }
        
        /// <summary>
        /// Calculate the angle of a detected <paramref name="_index"/> when <paramref name="_objectCount"/> number of indices cover 360 degrees.
        /// </summary>
        /// <param name="_index">Selected index of the list.</param>
        /// <param name="_objectCount">Total number of objects in the list.</param>
        /// <returns>Angle of the selected index.</returns>
        float GetAngleOfIndex (int _index, int _objectCount)
        {
            float increment = 360f / _objectCount;
            return _index * increment;
        }
        
        /// <summary>
        /// Calculate the index at the given <paramref name="_angle"/> when <paramref name="_objectCount"/> number of indices cover 360 degrees.
        /// </summary>
        /// <param name="_angle">Given angle with which the index is calculated.</param>
        /// <param name="_objectCount">Total number of objects in the list.</param>
        /// <returns>Index of the selected angle.</returns>
        int GetSelectedIndex (float _angle, int _objectCount)
        {
            float sectorSize = 360f / _objectCount;
            return Mathf.Min(Mathf.FloorToInt(((_angle) - (sectorSize / 2f)) / sectorSize) + 1, 9);
        }
        
        /// <summary>
        /// Update the color property of a number at the given <paramref name="_index"/>.
        /// </summary>
        void UpdateIconColors ()
        {
            for (int i = 0; i < numeric09Objects.Length; i++) 
            {
                if (i != SelectedNumber) 
                {
                    numeric09Objects[i].color = (i == selectedIndex && selectedRing == 1) ? highlightColor : normalColor;
                    numeric09Objects[i].transform.localScale = Vector3.one * 1f;
                } 
                else 
                {
                    numeric09Objects[i].color = highlightColor;
                    numeric09Objects[i].transform.localScale = Vector3.one * 1.25f;
                }
            }

            for (int i = 0; i < iconsParent.childCount; i++) 
            {
                iconsParent.GetChild(i).GetComponent<SpriteRenderer>().color = (i == selectedIndex && selectedRing == 0) ? highlightColor : normalColor;
            }
        }
        
        /// <summary>
        /// Display the correct selector object and rotate it to match the position of the user's thumb.
        /// </summary>
        void SetSelector (bool _on)
        {
            if (_on) 
            {
                innerSelector.gameObject.SetActive(InnerRingSelected);
                outerSelector.gameObject.SetActive(OuterRingSelected);

                if (OuterRingSelected) 
                { 
                    outerSelector.localEulerAngles = new Vector3(0f, GetAngleOfIndex(selectedIndex, numeric09Objects.Length), 0f); 
                }       
                else 
                { 
                    innerSelector.localEulerAngles = new Vector3(0f, GetAngleOfIndex(selectedIndex, 4) + 180f, 0f); 
                }
            } 
            else 
            {
                innerSelector.gameObject.SetActive(false);
                outerSelector.gameObject.SetActive(false);
            }
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace