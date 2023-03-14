/******************************************************************************
 * File        : GraphArea.cs
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
using UnityEngine.UI;
using TMPro;
using DUKE.KLHData;
using DUKE.UI;


namespace DUKE.UI {


    /// <summary>
    /// Defines the area in which a data set will be displayed.
    /// Creates a set of DataPoints with which data can be displayed on the curved plane.
    /// </summary>
    [ExecuteAlways]
    public class GraphArea : MonoBehaviour 
    {
        #region Variables

        #region General Variables

        [SerializeField] CurvedUIBase curvedGrid;
        [SerializeField] RectTransform rectTransform;
        [SerializeField] RectTransform graphsContainer;
        [SerializeField] RectTransform backgroundGrid;
        [SerializeField] RectTransform columnNamesContainer;
        [SerializeField] Transform bottomValuesContainer;
        System.Random random;

        #endregion
        #region GraphArea Variables

        [SerializeField] float marginBottom = 100;
        [SerializeField] float marginTop = 100;
        [SerializeField] float marginLeft = 100;
        [SerializeField] float marginRight = 100;
        [SerializeField] int horizontalRange;
        [SerializeField] Vector2 verticalRange;
        [SerializeField] BarDisplayType barDisplayType;

        [SerializeField] Vector3[] graphBasePoints;
        [SerializeField] Vector3[] graphTopPoints;

        #endregion
        #region Grid Variables

        /// <summary>
        /// <typeparamref name="Color"/> of the grid lines.
        /// </summary>
        public Color gridLineColor;

        /// <summary>
        /// <typeparamref name="Color"/> of the grid background;
        /// </summary>
        public Color gridBackgroundColor;
        
        /// <summary>
        /// Name of this instance.
        /// </summary>
        public string gridName = "[Graph Name]";
        
        /// <summary>
        /// Horizontal value unit.
        /// </summary>
        public string gridHorizontalUnit = "[Horizontal Unit]";
        
        /// <summary>
        /// Vertical value unit.
        /// </summary>
        public string gridVerticalUnit = "[Vertical Unit]";
        
        [SerializeField] Image backgroundImage;
        [SerializeField] Material gridMaterialInstance;
        [SerializeField] int gridBorderWidth;
        [SerializeField] int gridLineWidth;
        [SerializeField] int gridHorizontalSegments;
        [SerializeField] int gridVerticalSegments;
        [SerializeField] TextMeshProUGUI gridNameT;
        [SerializeField] TextMeshProUGUI gridHorizontalUnitT;
        [SerializeField] TextMeshProUGUI gridHorizontalMinValueT;
        [SerializeField] TextMeshProUGUI gridHorizontalMaxValueT;
        [SerializeField] TextMeshProUGUI gridVerticalUnitT;
        [SerializeField] TextMeshProUGUI gridVerticalMinValueT;
        [SerializeField] TextMeshProUGUI gridVerticalMaxValueT;
        [SerializeField] List<TextMeshProUGUI> gridHorizontalAdditionalValuesT;
        [SerializeField] List<TextMeshProUGUI> gridVerticalAdditionalValuesT;
        [SerializeField] float gridNameOffsetFromGrid = 200f;
        [SerializeField] float gridUnitOffsetFromGrid = 125f;
        [SerializeField] float gridValueOffsetFromGrid = 50f;
        [SerializeField] TextMeshProUGUI[] renewableTopValueLabels;
        [SerializeField] TextMeshProUGUI[] districtTopValueLabels;
        [SerializeField] TextMeshProUGUI[] bottomValueLabels;

        #endregion
        #region Graph Creation Variables

        /// <summary>
        /// TRUE allows for custom overriding of bar settings.
        /// </summary>
        public bool overrideBarDimensions = false;

        /// <summary>
        /// Custom width of bars.
        /// </summary>
        public float customBarWidth;

        /// <summary>
        /// Custom depth of bars.
        /// </summary>
        public float customBarDepth;

        /// <summary>
        /// TRUE allows for displaying of data names and units.
        /// </summary>
        public bool displayNameAndUnits = true;

        /// <summary>
        /// TRUE allows for displaying of horizontal values.
        /// </summary>
        public bool displayHorizontalValues = true;

        /// <summary>
        /// TRUE allows for displaying of vertical values.
        /// </summary>
        public bool displayVerticalValues = true;

        /// <summary>
        /// TRUE rotates vertical values to be displayed horizontally.
        /// </summary>
        public bool verticalValuesAtHorizontalAlignment = false;

        /// <summary>
        /// Custom depth offset of bars.
        /// </summary>
        [SerializeField] float barDepthOffset;

        /// <summary>
        /// Custom depth offset of lines.
        /// </summary>
        [SerializeField] float lineDepthOffset;

        #endregion
        
        #endregion


        #region Properties 

        /// <summary>
        /// <typeparamref name="CurvedUIBase"/> used for calculating the positions of the elements of <typeparamref name="Graphs"/>. 
        /// </summary>
        public CurvedUIBase CurvedUIBase { 
            get { return curvedGrid; } }
   
        /// <summary>
        /// <typeparamref name=""/> of this instance.
        /// </summary>
        public RectTransform GraphAreaRectTransform { 
            get { return rectTransform; } }
        
        /// <summary>
        /// Container object for <typeparamref name="Graphs"/>.
        /// </summary>
        public RectTransform GraphsContainer { 
            get { return graphsContainer; } }
        
        /// <summary>
        /// <typeparamref name="RectTransform"/> of the background grid.
        /// </summary>
        public RectTransform BackgroundGrid { 
            get { return backgroundGrid; } }
   
        /// <summary>
        /// Vertical range of the background grid.
        /// </summary>
        public Vector2 DataValueRange { 
            get { return verticalRange; } 
            set { verticalRange = new Vector2(Mathf.Min(value.x, verticalRange.y - 1), Mathf.Max(verticalRange.x + 1, value.y)); } }
        
        /// <summary>
        /// Top value of <paramref name="DataValueRange"/>.
        /// </summary>
        public float MaxValue { 
            get { return DataValueRange.y; } }
        
        /// <summary>
        /// Bottom value of <paramref name="DataValueRange"/>.
        /// </summary>
        public float MinValue { 
            get { return DataValueRange.x; } }

        /// <summary>
        /// Border width of the background grid.
        /// </summary>
        public int GridBorderWidth { 
            get { return gridBorderWidth; } 
            set { gridBorderWidth = value; } }
        
        /// <summary>
        /// Width of standard lines of the background grid.
        /// </summary>
        public int GridLineWidth { 
            get { return gridLineWidth; } 
            set { gridLineWidth = value; } }
        
        /// <summary>
        /// Horizontal segments of the background grid. 
        /// Horizontal segment lines travel vertically.
        /// </summary>
        public int HorizontalGridSegments { 
            get { return gridHorizontalSegments; } 
            set { gridHorizontalSegments = value; } }
        
        /// <summary>
        /// Veritcal segments of the background grid.
        /// Vertical segment lines travel horizontally.
        /// </summary>
        public int VerticalGridSegments { 
            get { return gridVerticalSegments; } 
            set { gridVerticalSegments = value; } }
        
        /// <summary>
        /// Number of <typeparamref name="Graphs"/> belonging to this instance.
        /// </summary>
        public int GraphCount { 
            get { return GraphsContainer.childCount; } }
        
        /// <summary>
        /// Number of data points per <typeparamref name="Graph"/>.
        /// </summary>
        public int DataCount { 
            get { return horizontalRange; } 
            set { horizontalRange = value; } }

        /// <summary>
        /// Size of the <typeparamref name="Rect"/> of this <typeparamref name="RectTransform"/>.
        /// </summary>
        public Vector2 RectSize { 
            get { return rectTransform.rect.size; } }
        
        /// <summary>
        /// Width of the area.
        /// </summary>
        public float AreaWidth { 
            get { return rectTransform.rect.width - marginLeft - marginRight; } }
        
        /// <summary>
        /// Height of the area.
        /// </summary>
        public float AreaHeight { 
            get { return rectTransform.rect.height - marginBottom - marginTop; } }
        
        #endregion


        #region Methods

        #region Public Methods
     
        /// <summary>
        /// Update every Graph of this GraphArea.
        /// </summary>
        public void UpdateGraphs () 
        {
            foreach (Graph g in GraphsOfThisArea()) {

                g.UpdateGraph();
            }

            UpdateBackgroundGridVisuals();
        }

        /// <summary>
        /// Called through editor script if overrideBarDimensions is true.
        /// (Does not work in final build!)
        /// </summary>
        public void UpdateBarsCustomValues()
        {
            List<Graph> graphs = GraphsOfThisArea();

            foreach (Graph g in graphs) {
                g.BarWidth = customBarWidth;
                g.BarDepth = customBarDepth;
            }
        }

        /// <summary>
        /// Add a new Graph to this GraphArea.
        /// </summary>
        /// <param name="_newGraph">The Graph to be added.</param>
        public void AddGraph (Graph _newGraph) 
        {
            if (null == random) { random = new System.Random(); }

            _newGraph.RectTransform.SetParent(graphsContainer, false);
            _newGraph.RectTransform.localPosition = new Vector3(0f, 0f, _newGraph.TypeIsBar ? barDepthOffset : lineDepthOffset);
            _newGraph.ID = random.Next(10000, 99999);
            _newGraph.BuildGraph();    

            DataCount = Mathf.Max(DataCount, _newGraph.DataCount);

            UpdateBackgroundGridVisuals();
            RecalculateGraphBasePoints();
        }

        /// <summary>
        /// Get every Graph inside this GraphArea.
        /// </summary>
        /// <returns></returns>
        public List<Graph> GraphsOfThisArea ()
        {
            List<Graph> graphsOfArea = new List<Graph>();

            foreach (RectTransform rt in graphsContainer) {

                if (rt.TryGetComponent(out Graph g)) graphsOfArea.Add(g);
            }

            return graphsOfArea;
        }
        
        /// <summary>
        /// Delete a Graph and it's hierarchy objects.
        /// </summary>
        /// <param name="_g"></param>
        public void DeleteGraph (Graph _g)
        {
            GameObject obj = _g.gameObject;
            DeleteObject(obj);
            RecalculateGraphBasePoints();
        }
        
        /// <summary>
        /// Update the size and material parameters of the Background Grid.
        /// </summary>
        public void UpdateBackgroundGridVisuals ()
        {
            UpdateGridSize();
            UpdateGridMaterialValues();
            UpdateGridTextObjects();
        }
        
        /// <summary>
        /// Calculate pixel position of a GraphPoint.
        /// </summary>
        /// <param name="_x">Horizontal index of the point.</param>
        /// <param name="_y">Vertical total value of the point.</param>
        /// <returns></returns>
        public Vector2 GetPixelPosition (int _x, float _y)
        {
            Vector2 ratio = new Vector2(
                (1f / DataCount * _x) + (0.5f / DataCount),
                (_y - DataValueRange.x) / (DataValueRange.y - DataValueRange.x));

            return GetPixelPosition(ratio);
        }
        
        /// <summary>
        /// Calculate pixel position of a GraphPoint. 
        /// </summary>
        /// <param name="_ratio">Ratio within 0...1 on two axii, describing the position between min and max coordinates.</param>
        /// <returns></returns>
        public Vector2 GetPixelPosition (Vector2 _ratio) 
        {
            float xPos = marginLeft + (AreaWidth * _ratio.x);
            float yPos = marginBottom + (AreaHeight * _ratio.y);

            return new Vector2(xPos, yPos);
        }

        /// <summary>
        /// Return a ratio of the provided value inside DataValueRange.
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        public float GetYRatio (float _value, bool _clamp01 = false)
        {
            float ratio = (_value - DataValueRange.x) / (DataValueRange.y - DataValueRange.x);

            if (_clamp01)   { return Mathf.Clamp(ratio, 0f, 1f); }
            else            { return ratio; }         
        }

        /// <summary>
        /// Calculate the total value of all graphs on a specific index.
        /// For example, asking from index 0 returns the total value of every graph's first point.
        /// </summary>
        /// <returns></returns>
        public float TotalValueAtIndex (int _index)
        {          
            float total = 0f;

            List<Graph> tempList = GraphsOfThisArea();

            for (int i = 0; i < tempList.Count; i++) {
                
                Graph g = tempList[i];
                if (g.Dataset.DataPointCount > _index) {

                    total += g.Dataset.Data[_index].Value;
                }
            }

            return total;
        }
        
        /// <summary>
        /// Get the value of a specific Graph's specific DataPoint.
        /// </summary>
        /// <param name="_graphIndex">Index of the Graph in question.</param>
        /// <param name="_xIndex">Index of the DataPoint containing the requested value.</param>
        /// <returns></returns>
        public float ValueAtIndex (int _graphIndex, int _xIndex)
        {
            if (GraphCount == 0) { return 0f; }

            Graph g = GraphsOfThisArea()[_graphIndex];
            GraphDataset dataset = g.Dataset;
            DataPoint d = dataset.Data[_xIndex];
            float v = d.Value;

            return v;
        }

        /// <summary>
        /// Clear (delete) the Graphs of this GraphArea.
        /// </summary>
        public void ClearGraphs () 
        {
            List<Graph> oldGraphs = GraphsOfThisArea();

            for (int i = oldGraphs.Count - 1; i >= 0; i--) {
                DeleteObject(oldGraphs[i].gameObject);
            }
        }

        /// <summary>
        /// Calculate the world position of a pixel position within the curved GraphArea.
        /// </summary>
        /// <param name="_ratio">Ratio within 0...1 on two axii, describing the position between min and max coordinates.</param>
        /// <param name="_depthOffset">Custom depth of the object if it differs from the standard.</param>
        /// <returns></returns>
        public Vector3 GetWorldPositionOnCurvedGraph (Vector2 _ratio, float _depthOffset = 0f) 
        {
            Vector2 correctedPixelPosition = _ratio - new Vector2(0f, RectSize.y);
            Vector3 curvedPosition = GetWorldPosition(correctedPixelPosition, true, _depthOffset);

            return curvedPosition;
        }

        /// <summary>
        /// Calculate the world position of a pixel position within the curved GraphArea.
        /// </summary>
        /// <param name="_xIndex">Horizontal index of the column.</param>
        /// <param name="_yValue">Vertical value within GraphArea.</param>
        /// <param name="_depthOffset">Custom depth of the object if it differs from the standard.</param>
        /// <returns></returns>
        public Vector3 GetWorldPositionWithIndexAndValue (int _xIndex, float _yValue, float _depthOffset = 0f)
        {
            Vector2 pixelPos = GetPixelPosition(_xIndex, _yValue);

            return GetWorldPositionOnCurvedGraph(pixelPos, _depthOffset);
        }

        /// <summary>
        /// Calculate the world position of a pixel position within the curved GraphArea.
        /// Lightweight calculation compared to GetWorldPositionOnCurvedGraph().
        /// </summary>
        /// <param name="_xIndex">Index of the datapoint.</param>
        /// <param name="_yValue">Value on the specified index.</param>
        /// <param name="_depthOffset">Custom depth of the object if it differs from the standard.</param>
        /// <returns></returns>
        public Vector3 GetWorldPositionOnCurvedGraphLite (int _xIndex, float _yValue, float _depthOffset = 0f)
        {
            if (null == graphBasePoints || null == graphTopPoints) { 
                
                return GetWorldPositionWithIndexAndValue(_xIndex, _yValue, _depthOffset);

            } else if (graphBasePoints.Length <= _xIndex || graphTopPoints.Length <= _xIndex) {

                return GetWorldPositionWithIndexAndValue(_xIndex, _yValue, _depthOffset);
            }

            return Vector3.Lerp(graphBasePoints[_xIndex], graphTopPoints[_xIndex], GetYRatio(_yValue));
        }

        #endregion
        #region MonoBehaviour Methods

        void Start ()
        {
            GetReferences();
        }
        
        void Update ()
        {
            if (null == graphsContainer)    { CreateGraphsContainer(); }
            if (null == backgroundGrid)     { CreateBackgroundGrid(); }   
        }

        void OnEnable ()
        {
            GetReferences();
            RecalculateGraphBasePoints();
            CalculateBottomPanelPositionsAndRotations();
        }

        #endregion
        #region Setup Methods

        /// <summary>
        /// Get the references of required components.
        /// </summary>
        void GetReferences ()
        {
            rectTransform = GetComponent<RectTransform>();
            curvedGrid = CurvedUIBase.FindBaseFromHierarchy(transform);
            bottomValuesContainer = transform.Find("Bottom Value Labels");

            if (null == gridMaterialInstance)   { CreateGridMaterial(); }
            if (null == graphsContainer)        { CreateGraphsContainer(); }
            if (null == backgroundGrid)         { CreateBackgroundGrid(); }
            if (null == random)                 { random = new System.Random(); }

            /// Top Value labels.
            Transform parent = transform.Find("Top Value Labels");
            int count = parent.GetChild(0).childCount;
            
            districtTopValueLabels = new TextMeshProUGUI[count];
            renewableTopValueLabels = new TextMeshProUGUI[count];
            bottomValueLabels = new TextMeshProUGUI[DataCount];

            for( int i = 0; i < count; i++) {

                districtTopValueLabels[i] = parent.GetChild(0).GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                renewableTopValueLabels[i] = parent.GetChild(1).GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();

                #if UNITY_EDITOR
                if (null == districtTopValueLabels[i])  { Debug.LogError("GraphArea.GetReferences(): District ("+i+") textObj is NULL."); }
                if (null == renewableTopValueLabels[i]) { Debug.LogError("GraphArea.GetReferences(): Renewable ("+i+") textObj is NULL."); }       
                #endif
            }

            for (int i = 0; i < DataCount; i++) {

                bottomValueLabels[i] = bottomValuesContainer.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            }
        }
        
        /// <summary>
        /// Create a container object for Graphs.
        /// </summary>
        void CreateGraphsContainer ()
        {
            RectTransform rt = (RectTransform)transform.Find("Graphs Container");

            if (transform.childCount == 0 || null == rt) {

                GameObject obj = new GameObject("Graphs Container");
                graphsContainer = obj.AddComponent<RectTransform>();

                graphsContainer.SetParent(rectTransform, false);
                graphsContainer.localPosition = Vector3.zero;
                graphsContainer.localScale = Vector3.one;

            } else {

                graphsContainer = rt;
            }
        }
        
        /// <summary>
        /// Create an Image background with an adjustable grid.
        /// </summary>
        void CreateBackgroundGrid ()
        {
            /// Create hierarchy object.
            GameObject obj = new GameObject("Background Grid");
            CurvedUIElement cObj = obj.AddComponent<CurvedUIElement>();
            CanvasRenderer cRenderer = obj.AddComponent<CanvasRenderer>();
            backgroundImage = obj.AddComponent<Image>();

            /// Setup basic parameters for the object.
            backgroundGrid = obj.GetComponent<RectTransform>();
            backgroundGrid.SetParent(rectTransform, false);
            backgroundGrid.localPosition = Vector3.zero;
            backgroundGrid.localScale = Vector3.one;

            /// Setup anchors and size of the panel.
            UpdateGridSize();

            backgroundImage.type = Image.Type.Simple;
            backgroundImage.material = gridMaterialInstance;

            columnNamesContainer = new GameObject("Point-specific Labels").AddComponent<RectTransform>();
            columnNamesContainer.SetParent(BackgroundGrid, false);

            /// Create text label objects.
            CreateGridTextObjects();
        }
        
        /// <summary>
        /// Create text label objects inside Background Grid.
        /// </summary>
        void CreateGridTextObjects ()
        {
            float verticalAlignmentAngle = verticalValuesAtHorizontalAlignment ? 0 : 90;

            RectTransform gridNameRT = Instantiate(Resources.Load("Prefabs/UI elements/Empty UI Label") as GameObject).GetComponent<RectTransform>();
            gridNameRT.gameObject.name = "Grid Name";
            gridNameRT.SetParent(BackgroundGrid, false);
            gridNameT = gridNameRT.GetComponent<TextMeshProUGUI>();
            gridNameT.text = (displayNameAndUnits ? gridName : "");
            gridNameT.margin = Vector4.zero;
            gridNameT.alignment = TextAlignmentOptions.Center;

            /// Horizontal info.
            RectTransform gridHorizontalUnitRT = Instantiate(Resources.Load("Prefabs/UI elements/Empty UI Label") as GameObject).GetComponent<RectTransform>();
            gridHorizontalUnitRT.SetParent(BackgroundGrid, false);
            gridHorizontalUnitRT.gameObject.name = "Horizontal Unit";
            gridHorizontalUnitT = gridHorizontalUnitRT.GetComponent<TextMeshProUGUI>();
            gridHorizontalUnitT.text = (displayNameAndUnits ? gridHorizontalUnit : "");
            gridHorizontalUnitT.margin = Vector4.zero;
            gridHorizontalUnitT.alignment = TextAlignmentOptions.Center;

            RectTransform gridHorizontalMinValueRT = Instantiate(Resources.Load("Prefabs/UI elements/Empty UI Label") as GameObject).GetComponent<RectTransform>();
            gridHorizontalMinValueRT.SetParent(BackgroundGrid, false);
            gridHorizontalMinValueRT.gameObject.name = "Horizontal Minimum Value";
            gridHorizontalMinValueT = gridHorizontalMinValueRT.GetComponent<TextMeshProUGUI>();
            gridHorizontalMinValueT.text = (displayHorizontalValues ? "1" : "");
            gridHorizontalMinValueT.margin = Vector4.zero;
            gridHorizontalMinValueT.alignment = TextAlignmentOptions.Center;

            RectTransform gridHorizontalMaxValueRT = Instantiate(Resources.Load("Prefabs/UI elements/Empty UI Label") as GameObject).GetComponent<RectTransform>();
            gridHorizontalMaxValueRT.SetParent(BackgroundGrid, false);
            gridHorizontalMaxValueRT.gameObject.name = "Horizontal Maximum Value";
            gridHorizontalMaxValueT = gridHorizontalMaxValueRT.GetComponent<TextMeshProUGUI>();
            gridHorizontalMaxValueT.text = (displayHorizontalValues ? horizontalRange.ToString() : "");
            gridHorizontalMaxValueT.margin = Vector4.zero;
            gridHorizontalMaxValueT.alignment = TextAlignmentOptions.Center;

            /// Vertical info.
            RectTransform gridVerticalUnitRT = Instantiate(Resources.Load("Prefabs/UI elements/Empty UI Label") as GameObject).GetComponent<RectTransform>();
            gridVerticalUnitRT.SetParent(BackgroundGrid, false);
            gridVerticalUnitRT.gameObject.name = "Vertical Unit";
            gridVerticalUnitT = gridVerticalUnitRT.GetComponent<TextMeshProUGUI>();
            gridVerticalUnitT.text = (displayNameAndUnits ? gridVerticalUnit : "");
            gridVerticalUnitT.margin = Vector4.zero;
            gridVerticalUnitT.alignment = TextAlignmentOptions.Center;
            gridVerticalUnitT.rectTransform.localEulerAngles = new Vector3(0f, 0f, verticalAlignmentAngle);

            RectTransform gridVerticalMinValueRT = Instantiate(Resources.Load("Prefabs/UI elements/Empty UI Label") as GameObject).GetComponent<RectTransform>();
            gridVerticalMinValueRT.SetParent(BackgroundGrid, false);
            gridVerticalMinValueRT.gameObject.name = "Vertical Minimum Value";
            gridVerticalMinValueT = gridVerticalMinValueRT.GetComponent<TextMeshProUGUI>();
            gridVerticalMinValueT.text = (displayVerticalValues ? DataValueRange.x.ToString() : "");
            gridVerticalMinValueT.margin = new Vector4(gridVerticalMinValueT.rectTransform.rect.width / 2f, 0f, 0f, 0f);
            gridVerticalMinValueT.alignment = TextAlignmentOptions.Left;
            gridVerticalMinValueRT.localEulerAngles = new Vector3(0, 0, verticalAlignmentAngle);

            RectTransform gridVerticalMaxValueRT = Instantiate(Resources.Load("Prefabs/UI elements/Empty UI Label") as GameObject).GetComponent<RectTransform>();
            gridVerticalMaxValueRT.SetParent(BackgroundGrid, false);
            gridVerticalMaxValueRT.gameObject.name = "Vertical Maximum Value";
            gridVerticalMaxValueT = gridVerticalMaxValueRT.GetComponent<TextMeshProUGUI>();
            gridVerticalMaxValueT.text = (displayVerticalValues ? DataValueRange.y.ToString() : "");
            gridVerticalMaxValueT.margin = new Vector4(0f, 0f, gridVerticalMaxValueT.rectTransform.rect.width / 2f, 0f);
            gridVerticalMaxValueT.alignment = TextAlignmentOptions.Right;
            gridVerticalMaxValueRT.localEulerAngles = new Vector3(0, 0, verticalAlignmentAngle);

            RectTransform gridBackgroundColumnNames = Instantiate(Resources.Load("Prefabs/UI elements/Bars and Graphs/Graph Background Column Names") as GameObject).GetComponent<RectTransform>();
            gridBackgroundColumnNames.SetParent(BackgroundGrid, false);
        }

        /// <summary>
        /// Create a new material instance for Grid.
        /// </summary>
        void CreateGridMaterial ()
        {
            /// Create a new instance of the base material.
            gridMaterialInstance = new Material(Resources.Load("Materials/PixelGrid") as Material);

            UpdateGridMaterialValues();
        }

        /// <summary>
        /// Calculate the base point of each index in world coordinates.
        /// </summary>
        void RecalculateGraphBasePoints ()
        {
            graphBasePoints = new Vector3[DataCount];
            graphTopPoints = new Vector3[DataCount];

            for (int i = 0; i < DataCount; i++) {

                graphBasePoints[i] = GetWorldPositionWithIndexAndValue(i, MinValue);
                graphTopPoints[i] = GetWorldPositionWithIndexAndValue(i, MaxValue);
            }
        }

        /// <summary>
        /// Update the size of the Grid within GraphArea.
        /// </summary>
        /// <param name="_newSize"></param>
        void UpdateGridSize ()
        {
            if (null == BackgroundGrid) { CreateBackgroundGrid(); }

            BackgroundGrid.anchorMin = Vector2.zero;
            BackgroundGrid.anchorMax = Vector2.one;
            BackgroundGrid.offsetMin = new Vector2(marginLeft, marginBottom);
            BackgroundGrid.offsetMax = new Vector2(-marginRight, -marginTop);
        }
        
        /// <summary>
        /// Update the Grid's material values.
        /// </summary>
        void UpdateGridMaterialValues ()
        {
            if (null == gridMaterialInstance) { CreateGridMaterial(); }

            gridMaterialInstance.SetColor("_Line_Color", gridLineColor);
            gridMaterialInstance.SetColor("_Background_Color", gridBackgroundColor);
            gridMaterialInstance.SetFloat("_Line_Width", GridLineWidth);
            gridMaterialInstance.SetFloat("_Border_Width", GridBorderWidth);
            gridMaterialInstance.SetFloat("_Horizontal_Segments", HorizontalGridSegments);
            gridMaterialInstance.SetFloat("_Vertical_Segments", VerticalGridSegments);
            gridMaterialInstance.SetVector("_Area_Size", new Vector2(BackgroundGrid.rect.width, BackgroundGrid.rect.height));
        }
        
        /// <summary>
        /// Update text and position of text labels.
        /// </summary>
        void UpdateGridTextObjects ()
        {
            /// Update text.
            gridNameT.text = displayNameAndUnits ? gridName : "";
            gridHorizontalUnitT.text = displayNameAndUnits ? gridHorizontalUnit : "";
            gridVerticalUnitT.text = displayNameAndUnits ? gridVerticalUnit : "";
            gridHorizontalMinValueT.text = (displayHorizontalValues ? "1" : "");
            gridHorizontalMaxValueT.text = (displayHorizontalValues ? horizontalRange.ToString() : "");
            gridVerticalMinValueT.text = DataValueRange.x.ToString();
            gridVerticalMaxValueT.text = DataValueRange.y.ToString();

            gridNameT.enabled = gridName != "";
            gridHorizontalUnitT.enabled = gridHorizontalUnit != "";
            gridVerticalUnitT.enabled = gridVerticalUnit != "";
            gridHorizontalMinValueT.enabled = gridHorizontalMaxValueT.enabled = displayHorizontalValues;            
            gridVerticalMinValueT.enabled = gridVerticalMaxValueT.enabled = displayVerticalValues;

            /// Update position.
            gridNameT.rectTransform.localPosition = new Vector3(0f, -AreaHeight / 2f - gridNameOffsetFromGrid, 0f);

            float hrzOffsetY = -AreaHeight / 2f - gridValueOffsetFromGrid - gridHorizontalMaxValueT.rectTransform.rect.height / 2f;
            gridHorizontalUnitT.rectTransform.localPosition = new Vector3(0f, -AreaHeight / 2f - gridUnitOffsetFromGrid, 0f);
            gridHorizontalMinValueT.rectTransform.localPosition = new Vector3(-AreaWidth / 2f, hrzOffsetY, 0f);          
            gridHorizontalMaxValueT.rectTransform.localPosition = new Vector3(AreaWidth / 2f, hrzOffsetY, 0f);

            float vrtOffsetX = -AreaWidth / 2f - gridValueOffsetFromGrid;
            gridVerticalUnitT.rectTransform.localPosition = new Vector3(-AreaWidth / 2f - gridUnitOffsetFromGrid, 0f, 0f);
            gridVerticalMinValueT.rectTransform.localPosition = new Vector3(vrtOffsetX, -AreaHeight / 2f , 0f);
            gridVerticalMaxValueT.rectTransform.localPosition = new Vector3(vrtOffsetX, AreaHeight / 2f, 0f);

            if (!Application.isPlaying) { return; }

            bool floorCountWarning = KLHManager.VisualisationUI.FloorCountWarning;

            /// Update Top Value labels.
            if (null != renewableTopValueLabels && Application.isPlaying && GraphCount > 1) {

                for (int i = 0; i < renewableTopValueLabels.Length; i++) {

                    float val=0;
                    if (GraphCount < 4) {
                        val = ValueAtIndex(1, i);
                    }               
                    else {
                        val = ValueAtIndex(1, i) + ValueAtIndex(3, i) ;
                    }

                    if (null != renewableTopValueLabels[i]) {

                        renewableTopValueLabels[i].transform.parent.gameObject.SetActive(val > 0);

                        if (val > 0) {

                            Vector3 wPos = GetWorldPositionOnCurvedGraphLite(i,TotalValueAtIndex(i)) + Vector3.up * 0.12f;
                            Vector3 localPos = transform.Find("Top Value Labels").InverseTransformPoint(wPos);
                            Color col = (i == 3 && floorCountWarning) ? KLHManager.VisualisationUI.DisabledColor : KLHManager.VisualisationUI.HybridDeviceColors[i];

                            renewableTopValueLabels[i].transform.parent.localPosition = localPos;
                            renewableTopValueLabels[i].text = FormatTopValueString(val);
                            renewableTopValueLabels[i].color = col;
                        }
                    }
                }
            }

            if (null != districtTopValueLabels && Application.isPlaying) {

                for (int i = 0; i < districtTopValueLabels.Length; i++) {
                    
                    float val=0;
                    if (GraphCount <= 2) {
                        val = ValueAtIndex(0, i);
                    }
                    else  {
                        val = ValueAtIndex(0, i) + ValueAtIndex(2, i) ;
                    }

                    if (null != districtTopValueLabels[i]) {

                        districtTopValueLabels[i].transform.parent.gameObject.SetActive(val > 0);

                        if (val > 0) {

                            Vector3 wPos = GetWorldPositionOnCurvedGraphLite(i,TotalValueAtIndex(i)) + Vector3.up * 0.04f;
                            Vector3 localPos = transform.Find("Top Value Labels").InverseTransformPoint(wPos);
                            Color col = (i == 3 && floorCountWarning) ? KLHManager.VisualisationUI.DisabledColor : KLHManager.VisualisationUI.DistrictColors[i];

                            districtTopValueLabels[i].transform.parent.localPosition = localPos;
                            districtTopValueLabels[i].text = FormatTopValueString(val);
                            districtTopValueLabels[i].color = col;

                        } 
                    }
                }
            }

            /// Update Bottom Percentages.
            if (null != bottomValueLabels && Application.isPlaying) {

                for (int i = 0; i < bottomValueLabels.Length; i++) {

                    float percent = Mathf.FloorToInt((TotalValueAtIndex(i) / DataValueRange.y) * 1000) / 10f;
                    string text = percent.ToString() + " %";
                    bottomValueLabels[i].text = text;
                }
            }
        }

        /// <summary>
        /// Delete a GameObject. 
        /// </summary>
        /// <param name="_obj"></param>
        void DeleteObject (GameObject _obj)
        {
            if (Application.isPlaying)  { Destroy(_obj); } 
            else                        { DestroyImmediate(_obj); }
        }

        /// <summary>
        /// Format the text which displays a Graph's value. 
        /// </summary>
        /// <param name="_value">Raw value to be formatted into string.</param>
        /// <returns></returns>
        string FormatTopValueString (float _value) 
        {
            return KLHManager.FormatIntToString((int)_value);
        }

        /// <summary>
        /// Convert a pixel position in GraphArea's coordinates into world position.
        /// </summary>
        /// <param name="_pixelPosition">The current pixel position.</param>
        /// <param name="_isCurved">Whether the GraphArea is curved or not.</param>
        /// <param name="_depthOffset">The depth offset of the position.</param>
        /// <returns></returns>
        Vector3 GetWorldPosition (Vector2 _pixelPosition, bool _isCurved = true, float _depthOffset = 0f) 
        {
            Vector3 adjustedLocalPos = new Vector3(_pixelPosition.x, _pixelPosition.y, _depthOffset);
            Vector3 adjustedWPos = rectTransform.TransformPoint(adjustedLocalPos);

            if (_isCurved) {

                Vector3 centerPos = GraphAreaRectTransform.TransformPoint(GraphAreaRectTransform.rect.center);
                Vector3 curvedPos = CurvedUIBase.ProjectPointToCurvedPlane(adjustedWPos, centerPos);
                return curvedPos;
            }

            return adjustedWPos;
        }

        /// <summary>
        /// Calculate the positions and orientations of each bottom panel within bottomValuesContainer.
        /// </summary>
        void CalculateBottomPanelPositionsAndRotations ()
        {
            if (null != bottomValueLabels) {

                for (int i = 0; i < bottomValueLabels.Length; i++) {

                    if (graphBasePoints.Length < bottomValueLabels.Length) {

                        #if UNITY_EDITOR
                        Debug.LogError("GraphArea.CalculateBottomPanelPositionsAndRotations(): BasePoints are fewer than BottomValues.");
                        #endif
                    
                    } else {

                        Transform panel = bottomValueLabels[i].transform.parent;
                        panel.position = graphBasePoints[i] - Vector3.up * 0.1f;
                        panel.LookAt(panel.position - CurvedUIBase.GetNormalOnCurvedPlane(panel.position));
                    }
                }
            }
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace