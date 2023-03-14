/******************************************************************************
 * File        : TooltipSettings.cs
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
using TMPro;
using DUKE.Controls;


namespace DUKE.UI {

    /// <summary>
    /// Modes of a <typeparamref name="Tooltip"/>.
    /// </summary>
    public enum TooltipMode {
        UI,
        CurvedUI,
        World
    }

    /// <summary>
    /// Anchor points of a <typeparamref name="Tooltip"/>.
    /// </summary>
    public enum TooltipPosition {
        Custom,
        Center,
        Top,
        Right,
        Bottom,
        Left,
        FollowPointer
    }



    /// <summary>
    /// Control the formatting and settings of the tooltip.
    /// </summary>
    [RequireComponent(typeof(Highlightable))]
    public class TooltipSettings : MonoBehaviour 
    {
        #region Variables

        [SerializeField] [HideInInspector] Tooltip createdTooltip;
        [SerializeField] bool autoDeleteTooltip = true;

        [Header("Panel Settings")]
        [SerializeField] bool autoScaleToContents = true;
        [SerializeField] float maxAutoScaleWidth = 2000;
        [SerializeField] TooltipPosition tooltipPosition = TooltipPosition.Top;
        [SerializeField] float positionalPadding = 50f;

        [Space(10f)]
        [SerializeField] Vector2 tooltipSize;
        [SerializeField] float borderWidth = 10;
        [SerializeField] float textMarginWidth = 10;
        [SerializeField] Vector3 localScale = Vector3.one;
        [SerializeField] Vector3 localEulers = Vector3.zero;
        [SerializeField] Vector3 offsetFromCenter = Vector3.zero;
        [SerializeField] Vector3 customWorldPosition;
        [SerializeField] bool useSourceElementCenter = false;

        [Space(10f)]
        [SerializeField] TooltipMode tooltipMode;

        [Header("Text Settings")]
        [SerializeField] string tooltipText = "[Tooltip text here]";
        [SerializeField] float fontSize = 48;
        [SerializeField] string translatorID;

        [Space(10f)]
        [SerializeField] TextAlignmentOptions alignmentOptions = TextAlignmentOptions.TopLeft;
        [SerializeField] TextOverflowModes overflowModes = TextOverflowModes.Truncate;
        [SerializeField] TextureMappingOptions horizontalTextureMappingOptions = TextureMappingOptions.Character;
        [SerializeField] TextureMappingOptions verticalTextureMappingOptions = TextureMappingOptions.Character;

        #endregion


        #region Properties

        /// <summary>
        /// TRUE enables the <typeparamref name="Tooltip"/>'s panels to scale to the content size automatically.
        /// </summary>
        public bool AutoScaleToContent { 
            get { return autoScaleToContents; } }
        
        /// <summary>
        /// Maximum width of automatic scaling.
        /// </summary>
        public float MaxAutoScaleWidth { 
            get { return maxAutoScaleWidth; } }
        
        
        /// <summary>
        /// TRUE when <paramref name="MaxAutoScaleWidth"/> is higher than 0.
        /// </summary>
        public bool LimitAutoScale { 
            get { return MaxAutoScaleWidth > 0; } }
        
        /// <summary>
        /// Current <typeparamref name="TooltipPosition"/>.
        /// </summary>
        public TooltipPosition TooltipPosition { 
            get { return tooltipPosition; } 
            set { tooltipPosition = value; } }
        
        /// <summary>
        /// Added padding from the anchor point of <paramref name="TooltipPosition"/>.
        /// </summary>
        public float PositionPadding { 
            get { return positionalPadding; } }

        /// <summary>
        /// Size of the <typeparamref name="Tooltip"/>'s background panel.
        /// </summary>
        public Vector2 TooltipSize { 
            get { return tooltipSize; } }
        
        /// <summary>
        /// Width of the border.
        /// </summary>
        public float BorderWidth { 
            get { return borderWidth; } }
        
        /// <summary>
        /// Size of the text's margin.
        /// </summary>
        public float TextMarginWidth { 
            get { return textMarginWidth; } }
        
        /// <summary>
        /// Local scale of the object.
        /// </summary>
        public Vector3 LocalScale { 
            get { return localScale; } }
        
        /// <summary>
        /// Local rotation of the object in EulerAngles.
        /// </summary>
        public Vector3 LocalEulers { 
            get { return localEulers; } }
        
        /// <summary>
        /// Offset from the center position. 
        /// </summary>
        public Vector3 OffsetFromCenter {
             get { return offsetFromCenter; } 
             set { offsetFromCenter = value; } }
        
        /// <summary>
        /// Custom world coordinate position.
        /// </summary>
        public Vector3 CustomWorldPosition { 
            get { return customWorldPosition; } }
        
        /// <summary>
        /// TRUE if <typeparamref name="CurvedUIElement"/>'s <paramref name="sourceRT"/> center should be used.
        /// </summary>
        public bool UseSourceElementCenter { 
            get { return useSourceElementCenter; } }

        /// <summary>
        /// Current <typeparamref name="TooltipMode"/>.
        /// </summary>
        public TooltipMode TooltipMode { 
            get { return tooltipMode; } 
            set { tooltipMode = value; } }
        
        /// <summary>
        /// Text content of the <typeparamref name="Tooltip"/>.
        /// </summary>
        public string TooltipText { 
            get { return tooltipText; } 
            set { tooltipText = value; } }
        
        /// <summary>
        /// Font size of the text component.
        /// </summary>
        public float FontSize { 
            get { return fontSize; } }
        
        /// <summary>
        /// <typeparamref name="TextAlignmentOptions"/> of the text component.
        /// </summary>
        public TextAlignmentOptions AlignmentOptions { 
            get { return alignmentOptions; } }
        
        /// <summary>
        /// <typeparamref name="TextOverflowModes"/> of the text component.
        /// </summary>
        public TextOverflowModes OverflowModes { 
            get { return overflowModes; } }
        
        /// <summary>
        /// Horizontal <typeparamref name="TextureMappingOptions"/> of the text component.
        /// </summary>
        public TextureMappingOptions HorizontalTextureMappingOptions { 
            get { return horizontalTextureMappingOptions; } }
        
        /// <summary>
        /// Vertical <typeparamref name="TextureMappingOptions"/> of the text component.
        /// </summary>
        public TextureMappingOptions VerticalTextureMappingOptions { 
            get { return verticalTextureMappingOptions; } }

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Instantiate a new <typeparamref name="Tooltip"/> object.
        /// </summary>
        public void CreateTooltip ()
        {
            if (null == createdTooltip) {

                GameObject tooltipObj = Instantiate(Resources.Load("Prefabs/UI elements/Tooltip") as GameObject);
                createdTooltip = tooltipObj.GetComponent<Tooltip>();
                createdTooltip.SetTooltip(this);
            }
        }

        /// <summary>
        /// Delete the <typeparamref name="Tooltip"/> object instantiated by this instance.
        /// </summary>
        public void DeleteTooltip ()
        {
            if (autoDeleteTooltip) {

                Destroy(createdTooltip.gameObject);
                createdTooltip = null;
            }
        }
        
        /// <summary>
        /// Set the <typeparamref name="Language"/> of the text.
        /// </summary>
        public void SetLanguage()
        {
            tooltipText = TranslatorSO.GetTranslationById(translatorID);
            
        }

        #endregion
        #region MonoBehaviour Methods

        private void OnEnable()
        {
            tooltipText = TranslatorSO.GetTranslationById(translatorID);
            KLHManager.LanguageSet += SetLanguage;
        }

        private void OnDisable()
        {
            KLHManager.LanguageSet -= SetLanguage;
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace