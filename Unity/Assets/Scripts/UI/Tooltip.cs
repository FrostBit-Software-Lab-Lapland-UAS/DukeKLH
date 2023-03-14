/******************************************************************************
 * File        : Tooltip.cs
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
using TMPro;
using DUKE.UI;
using DUKE.Controls;


namespace DUKE.UI {


    /// <summary>
    /// Display text information on a panel.
    /// </summary>
    public class Tooltip : MonoBehaviour
    {
        #region Variables

        [SerializeField] Image backgroundPanel;
        [SerializeField] Image tooltipPanel;
        [SerializeField] TextMeshProUGUI textObj;
        [SerializeField] RectTransform baseRect;
        [SerializeField] TooltipSettings tts;
        Vector2 textAreaSize;

        #endregion


        #region Properties

        /// <summary>
        /// <typeparamref name="RectTransform"/> of the background panel.
        /// </summary>
        protected RectTransform bgRect { 
            get { return backgroundPanel.rectTransform; } }
         
        /// <summary>
        /// <typeparamref name="RectTransform"/> of the tooltip panel.
        /// </summary>
        protected RectTransform ttRect { 
            get { return tooltipPanel.rectTransform; } }
        
        /// <summary>
        /// <typeparamref name="RectTransform"/> of the text object panel.
        /// </summary>
        protected RectTransform txtRect { 
            get { return textObj.rectTransform; } }

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Get the settings of the Tooltip from _settings and begin the activation sequence.
        /// </summary>
        /// <param name="_settings"></param>
        public void SetTooltip (TooltipSettings _settings)
        {
            tts = _settings;
        
            SetAlpha(0f);

            if (tts.TooltipMode != TooltipMode.CurvedUI)    { RemoveCurvedObjects(); } 
            else                                            { UpdateCurvedObjectParameters(); }

            textObj.text = tts.TooltipText;
            textObj.fontSize = tts.FontSize;
            textObj.overflowMode = tts.OverflowModes;
            textObj.alignment = tts.AlignmentOptions;
            textObj.horizontalMapping = tts.HorizontalTextureMappingOptions;
            textObj.verticalMapping = tts.VerticalTextureMappingOptions;

            /// Set Tooltip pivot based on TooltipPosition.
            baseRect.pivot = GetPreferredTooltipPivot();

            SetTooltipSize(textObj.GetPreferredValues());

            if (tts.AutoScaleToContent 
                && tts.LimitAutoScale 
                && textObj.TryGetComponent(out LayoutElement loe)) {

                loe.preferredWidth = tts.MaxAutoScaleWidth;
            }

            StartCoroutine(DisplayTooltip(0f));
        }

        #endregion
        #region MonoBehaviour Methods

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (null != tts) {

                if (tts.TooltipPosition == TooltipPosition.FollowPointer) {
                    
                    baseRect.localPosition = GetLocalPosition();
                }
            }
        }

        #endregion
        #region Tooltip Methods

        /// <summary>
        /// Calculate and set the size of the Tooltip.
        /// </summary>
        /// <param name="_size">New size of the tooltip.</param>
        void SetTooltipSize (Vector2 _size) 
        {
            if (tts.AutoScaleToContent) {
            
                baseRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _size.x + tts.TextMarginWidth + tts.BorderWidth);
                baseRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _size.y + tts.TextMarginWidth + tts.BorderWidth);
                backgroundPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _size.x + tts.TextMarginWidth + tts.BorderWidth);
                backgroundPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _size.y + tts.TextMarginWidth + tts.BorderWidth);
                tooltipPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _size.x + tts.TextMarginWidth);
                tooltipPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _size.y + tts.TextMarginWidth);
                textObj.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _size.x);
                textObj.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _size.y);

            } else {
                
                baseRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tts.TooltipSize.x);
                baseRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tts.TooltipSize.y);
                backgroundPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tts.TooltipSize.x);
                backgroundPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tts.TooltipSize.y);
                tooltipPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tts.TooltipSize.x - tts.BorderWidth);
                tooltipPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tts.TooltipSize.y - tts.BorderWidth);
                textObj.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tts.TooltipSize.x - tts.BorderWidth - tts.TextMarginWidth);
                textObj.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tts.TooltipSize.y - tts.BorderWidth - tts.TextMarginWidth);
            }
        }

        /// <summary>
        /// Get Vector2 coordinates of a RectTransform independent of pivot point.
        /// </summary>
        /// <param name="_rt">RectTransform in question.</param>
        /// <returns>Vector3 position corresponding to the RectTransform's center point.</returns>
        Vector3 GetElementCenterOffsetFromPivot (RectTransform _rt)
        {
            Vector2 dimensionMultiplicator = new Vector2( 0.5f - _rt.pivot.x, 0.5f - _rt.pivot.y);
            Vector2 size = _rt.rect.size;
            Vector3 offset = new Vector3( size.x * dimensionMultiplicator.x, size.y * dimensionMultiplicator.y, 0f);

            return offset;
        }

        /// <summary>
        /// Get the preferred pivot of the Tooltip.
        /// </summary>
        /// <returns></returns>
        Vector2 GetPreferredTooltipPivot ()
        {
            switch (tts.TooltipPosition) {

                default:
                case TooltipPosition.Custom:
                case TooltipPosition.Center:
                    return new Vector2(0.5f, 0.5f);

                case TooltipPosition.Top:
                    return new Vector2(0.5f, 0f);

                case TooltipPosition.Bottom:
                    return new Vector2(0.5f, 1f);

                case TooltipPosition.Left:
                    return new Vector2(1f, 0.5f);

                case TooltipPosition.Right:
                    return new Vector2(0f, 0.5f);
            }
        }

        /// <summary>
        /// Get the local position of the Tooltip based on TooltipPosition enum.
        /// </summary>
        /// <returns></returns>
        Vector3 GetLocalPosition ()
        {
            RectTransform srt = tts.GetComponent<RectTransform>();
            Vector3 offsetFromPivot = GetElementCenterOffsetFromPivot(srt);
            Vector2 dimensionOffset = srt.rect.size / 2f;

            switch (tts.TooltipPosition) {

                default:
                case TooltipPosition.Custom:       
                    return  srt.localPosition + offsetFromPivot + tts.OffsetFromCenter;

                case TooltipPosition.Top: 
                    return srt.localPosition + offsetFromPivot + srt.up * (dimensionOffset.y + tts.PositionPadding) + srt.forward * (tts.OffsetFromCenter.z);

                case TooltipPosition.Bottom: 
                    return srt.localPosition + offsetFromPivot - srt.up * (dimensionOffset.y + tts.PositionPadding) + srt.forward * (tts.OffsetFromCenter.z);

                case TooltipPosition.Left: 
                    return srt.localPosition + offsetFromPivot - srt.right * (dimensionOffset.x + tts.PositionPadding) + srt.forward * (tts.OffsetFromCenter.z);

                case TooltipPosition.Right: 
                    return srt.localPosition + offsetFromPivot + srt.right * (dimensionOffset.x + tts.PositionPadding) + srt.forward * (tts.OffsetFromCenter.z);

                case TooltipPosition.Center:

                    if (tts.UseSourceElementCenter) 
                            { return srt.GetComponent<CurvedUIElement>().SourceRT.localPosition + offsetFromPivot; }
                    else    { return srt.localPosition + offsetFromPivot; }
                    
                case TooltipPosition.FollowPointer:
                    return KLHManager.ActiveInput.Hit.point + offsetFromPivot + tts.OffsetFromCenter;
            }
        }

        /// <summary>
        /// Remove unnecessary CurvedObject components.
        /// </summary>
        void RemoveCurvedObjects ()
        {
            Destroy(GetComponent<CurvedUIElement>());
            Destroy(backgroundPanel.GetComponent<CurvedUIElement>());
            Destroy(tooltipPanel.GetComponent<CurvedUIElement>());
            Destroy(textObj.GetComponent<CurvedUIElement>());        
        }

        /// <summary>
        /// Update the CurvedUIBase of the Tooltip's CurvedUIElements. 
        /// </summary>
        void UpdateCurvedObjectParameters ()
        {     
            if (!tts.TryGetComponent(out CurvedUIElement cUI)) {

                cUI = tts.transform.GetComponentInParent(typeof(CurvedUIElement)) as CurvedUIElement;
            }

            CurvedUIBase g = cUI.CurvedUIBase;

            transform.GetComponent<CurvedUIElement>().CurvedUIBase = g;
            backgroundPanel.GetComponent<CurvedUIElement>().CurvedUIBase = g;
            tooltipPanel.GetComponent<CurvedUIElement>().CurvedUIBase = g;
            textObj.GetComponent<CurvedUIElement>().CurvedUIBase = g;
        }

        /// <summary>
        /// Get the first Canvas object in the hierarchy.
        /// </summary>
        /// <returns></returns>
        Canvas FindCanvasInHierarchy ()
        {

            Canvas[] canvases = GetComponentsInParent<Canvas>();

            if (canvases.Length > 0)    { return canvases[canvases.Length - 1]; }
            else                        { return null; }
        }

        /// <summary>
        /// Set the visibility of Image components.
        /// </summary>
        /// <param name="_alpha"></param>
        void SetAlpha (float _alpha) 
        {
            backgroundPanel.color = new Color(
                backgroundPanel.color.r,
                backgroundPanel.color.g,
                backgroundPanel.color.b,
                _alpha);

            tooltipPanel.color = new Color(
                tooltipPanel.color.r,
                tooltipPanel.color.g,
                tooltipPanel.color.b,
                _alpha);

            textObj.color = new Color(
                textObj.color.r,
                textObj.color.g,
                textObj.color.b,
                _alpha);
        }

        #endregion
        #region Coroutine Methods

        /// <summary>
        /// Display the tooltip. 
        /// </summary>
        /// <returns></returns>
        IEnumerator DisplayTooltip (float _delay) {

            yield return new WaitForEndOfFrame();     

            textObj.GetComponent<ContentSizeFitter>().enabled = false;

            yield return new WaitForEndOfFrame();     
        
            SetTooltipSize(textObj.rectTransform.rect.size);

            yield return new WaitForEndOfFrame();    

            /// Set position through parenting and localPosition:
            baseRect.SetParent(tts.transform.parent);
            baseRect.localScale = tts.LocalScale;
            baseRect.localEulerAngles = tts.LocalEulers;
            baseRect.localPosition = GetLocalPosition();
            
            /// Detach the tooltip and set it as the last element of the UI (rendererd on top):
            baseRect.SetParent(FindCanvasInHierarchy().transform);

            SetAlpha(1f);
        }


        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace