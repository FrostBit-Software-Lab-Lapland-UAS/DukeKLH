/******************************************************************************
 * File        : ObjectDimensions.cs
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


namespace DUKE {


    /// <summary>
    /// A single dimension line used for displaying measurements and dimensions.
    /// </summary>
    public class DimensionLine : MonoBehaviour 
    {
        #region Variables

        [SerializeField] Transform parentTransform;
        [SerializeField] Transform lineATransform;
        [SerializeField] Transform lineBTransform;
        [SerializeField] LineRenderer lineA;
        [SerializeField] LineRenderer lineB;
        [SerializeField] TextMeshPro textObj;
        [SerializeField] Material lineMaterial;
        [SerializeField] bool materialsSet = false;


        [Space(20f)]
        [SerializeField, Range(1f, 50f)] float buildingScale = 1f;


        [Space(10f)]
        [SerializeField] Color dimensionColor;
        [SerializeField] float lineWidth = 0.01f;
        [SerializeField] float lineEndPointScale = 0.05f;


        [Space(10f)]
        [SerializeField, Range(0f, 1f)] float textPosition = 0.5f;
        [SerializeField] float textScale;
        [SerializeField] float textDepthOffset = 0.1f;
        [SerializeField, Range(0, 3)] int decimals = 0;
        [SerializeField] bool roundUp = false;
        [SerializeField] bool addOneToResult = false;


        [Space(20f)]
        [SerializeField] bool autoUpdate = false;

        #endregion


        #region Properties

        /// <summary>
        /// Position of the text within 0 and 1 (two end points).
        /// </summary>
        public float TextPosition 
        { 
            get { return textPosition; } 
            set { textPosition = Mathf.Clamp(value, 0f, 1f); } 
        }

        /// <summary>
        /// Current scale for the dimension.
        /// </summary>
        public float BuildingScale 
        {
            get { return buildingScale; } 
            set { buildingScale = value; } 
        }
        
        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Refresh and update the dimension's visuals.
        /// </summary>
        public void UpdateDimension ()
        {
            if (!materialsSet) { SetupLineMaterials(); }

            if (null != lineA && null != lineB && null != textObj) 
            {
                UpdateTextObject(CalculateDistance(), decimals);
                //UpdateLine(lineA);
                //UpdateLine(lineB);


                float distance = Vector3.Magnitude(lineATransform.position - lineBTransform.position);

                Vector3 bottomLineDir = (lineBTransform.position - lineATransform.position).normalized;
                Vector3 edgeLineDir = Vector3.Cross(bottomLineDir, Vector3.up);

                lineA.positionCount = 4;
                lineA.SetPosition(0, Vector3.zero);
                lineA.SetPosition(1, edgeLineDir * lineEndPointScale);
                lineA.SetPosition(2, edgeLineDir * lineEndPointScale + bottomLineDir * distance);
                lineA.SetPosition(3, bottomLineDir * distance);
            } 
        }
        
        /// <summary>
        /// Set the end points of the dimension.
        /// </summary>
        /// <param name="_wPosA"></param>
        /// <param name="_wPosB"></param>
        public void SetPositions (Vector3 _wPosA, Vector3 _wPosB)
        {
            transform.localPosition = _wPosA;
            lineATransform.localPosition = _wPosA;
            lineBTransform.localPosition = _wPosB;

            UpdateDimension();
        }

        #endregion
        #region MonoBehaviour Methods

        private void Update ()
        {
            if (autoUpdate) { UpdateDimension(); }
        }

        #endregion
        #region Calculations and Visual Updating Methods

        /// <summary>
        /// Create a new instance of lineMaterial and assign it to LineRenderers.
        /// </summary>
        void SetupLineMaterials ()
        {
            Material instance = new Material(lineMaterial);

            instance.SetColor("_Color", dimensionColor);

            lineA.material = instance;
            lineB.material = instance;

            lineMaterial = instance;
            materialsSet = true;
        }
        
        /// <summary>
        /// Update the parameters of ''textObj'.
        /// </summary>
        /// <param name="_rawValue"></param>
        /// <param name="_decimals"></param>
        /// <param name="_suffix"></param>
        void UpdateTextObject (float _rawValue, int _decimals = 0, string _suffix = "m")
        {
            float decimalMultiplier = Mathf.Pow(10, _decimals);
            int addition = addOneToResult ? 1 : 0;
            float value = roundUp
                ? Mathf.FloorToInt(_rawValue * decimalMultiplier + addition  + 1) / decimalMultiplier
                : Mathf.RoundToInt(_rawValue * decimalMultiplier + addition) / decimalMultiplier;

            string format = "";

            if (_decimals == 1)         { format = "{0:F1}"; } 
            else if (_decimals == 2)    { format = "{0:F2}"; } 
            else if (_decimals == 3)    { format = "{0:F3}"; } 
            else                        { format = "{0:F0}"; }

            string s = string.Format(format, value) + " " + _suffix;

            textObj.text = s;
            textObj.rectTransform.localScale = Vector3.one * textScale / 100f;
            //textObj.transform.position = lineATransform.position + ((lineBTransform.position - lineATransform.position) * TextPosition) - transform.forward * textDepthOffset;
            textObj.transform.localPosition = lineBTransform.position / 2f;
        }   
        
        /// <summary>
        /// Update the parameters of <paramref name="_line"/>.
        /// </summary>
        /// <param name="_line"></param>
        void UpdateLine (LineRenderer _line)
        {
            /*
            Transform lineTransform = _line.transform;
            _line.startWidth = _line.endWidth = lineWidth;
            _line.startColor = _line.endColor = dimensionColor;
            _line.material.SetColor("_UnlitColor", dimensionColor);

            _line.positionCount = 4;

            _line.SetPosition(0, lineTransform.position + lineTransform.up * lineEndPointScale);
            _line.SetPosition(1, lineTransform.position - lineTransform.up * lineEndPointScale);
            _line.SetPosition(2, lineTransform.position);
            _line.SetPosition(3, GetEdgeOfText(lineTransform));
            */
            
            Transform lineTransform = _line.transform;
            _line.startWidth = _line.endWidth = lineWidth;
            _line.startColor = _line.endColor = dimensionColor;
            _line.material.SetColor("_UnlitColor", dimensionColor);

            _line.positionCount = 4;

            _line.SetPosition(0, Vector3.up * lineEndPointScale);
            _line.SetPosition(1, -Vector3.up * lineEndPointScale);
            _line.SetPosition(2, Vector3.zero);
            _line.SetPosition(3, GetEdgeOfText(lineTransform));

        }
        
        /// <summary>
        /// Calculate the distance of the dimension modified by 'buildingScale'.
        /// </summary>
        /// <returns></returns>
        float CalculateDistance ()
        {
            return Vector3.Distance(lineATransform.position, lineBTransform.position) * buildingScale;
        }
        
        /// <summary>
        /// Calculate the end point for <paramref name="_lineTransform"/>'s LineRenderer component.
        /// </summary>
        /// <param name="_lineTransform"></param>
        /// <returns></returns>
        Vector3 GetEdgeOfText (Transform _lineTransform)
        {
            float trueWidth = textObj.rectTransform.rect.width * textObj.rectTransform.localScale.x;
            float trueHeight = textObj.rectTransform.rect.height * textObj.rectTransform.localScale.y;

            Vector3 textCenter = Vector3.forward * textDepthOffset;
            //Vector3 textCenter = textObj.rectTransform.position + transform.forward * textDepthOffset;
            //Vector3 direction = (_lineTransform.position - textCenter).normalized;

            return textCenter + Vector3.forward * (trueWidth / 2f - 0.01f);
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace