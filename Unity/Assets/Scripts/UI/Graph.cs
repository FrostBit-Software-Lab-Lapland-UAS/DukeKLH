/******************************************************************************
 * File        : Graph.cs
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
using DUKE.Controls;
using DUKE.KLHData;


namespace DUKE.UI {

    /// <summary>
    /// Main visual types of <typeparamref name="Graph"/>.
    /// </summary>
    public enum GraphType 
    {
        Bar,
        Line
    }

    /// <summary>
    /// Display types of <typeparamref name="Graph"/> with <typeparamref name="GraphType"/>.<typeparamref name="Bar"/>.
    /// </summary>
    public enum BarDisplayType 
    {
        Stacked,
        SideBySide
    }


    /// <summary>
    /// Relay information about <typeparamref name="Graph"/> data. 
    /// </summary>
    public struct GraphInfo 
    {
        /// <summary>
        /// Name of the data.
        /// </summary>
        public string name;
        
        /// <summary>
        /// Value of the data.
        /// </summary>
        public float value;
        
        /// <summary>
        /// Ratio of the value when compared against the highest value of the <typeparamref name="GraphDataset"/>.<paramref name="Data"/>.
        /// </summary>
        public float ratio;
        
        /// <summary>
        /// Unit of data. 
        /// </summary>
        public string unit;
        
        /// <summary>
        /// Additional information of the data.
        /// </summary>
        public string extraInfo;



        public GraphInfo (string _name, float _value, float _ratio, string _unit, string _extraInfo="") 
        {
            name = _name;
            value = _value;
            ratio = _ratio;
            unit = _unit;
            extraInfo = _extraInfo;
        }

        public float Percent (int _decimals) 
        {
            return KLHMath.GetPercent(ratio, _decimals);
        }
    }



    /// <summary>
    /// Base class for different Graphs.
    /// </summary>
    [ExecuteAlways]
    [System.Serializable]
    public class Graph : MonoBehaviour 
    {
        #region Variables

        #region General Variables

        [SerializeField] int id;
        [SerializeField] GraphArea graphArea;
        [SerializeField] GraphType graphType;
        [SerializeField, HideInInspector] GraphType prevGraphType;
        [SerializeField] BarDisplayType barDisplayType;
        [SerializeField] GraphDataSelection highlightDisplayedData;
        [SerializeField] GraphDataset dataset;
        [SerializeField, HideInInspector] int sampleSize;

        #endregion
        #region Visual Component and RectTransform Variables

        /// <summary>
        /// TRUE allows for continuous updating of the <typeparamref name="Graph"/>.
        /// </summary>
        public bool liveUpdate = false;
        [SerializeField] Color renderColor = Color.white;
        [SerializeField] Color[] barColors;
        [SerializeField] float pointSize = 0.5f;
        [SerializeField] float lineWidth = 0.1f;
        [SerializeField] float barPadding = 1f;
        [SerializeField] float barWidth = 100f;
        [SerializeField] float barDepth = 0f;
        [SerializeField] [Range(0f, 1f)] float lerpRatio = 1f;
        [SerializeField] [Range(0f, 1f)] float lerpOverlap = 1f;
        [SerializeField] RectTransform rectTransform;
        [SerializeField] RectTransform pointContainer;
        [SerializeField] Material baseRenderMaterial;
        [SerializeField] LineRenderer lineRend;
        [SerializeField] Mesh barMesh;

        #endregion
   
        #endregion


        #region Properties

        /// <summary>
        /// "Parent" <typeparamref name="GraphArea"/> of this instance.
        /// </summary>
        /// <value></value>
        public GraphArea GraphArea { 
            get { return graphArea; } }

        /// <summary>
        /// <typeparamref name="RectTransform"/> of this instance.
        /// </summary>
        public RectTransform RectTransform { 
            get { return rectTransform; } }
        
        /// <summary>
        /// Current <typeparamref name="GraphType"/> of this instance.
        /// </summary>
        public GraphType GraphDisplayType { 
            get { return graphType; } 
            set { graphType = value; } }

        /// <summary>
        /// Current <typeparamref name="BarDisplayType"/> of this instance.
        /// </summary>
        public BarDisplayType BarDisplayType { 
            get { return barDisplayType; } 
            set { barDisplayType = value; } }
        
        /// <summary>
        /// Current <typeparamref name="GraphDataSelection"/> of this instance.
        /// </summary>
        public GraphDataSelection HighlightData { 
            get { return highlightDisplayedData; }
            set { highlightDisplayedData = value; }  }
        
        /// <summary>
        /// Identification number of this instance.
        /// </summary>
        public int ID { 
            get { return id; } 
            set { id = value; } }
        
        /// <summary>
        /// Default render <typeparamref name="Color"/> of this instance.
        /// </summary>
        public Color RenderColor { 
            get { return renderColor; } 
            set { renderColor = value; } }     
        
        /// <summary>
        /// Current <typeparamref name="GraphDataset"/> of this instance.
        /// </summary>
        public GraphDataset Dataset { 
            get { return dataset; } 
            set { dataset = value; SetDataPointObjectData(); } }
        
        /// <summary>
        /// Width of the bar objects.
        /// </summary>
        public float BarWidth { 
            get { return barWidth; } 
            set { barWidth = value; } }
         
        /// <summary>
        /// Depth of the bar objects.
        /// </summary>
        public float BarDepth { 
            get {return barDepth; } 
            set { barDepth = value;} }
        
        /// <summary>
        /// Total number of data points on X-axis.
        /// </summary>
        public int DataCount { 
            get { return Dataset.Data.Count; } }
            
        /// <summary>
        /// Number of generated <typeparamref name="DataPointObjects"/>.
        /// </summary>
        protected int PointCount { 
            get { return pointContainer.childCount; } }
        
        /// <summary>
        /// Float between 0 and 1, which controls the animation of data from  0 to value.
        /// </summary>
        public float LerpRatio { 
            get { return lerpRatio; } 
            set { lerpRatio = Mathf.Clamp(value, 0f, 1f); } }
        
        /// <summary>
        /// Overlap ratio of <paramref name="LerpRatio"/>. 
        /// Value of 0 causes <paramref name="LerpRatio"/> to animate data one by one.
        /// Value of 1 causes <paramref name="LerpRatio"/> to animate data at the same time.
        /// </summary>
        public float LerpOverlap { 
            get { return lerpOverlap; } 
            set { lerpOverlap = Mathf.Clamp(value, 0f, 1f); } }

        /// <summary>
        /// TRUE if <paramref name="graphType"/> is <typeparamref name="GraphType"/>.<typeparamref name="Line"/>.
        /// </summary>
        public bool TypeIsLine { 
            get { return graphType == GraphType.Line; } }
        
        /// <summary>
        /// TRUE if <paramref name="graphType"/> is <typeparamref name="GraphType"/>.<typeparamref name="Bar"/>.
        /// </summary>
        public bool TypeIsBar { 
            get { return graphType == GraphType.Bar; } }
        
        /// <summary>
        /// TRUE if <paramref name="barDisplayType"/> is <typeparamref name="BarDisplayType"/>.<typeparamref name="SideBySide"/>.
        /// </summary>
        public bool BarsSideBySide { 
            get { return barDisplayType == BarDisplayType.SideBySide; } }

        /// <summary>
        /// TRUE if <paramref name="barDisplayType"/> is <typeparamref name="BarDisplayType"/>.<typeparamref name="Stacked"/>.
        /// </summary>
        public bool BarsStacked { 
            get { return barDisplayType == BarDisplayType.Stacked; } }

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Initial method to be called when creating a new Graph. 
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_color"></param>
        /// <param name="_pointSize"></param>
        /// <param name="_lineWidth"></param>
        /// <param name="_barPadding"></param>
        /// <param name="_testData"></param>
        public void InitializeGraph (
            GraphType _type, BarDisplayType _barDisplayType, Color _color, GraphDataset _dataset = null, 
            float _pointSize = 0f, float _lineWidth = 0.02f, float _barPadding = 0f, float _customZscale = 0f)
        {
            graphType = _type;
            barDisplayType = _barDisplayType;
            renderColor = _color;
            pointSize = _pointSize;
            lineWidth = _lineWidth;
            dataset = _dataset;
            barPadding = _barPadding;
            barDepth = _customZscale;
            sampleSize = (dataset == null) ? 3 : dataset.Data.Count;
            liveUpdate = true;
            lerpRatio = lerpOverlap = 1f;
        }
        public void InitializeGraph (string _name, GraphArea _graphArea, GraphDataset _dataset, Color[] _colors = null) 
        {
            
            transform.name = _name;
            graphArea = _graphArea;
            dataset = _dataset;
            barColors = _colors;
        }
       
        /// <summary>
        /// Called when manual update of the graph is required.
        /// </summary>
        public void UpdateGraph ()
        {
            /// Use test data while in development.
            if (null == dataset) return;
            if (null == pointContainer) pointContainer = (RectTransform)transform.GetChild(0);

            if (DataCount != PointCount) {

                CreatePoints(DataCount, true);
            }

            /// Enable the correct visuals.
            if (prevGraphType != graphType) {

                SwitchGraphTypeVisuals();
                prevGraphType = graphType;
            }

            /// Update the position of displayed objects.
            UpdatePoints();

            if (TypeIsLine) {

                UpdateLineRenderer();
            }

            /// Update material colors.
            UpdateVisuals();
            GraphArea.UpdateBackgroundGridVisuals();
            SetDataPointObjectDataSelection(HighlightData);
        }
        
        void SetDataPointObjectData () 
        {
            if (null != Dataset) {
                
                for (int i = 0; i < pointContainer.childCount; i++) {

                    if (i < Dataset.DataPointCount) {

                        pointContainer.GetChild(i).GetComponent<DataPointObject>().DataPoint = Dataset.Data[i];
                    }
                }
            }
        }
        
        public void SetDataPointObjectDataSelection (GraphDataSelection _selectionSet)
        {
            foreach(Transform t in pointContainer) {
                t.GetComponent<DataPointObject>().SelectedData = _selectionSet;
            }
        }
        
        /// <summary>
        /// Create the Graph and the required components.
        /// </summary>
        public void BuildGraph ()
        {
            GetReferences();
            ClearChildren(rectTransform);
            CreateHierarchyObjects();
            SetupLineRenderer();
            UpdateGraph();
        }
        
        /// <summary>
        /// Get the name and value of a point with the defined <paramref name="_index"/>.
        /// </summary>
        /// <param name="_index">Index with which the DataPoint is selected.</param>
        /// <returns></returns>
        public GraphInfo GetGraphInfo (int _index)
        {
            if (_index < 0 || _index >= DataCount) { 
                
                return new GraphInfo("NULL", 0f, 0f, "NULL"); 
                
            } else {
                
                string dName = TranslatorSO.GetTranslationById(dataset.Data[_index].Name);
                float dValue = dataset.Data[_index].Value;
                float dRatio = dValue / graphArea.TotalValueAtIndex(_index);
                string dUnit = graphArea.gridVerticalUnit;
                string dExtraInfo = TranslatorSO.GetTranslationById(dataset.Data[_index].ExtraInfo);

                return new GraphInfo(dName, dValue, dRatio, dUnit, dExtraInfo);
            }
        }

        #endregion
        #region Setup and Hierarchy Methods

        /// <summary>
        /// Get the references of required components.
        /// </summary>
        protected void GetReferences ()
        {
            if (null == rectTransform) { rectTransform = gameObject.GetComponent<RectTransform>(); }
            if (null == graphArea) { graphArea = transform.parent.parent.GetComponent<GraphArea>(); }
            if (null == baseRenderMaterial) {

                baseRenderMaterial = new Material(Resources.Load("Materials/BarMaterial") as Material);
                baseRenderMaterial.SetColor("_EmissiveColor", renderColor);
            }
        }
        
        /// <summary>
        /// Create the hierarchy objects.
        /// </summary>
        protected void CreateHierarchyObjects ()
        {
            CreateContainers();
            CreatePoints(DataCount);
        }
        
        /// <summary>
        /// Create a container object for point objects.
        /// </summary>
        protected void CreateContainers ()
        {
            if (null == pointContainer) {

                GameObject obj = new GameObject("Data Point Container");
                pointContainer = obj.AddComponent<RectTransform>();
                pointContainer.SetParent(rectTransform, false);
                pointContainer.localScale = Vector3.one;
            }
        }
        
        /// <summary>
        /// Create point objects that are used for displaying data points on a graph.
        /// Set every point as disabled by default.
        /// </summary>
        /// <param name="_count"></param>
        /// <param name="_createChildObjects"></param>
        protected void CreatePoints (int _count, bool _createChildObjects = true)
        {
            ClearChildren(pointContainer);

            for (int i = 0; i < _count; i++) {

                CreatePoint(i, _createChildObjects);
            }
        } 
        
        /// <summary>
        /// Create a single point object and parent it to the container.
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        protected RectTransform CreatePoint (int _index, bool _createChildSphere = true)
        {
            /// Create the point object.
            GameObject obj = new GameObject("Data Point " + _index);
            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.SetParent(pointContainer, false);
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            DataPointObject dpo = obj.AddComponent<DataPointObject>();
            dpo.DataPoint = Dataset.Data[_index];
            dpo.Graph = this;

            /// Return the point object unless it has to also contain a visual representation (sphere).
            if (!_createChildSphere) return rt;

            /// Create a child object (Point).
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Sphere";
            sphere.layer = LayerMask.NameToLayer("Interactable");
            Highlightable shl = sphere.AddComponent<Highlightable>();
            shl.Mode |= HighlightMode.Material;
            shl.Mode |= HighlightMode.GraphText;
            RectTransform srt = sphere.AddComponent<RectTransform>();
            srt.SetParent(rt);
            srt.localPosition = Vector3.zero;
            srt.anchoredPosition = Vector2.one * 0.5f;
            srt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            srt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            srt.localScale = Vector3.zero;

            /// Adjust the sphere's MeshRenderer's settings.
            MeshRenderer sRend = srt.GetComponent<MeshRenderer>();
            sRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            sRend.sharedMaterial = baseRenderMaterial;

            /// Create child objects (Bar).
            GameObject bar = Instantiate(Resources.Load("Prefabs/UI elements/Bars and Graphs/Bar_cube") as GameObject);
            bar.name = "Bar";
            bar.layer = LayerMask.NameToLayer("Interactable");
            Highlightable bhl = bar.AddComponent<Highlightable>();
            bhl.Mode |= HighlightMode.Material;
            bhl.Mode |= HighlightMode.GraphText;
            RectTransform brt = bar.GetComponent<RectTransform>();
            brt.SetParent(rt, false);
            brt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            brt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            bar.SetActive(graphType == GraphType.Bar);
            MeshRenderer bRend = bar.GetComponent<MeshRenderer>();
            Material m = barColors == null ? baseRenderMaterial : new Material(baseRenderMaterial);
            m.SetColor("_EmissiveColor", barColors == null ? renderColor : barColors[_index]);
            bRend.material = m;
            bRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            MeshFilter bFilter = bar.GetComponent<MeshFilter>();
            bFilter.mesh = barMesh;

            TooltipSettings bartts = bar.AddComponent<TooltipSettings>();
            bartts.TooltipPosition = TooltipPosition.Custom;
            bartts.OffsetFromCenter = new Vector3(250,0,-20);
            bartts.TooltipMode = TooltipMode.CurvedUI;
            bartts.TooltipText = dpo.DataLogString();

            return rt;
        }
       
        /// <summary>
        /// Setup a Line Renderer component for visual representation.
        /// </summary>
        protected void SetupLineRenderer ()
        {
            if (!pointContainer.TryGetComponent(out lineRend))  { lineRend = pointContainer.gameObject.AddComponent<LineRenderer>(); }

            lineRend.startWidth = lineRend.endWidth = lineWidth;
            lineRend.sharedMaterial = baseRenderMaterial;
            lineRend.enabled = false;
            lineRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        #endregion
        #region Updating and Position Calculation Methods

        /// <summary>
        /// Switch visual representation type between Bar and Line.
        /// Currently the check is done by comparing the names of the objects, which is not optimal.
        /// </summary>
        /// <param name="_targetType"></param>
        protected void SwitchGraphTypeVisuals ()
        {
            lineRend.enabled = TypeIsLine;

            foreach (Transform t in pointContainer) {

                t.GetChild(0).gameObject.SetActive(TypeIsLine);
                t.GetChild(1).gameObject.SetActive(TypeIsBar);
            }      
        }
        
        /// <summary>
        /// Update the position of each point. 
        /// </summary>
        protected void UpdatePoints ()
        {
            for (int i = 0; i < DataCount; i++) {
    
                RectTransform point = pointContainer.GetChild(i).GetComponent<RectTransform>();
                DataPoint dataPoint = dataset.Data[i];

                float interpolatedValue = GetLerpPointHeight(i, 0f, dataPoint.Value);
                Vector2 pixelPosition = graphArea.GetPixelPosition(i, interpolatedValue);

                UpdatePoint(point, pixelPosition, i);
            }
        }
        
        /// <summary>
        /// Update the position of a single point.
        /// </summary>
        /// <param name="_point"></param>
        /// <param name="_pixelPosition"></param>
        protected void UpdatePoint (RectTransform _point, Vector2 _pixelPosition, int _index)
        {
            _point.localPosition = _pixelPosition - graphArea.RectSize / 2f;

            if (TypeIsLine) {

                Transform sphere = _point.GetChild(0);
                ChangeSpherePosition(sphere, _point);
                ChangeSphereSize(sphere, pointSize);

            } else if (TypeIsBar) {

                Transform bar = _point.GetChild(1);
                ChangeBarPosition(bar, _point, _index);
                ChangeBarSize(bar, _point, _index);              
            }
        }
        
        /// <summary>
        /// Calculate a lerp ratio to the currently selected index. 
        /// </summary>
        /// <param name="_base"></param>
        /// <param name="_target"></param>
        /// <returns></returns>
        public float GetLerpPointHeight (int _x, float _base, float _target)
        {
            /// Calculate a rate of lerp for this specific index to create a wave-like smooth animation to the Graph.
            float lerpRatioPerPoint = 1f / DataCount;     // 0.2f
            float lerpValueOfIndex =
                (LerpRatio - (lerpRatioPerPoint * _x * (1 - LerpOverlap)))
                * 1f / ((DataCount - ((1 - LerpOverlap) * (DataCount - 1))) / DataCount);
            float pointHeight = Mathf.Lerp(_base, _target, Mathf.Clamp(lerpValueOfIndex, 0f, 1f));

            return pointHeight;
        }
        
        /// <summary>
        /// Change the position of a single point (sphere).
        /// </summary>
        /// <param name="_sphere"></param>
        /// <param name="_parent"></param>
        protected void ChangeSpherePosition (Transform _sphere, Transform _parent)
        {
            Vector3 flatPos = _parent.position;
            Vector3 centerPos = graphArea.GraphAreaRectTransform.TransformPoint(graphArea.GraphAreaRectTransform.rect.center);
            Vector3 curvedPos = graphArea.CurvedUIBase.ProjectPointToCurvedPlane(flatPos, centerPos);

            _sphere.localPosition = _parent.InverseTransformPoint(curvedPos);
        }
        
        /// <summary>
        /// Change the size of a single point (sphere).
        /// </summary>
        /// <param name="_sphere"></param>
        /// <param name="_size"></param>
        protected void ChangeSphereSize (Transform _sphere, float _size)
        {
            _sphere.localScale = Vector3.one / graphArea.CurvedUIBase.CanvasScale * _size / 100f;
        }
        
        /// <summary>
        /// Change the position of a single bar.
        /// </summary>
        /// <param name="_bar"></param>
        /// <param name="_parent"></param>
        protected void ChangeBarPosition (Transform _bar, Transform _parent, int _xIndex)
        {
            Vector2 pixelPosition = Vector2.zero;
            Vector3 position = Vector3.zero;
            float baseValue = 0;

            if (BarsStacked) {

                for (int y = 0; y < graphArea.GraphCount; y++) {

                    Graph graph = graphArea.GraphsContainer.GetChild(y).GetComponent<Graph>();

                    if (this == graph)          { break; } 
                    else if (graph.TypeIsBar)   { baseValue += graph.GetLerpPointHeight(_xIndex, 0f, graph.Dataset.Data[_xIndex].Value); }
                }

                position = graphArea.GetWorldPositionOnCurvedGraphLite(_xIndex, baseValue);

            } else if (BarsSideBySide) {

                int order = 0;

                for (int i = 0; i < graphArea.GraphCount; i++) {

                    Graph graph = graphArea.GraphsContainer.GetChild(i).GetComponent<Graph>();

                    if (this == graph)          { break; } 
                    else if (graph.TypeIsBar)   { order++; }
                }

                float xDelta = ((-graphArea.GraphCount / 2f)+ 0.5f + order) * (barWidth / graphArea.GraphCount);
                pixelPosition = graphArea.GetPixelPosition(_xIndex, 0) - (graphArea.RectSize / 2f) + new Vector2(xDelta, 0f);
                Vector2 ratio = new Vector2(pixelPosition.x / graphArea.RectSize.x, pixelPosition.y / graphArea.RectSize.y);
                position = graphArea.GetWorldPositionOnCurvedGraph(ratio,_parent.localPosition.z);   
            }
    
            Vector3 lookDirection = graphArea.CurvedUIBase.GetNormalOnCurvedPlane(position);

            _bar.localPosition = _parent.InverseTransformPoint(position);
            _bar.LookAt(position + lookDirection, graphArea.transform.up);
        }

        /// <summary>
        /// Calculate a point in Graph's local coordinates. 
        /// </summary>
        /// <param name="_xIndex">The index of horizontal column to which this point belongs.</param>
        /// <param name="_yValue">The value of the data point.</param>
        /// <param name="_zDepth">Custom depth of the object if it differs from the standard.</param>
        /// <returns></returns>
        public Vector3 GetLocalPointOnGraph (int _xIndex, float _yValue, float _zDepth = 0f)  
        {
            Vector2 pixelPosition = graphArea.GetPixelPosition(_xIndex, _yValue) - graphArea.RectSize / 2f;

            return GetLocalPointOnGraph(pixelPosition, _zDepth);
        }

        /// <summary>
        /// Calculate a point in Graph's local coordinates.
        /// </summary>
        /// <param name="_pixelPosition">Local UI coordinates of the object.</param>
        /// <param name="_zDepth">Custom depth of the object if it differs from the standard.</param>
        /// <returns></returns>
        public Vector3 GetLocalPointOnGraph (Vector2 _pixelPosition, float _zDepth = 0f) 
        {
            Vector3 adjustedLocalPos = new Vector3(_pixelPosition.x, _pixelPosition.y, _zDepth);
            Vector3 adjustedWPos = rectTransform.TransformPoint(adjustedLocalPos);
            Vector3 centerPos = graphArea.GraphAreaRectTransform.TransformPoint(graphArea.GraphAreaRectTransform.rect.center);
            Vector3 curvedPos = graphArea.CurvedUIBase.ProjectPointToCurvedPlane(adjustedWPos, centerPos);

            return curvedPos;
        }

        /// <summary>
        /// Change the size of a single bar.
        /// </summary>
        /// <param name="_bar"></param>
        protected void ChangeBarSize (Transform _bar, Transform _parent, int _index)
        {
            float xScale = 0;
            float yScale = 0;
            float zScale = 0;
            float yRatio = graphArea.GetYRatio(Dataset.Data[_index].Value) * GetLerpPointHeight(_index, 0f, 1f);

            if (BarsStacked) {

                xScale = barWidth;
                zScale = (barDepth > 0) ? barDepth : xScale;     
                yScale = Mathf.Clamp(yRatio * graphArea.AreaHeight, 0f, graphArea.AreaHeight);

            } else if (BarsSideBySide) {

                xScale = barWidth / graphArea.GraphCount;
                zScale = (barDepth > 0) ? barDepth : xScale;
                yScale = Mathf.Clamp(yRatio * graphArea.AreaHeight, 0f, graphArea.AreaHeight);
            }
                
            _bar.localScale = new Vector3(xScale, yScale, zScale);
            _bar.GetComponent<BoxCollider>().enabled = yScale > 0f;
        }
        
        /// <summary>
        /// Update LineRenderer with point positions.
        /// </summary>
        protected void UpdateLineRenderer ()
        {
            Vector3[] positions = new Vector3[PointCount];

            for (int i = 0; i < positions.Length; i++) {

                Vector3 flatPos = pointContainer.GetChild(i).position;
                Vector3 centerPos = graphArea.GraphAreaRectTransform.TransformPoint(graphArea.GraphAreaRectTransform.rect.center);
                Vector3 curvedPos = graphArea.CurvedUIBase.ProjectPointToCurvedPlane(flatPos, centerPos);
                positions[i] = curvedPos;
            }

            lineRend.positionCount = positions.Length;
            lineRend.SetPositions(positions);
            lineRend.startWidth = lineRend.endWidth = lineWidth;
        }
        
        /// <summary>
        /// Update the color of every material on the Graph.
        /// </summary>
        protected void UpdateVisuals ()
        {
            baseRenderMaterial.SetColor("_EmissiveColor", renderColor);
        }

        #endregion
        #region Deletion Methods

        /// <summary>
        /// Clear every child of the provided RectTransform.
        /// </summary>
        /// <param name="_rectTransform"></param>
        protected void ClearChildren (RectTransform _rectTransform)
        {
            if(_rectTransform.childCount > 0) {

                for(int i = _rectTransform.childCount - 1; i >= 0; i--) {

                    DeleteObject( _rectTransform.GetChild(i).gameObject);
                }
            }
        }
        
        /// <summary>
        /// Destroy an object. Use Destroy or DestroyImmediate depending on editor state (Play / Editor).
        /// </summary>
        /// <param name="_obj"></param>
        protected void DeleteObject (GameObject _obj)
        {
            if (Application.isPlaying) Destroy(_obj);
            else DestroyImmediate(_obj);
        }

        #endregion
        
        #endregion

 
    } /// End of Class


} /// End of Namespace