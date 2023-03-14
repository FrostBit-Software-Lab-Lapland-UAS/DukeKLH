/******************************************************************************
 * File        : VRLaser.cs
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


namespace DUKE.Controls {


    /// <summary>
    /// Controls the visual representation of a VR-controller's laser pointer.
    /// </summary>
    public class VRLaser : MonoBehaviour 
    {
        #region Variables

        [SerializeField] VRInput input;
        [SerializeField] LineRenderer line;
        [SerializeField] Light glowLight;
        [SerializeField] Material mat;
        [SerializeField] Material overlapMat;
        [SerializeField] Color col;
        [SerializeField] Color highlightCol;
        [SerializeField] Color interactCol;
        [SerializeField] float colorIntensity;
        [SerializeField] float idleLaserLength = 1f;
        float laserLength;
        [SerializeField] float laserWidth;
        [SerializeField] float pointSize;
        [SerializeField] float pointSizeMultiplier;
        [SerializeField] Transform point;
        [SerializeField] Transform overlap;

        #endregion


        #region Properties

        /// <summary>
        /// TRUE if laser should be visible.
        /// </summary>
        public bool ShowLaser 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// TRUE is overlap sphere should be visible.
        /// </summary>
        public bool ShowOverlap 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Current color of the laser.
        /// </summary>
        public Color ActiveColor 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// Current intensity of the laser.
        /// </summary>
        public float ActiveIntensity 
        { 
            get { return colorIntensity; } 
        }

        /// <summary>
        /// TRUE when <paramref name="input"/> is linked to an <typeparamref name="Interactable"/>.
        /// </summary>
        protected bool IsInteracting
        {
            get;
            set;
        }

        /// <summary>
        /// Saved length of the laser ray, updated when <paramref name="input"/> begins or ends an interaction.
        /// </summary>
        protected float InteractingRayLength
        {
            get;
            set;
        } = 0f;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        void OnEnable ()
        {
            if (null != input)
            {
                input.OnInteractionLinked += InteractionStarted;
                input.OnInteractionReleased += InteractionEnded;
            }
        }

        void OnDisable ()
        {
            if (null != input)
            {
                input.OnInteractionLinked -= InteractionStarted;
                input.OnInteractionReleased -= InteractionEnded;
            }
        }

        private void Start ()
        {
            if (null == input)  { input = transform.parent.parent.GetComponent<VRInput>(); }
            ShowLaser = true;

            mat = Instantiate(Resources.Load("Materials/EmissiveColor") as Material);      
            mat.SetColor("_Color", col);
            point.GetComponent<MeshRenderer>().material = mat;
            line.material = mat;
            line.positionCount = 2;

            overlapMat = Instantiate(Resources.Load("Materials/VR Touch Point") as Material);
            overlapMat.SetColor("_Color", Color.white);
            overlap.GetComponent<MeshRenderer>().material = overlapMat;
        }

        private void LateUpdate ()
        {
            ShowOverlap = input.currentInteractionMode == InteractionMode.Overlap;
            ShowLaser = !ShowOverlap;

            if (ShowLaser) 
            {
                bool pointActive = true;

                overlap.gameObject.SetActive(false);
                line.enabled = true;

                if (IsInteracting && UseSavedRay())
                {
                    laserLength = InteractingRayLength;      
                    line.startWidth = laserWidth;
                    line.endWidth = laserWidth;
                    line.SetPosition(0, Vector3.zero);
                    line.SetPosition(1, Vector3.up * laserLength);  
                    ActiveColor = interactCol; 
                }
                else
                {
                    if (input.RayIsBlocked)
                    {
                        laserLength = input.RayLength;
                        line.startWidth = laserWidth;
                        line.endWidth = laserWidth;
                        line.SetPosition(0, Vector3.zero);
                        line.SetPosition(1, Vector3.up * laserLength);
                        ActiveColor = input.TriggerPressed ? interactCol : col; 
                    }
                    else if (null == input.PointerTarget) 
                    {
                        laserLength = idleLaserLength;
                        line.startWidth = laserWidth;
                        line.endWidth = 0f;
                        line.SetPosition(0, Vector3.zero);
                        line.SetPosition(1, Vector3.up * laserLength);        
                        ActiveColor = input.TriggerPressed ? interactCol : col; 
                        pointActive = false;
                    } 
                    else 
                    {   
                        laserLength = input.Hit.distance;
                        line.startWidth = laserWidth;
                        line.endWidth = laserWidth;
                        line.SetPosition(0, Vector3.zero);
                        line.SetPosition(1, transform.InverseTransformPoint(input.Hit.point));
                        ActiveColor = input.TriggerPressed ? interactCol : highlightCol;   
                    }
                }

                mat.SetColor("_Color", ActiveColor);
                mat.SetFloat("_Intensity", ActiveIntensity);
                glowLight.color = ActiveColor;
                glowLight.intensity = 3f;

                point.gameObject.SetActive(pointActive);

                if (pointActive)
                {
                    point.localScale = (1 + (laserLength / pointSizeMultiplier)) * pointSize * Vector3.one;
                    point.position = transform.TransformPoint(line.GetPosition(1));
                }
            } 
            else if (null != input.OverlapTarget)
            {
                point.gameObject.SetActive(false);
                overlap.gameObject.SetActive(true);

                line.enabled = false;
                overlap.transform.position = input.TouchPointPosition;
                overlap.transform.localScale = Vector3.one * input.TouchRadius * 2f;

                Color overlapColor = (input.GripPressed || input.TriggerPressed) ? interactCol : highlightCol;
                
                overlapMat.SetColor("_Color", overlapColor);
                glowLight.color = overlapColor;
                glowLight.intensity = 3f;
            }         
        }

        #endregion
        #region Event Methods

        /// <summary>
        /// Called when <paramref name="input"/> begins an interaction.
        /// </summary>
        /// <param name="_interactable"><paramref name="input"/> that was bound to <paramref name="input"/>.</param>
        void InteractionStarted (Interactable _interactable)
        {
            IsInteracting = true;
            InteractingRayLength = input.RayLength;
        }
  
        /// <summary>
        /// Called when <paramref name="input"/> ends an interaction.
        /// </summary>
        /// <param name="_interactable"><paramref name="input"/> that was released by <paramref name="input"/>.</param>
        void InteractionEnded (Interactable _interactable)
        {
            IsInteracting = false;
            InteractingRayLength = 0f;
        }

        #endregion
        # region Assisting Methods

        protected bool UseSavedRay ()
        {
            System.Type type = input.CurrentInteractable.GetType();

            return (type != typeof(DrawArea) && type != typeof(MovablePaperDoll));
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace