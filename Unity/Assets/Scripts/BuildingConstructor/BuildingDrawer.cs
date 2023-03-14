/******************************************************************************
 * File        : BuildingDrawer.cs
 * Version     : 1.0
 * Author      : Miika Puljujärvi (miika.puljujarvi@lapinamk.fi), Petteri Maljamäki (petteri.maljamaki@lapinamk.fi)
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DUKE.Controls;
using DUKE.KLHData;


namespace DUKE {


    /// <summary>
    /// A recorded state of <typeparamref name="DrawGrid"/>.
    /// </summary>
    [Serializable]
    public struct DrawGridState 
    {
        #region Variables and Properties

        [SerializeField] DrawGrid grid;
        [SerializeField] string name;

        /// <summary>
        /// A <typeparamref name="DrawGrid"/> instance.
        /// </summary>
        public DrawGrid Grid        { get { return grid; } set { grid = value; } }
        
        #endregion



        /// <summary>
        /// Constructor of <typeparamref name="DrawGridState"/>.
        /// </summary>
        /// <param name="_grid">DrawGrid state.</param>
        /// <param name="_name">Name of the state.</param>
        public DrawGridState (DrawGrid _grid, string _name)
        {   
            grid = _grid;
            name = _name;        
        }
    
    
    } /// End of Struct

    

    /// <summary>
    /// A collection of information about a <typeparamref name="DrawGrid"/>'s dimensions.
    /// </summary>
    public struct BuildingBounds 
    {
        #region Variables

        /// <summary>
        /// Total dimension of <typeparamref name="DrawGrid"/>'s <typeparamref name="DrawCells"/> with value of 1 or more.
        /// </summary>
        public Vector3 boundDimensions;

        /// <summary>
        /// Center point of <typeparamref name="boundDimensions"/>.
        /// </summary>
        public Vector3 originPoint;

        /// <summary>
        /// Hypotenuse of <typeparamref name="boundDimensions"/>.
        /// </summary>
        public float maxCornerDistance;

        #endregion



        /// <summary>
        /// Constructor of <typeparamref name="BuildingBounds"/>.
        /// </summary>
        /// <param name="_dimensions"></param>
        /// <param name="_origin"></param>
        /// <param name="_isEmpty"></param>
        public BuildingBounds (Vector3 _dimensions, Vector3 _origin, bool _isEmpty = false)
        {
            boundDimensions = _dimensions;
            originPoint = _origin;
            maxCornerDistance = Mathf.Sqrt((Mathf.Pow(boundDimensions.x, 2) + Mathf.Pow(boundDimensions.z, 2)));
        }


    } /// End of Struct



    /// <summary>
    /// Design a building by drawing a floor plan and scaling it with the desired number of stories.
    /// </summary>
    public class BuildingDrawer : MonoBehaviour {

        #region Variables

        #region General Variables

        /// <summary>
        /// Static instance of <typeparamref name="BuildingDrawer"/>.
        /// </summary>
        public static BuildingDrawer Current;

        [SerializeField] DrawArea drawInteractable;

        /// <summary>
        /// Control panel containing floor and preset <typeparamref name="Clickables"/>.
        /// </summary>
        [SerializeField] GameObject controlsObject;

        /// <summary>
        /// Plane containing the main collider of the <typeparamref name="BuildingDrawer"/>.
        /// </summary>
        [SerializeField] Transform drawPlane;

        /// <summary>
        /// Background plane.
        /// </summary>
        [SerializeField] Transform backgroundPlane;

        /// <summary>
        /// Container object for dimensions and visual indicator objects.
        /// </summary>
        [SerializeField] Transform drawIndicator;

        /// <summary>
        /// Container for level <typeparamref name="Mesh"/> objects.
        /// </summary>
        [SerializeField] Transform levelMeshContainer;

        /// <summary>
        /// Container for outline <typeparamref name="Mesh"/> objects.
        /// </summary>
        [SerializeField] Transform outlineMeshContainer;

        /// <summary>
        /// Container for bottom <typeparamref name="Mesh"/> objects.
        /// </summary>
        [SerializeField] Transform bottomMeshContainer;

        #endregion
        #region Material Variables

        /// <summary>
        /// <typeparamref name="Material"/> for level <typeparamref name="Mesh"/>.
        /// </summary>
        [Space(10f)]
        [SerializeField] Material levelMeshMaterial;

        /// <summary>
        /// <typeparamref name="Material"/> for outline <typeparamref name="Mesh"/>.
        /// </summary>
        [SerializeField] Material outlineMeshMaterial;

        /// <summary>
        /// <typeparamref name="Material"/> for bottom <typeparamref name="Mesh"/>.
        /// </summary>
        [SerializeField] Material bottomMeshMaterial;

        /// <summary>
        /// <typeparamref name="Material"/> for draw plane.
        /// </summary>
        [SerializeField] [HideInInspector] Material drawPlaneMaterial;

        /// <summary>
        /// <typeparamref name="Material"/> for background plane.
        /// </summary>
        [SerializeField] [HideInInspector] Material backgroundMaterial;

        #endregion
        #region Grid Variables

        /// <summary>
        /// Size of the draw plane (X, Y).
        /// </summary>
        [Header("Grid Settings")]
        public Vector2 drawAreaSize = new Vector2(3f, 2f);

        /// <summary>
        /// Scale divider of the grid and meshes.
        /// </summary>
        [SerializeField] float buildingScale = 20f;

        /// <summary>
        /// Width of draw area's standard lines (every unit).
        /// </summary>
        [Space(5f)]
        public float lineWidth;

        /// <summary>
        /// Width of draw area's large lines (frequence determined by <typeparamref name="thickLineInterval"/>).
        /// </summary>
        public float thickLineWidth;

        /// <summary>
        /// width of the indicator line objects.
        /// </summary>
        public float indicatorLineWidth;

        /// <summary>
        /// Frequency of thick lines within draw area.
        /// </summary>
        public int thickLineInterval = 10;

        /// <summary>
        /// TRUE will limit the grid to the size allowed by <paramref name="drawAreaSize"/> and <paramref name="buildingScale"/>.
        /// FALSE will allow for grid to reach over the draw plane.
        /// </summary>
        public bool trimOverflowCells = true;

        /// <summary>
        /// Color of the background plane.
        /// </summary>
        [Space(20f)]
        public Color baseColor = new Color(0.45f, 0.45f, 0.45f, 1f);

        /// <summary>
        /// Color of the draw plane's lines.
        /// </summary>
        public Color lineColor = Color.white;

        /// <summary>
        /// Color array of different building heights (floor 1-9).
        /// </summary>
        public Color[] drawColors = new Color[9];

        /// <summary>
        /// Currently selected floor.
        /// </summary>
        [SerializeField] [HideInInspector] int currentFloor = 1;

        #endregion
        #region Draw and Grid Variables

        /// <summary>
        /// Currently active <typeparamref name="DrawAction"/>.
        /// </summary>
        DrawAction currentDrawAction;

        /// <summary>
        /// <typeparamref name="DrawGridState"/> list that can be cycled through with Undo and Redo buttons.
        /// </summary>
        [SerializeField] List<DrawGridState> undoGridStates;

        /// <summary>
        /// Index of the active <typeparamref name="DrawGridState"/> within <typeparamref name="undoGridStates"/>.
        /// </summary>
        [SerializeField] int currentGridStateIndex;

        /// <summary>
        /// Maximum size of <typeparamref name="undoGridStates"/>.
        /// </summary>
        [SerializeField] int maxGridStates = 256;

        /// <summary>
        /// <typeparamref name="Mesh"/> array containing each floor's level <typeparamref name="Mesh"/>.
        /// </summary>
        [SerializeField] Mesh[] levelMeshes;

        /// <summary>
        /// <typeparamref name="Mesh"/> array containing each floor's outline <typeparamref name="Mesh"/>.
        /// </summary>
        [SerializeField] Mesh[] outlineMeshes;

        /// <summary>
        /// <typeparamref name="Mesh"/> array containing each floor's bottom <typeparamref name="Mesh"/>.
        /// </summary>
        [SerializeField] Mesh[] bottomMeshes;

        /// <summary>
        /// <typeparamref name="IndicatorLineObject"> that is used to create draw "boundaries" in realtime.
        /// </summary>
        [SerializeField] IndicatorLineObject drawBoundsObj;

        /// <summary>
        /// Displays dimensions of the draw selection.
        /// </summary>
        [SerializeField] ObjectDimensions objDimensions;
        [SerializeField] Dimension[] dimensions;

        #endregion
        #region Mesh Variables

        /// <summary>
        /// Number of vertices in outlineMesh's cross-section. 
        /// </summary>
        [SerializeField] int runResolution = 4;

        /// <summary>
        /// Radius of outlineMesh's cross-section.
        /// </summary>
        [SerializeField] float runThickness = 0.005f;

        /// <summary>
        /// Inset of each grid tile in bottomMesh. Inset of 0.0f creates a visually uniform mesh. 
        /// Inset of 0.001f shrinks each grid tile quad's edges 1/1000th of a unit from edge towards the center of the tile.
        /// </summary>
        [SerializeField] float bottomMeshTileInset = 0.002f;

        /// <summary>
        /// <typeparamref name="Vector3"/> list used for generating <typeparamref name="Meshes"/>.
        /// </summary>
        [SerializeField] List<Vector3> vertices = new List<Vector3>();

        /// <summary>
        /// <typeparamref name="int"/> list used for generating <typeparamref name="Meshes"/>.
        /// </summary>
        [SerializeField] List<int> triangles= new List<int>();

        /// <summary>
        /// <typeparamref name="Vector2"/> list used for generating <typeparamref name="Meshes"/>.
        /// </summary>
        [SerializeField] List<Vector2> uvs= new List<Vector2>();

        #endregion
        #region Debug Variables

        #if UNITY_EDITOR
        [SerializeField] bool drawDebug = true;
        #endif

        #endregion

        #endregion


        #region Properties

        #region General Properties

        /// <summary>
        /// Reference to <typeparamref name="KLHManager"/>.<typeparamref name="Building"/>.
        /// </summary>
        public Building Building 
        { 
            get { return KLHManager.Building; } 
            set { KLHManager.Building = value; } 
        }
        
        /// <summary>
        /// Reference to <typeparamref name="KLHManager"/>.<typeparamref name="Buildings"/> list.
        /// </summary>
        public List<Building> Buildings 
        { 
            get { return KLHManager.Buildings; } 
        }

        /// <summary>
        /// Reference to <typeparamref name="KLHManager"/>.<typeparamref name="Building"/>.<typeparamref name="DrawGrid"/>.
        /// </summary>
        public DrawGrid DrawGrid 
        { 
            get { return Building.DrawGrid; } 
            set { Building.DrawGrid = value; } 
        }

        /// <summary>
        /// Size of the draw plane (X, Y).
        /// </summary>
        public Vector2 DrawAreaSize 
        { 
            get { return drawAreaSize; } 
        }

        /// <summary>
        /// Current building scale of the area.
        /// </summary>
        public static float BuildingScale
        {
            get { return Current.buildingScale; }
        }

        /// <summary>
        /// Currently selected floor.
        /// </summary>
        public int CurrentFloor 
        { 
            get { return currentFloor; } 
        }

        public static float TrueHeight
        {
            get { return Current.CurrentFloor * KLHMath.floorHeight / BuildingScale; }
        }

        #endregion
        #region Static Properties

        /// <summary>
        /// Color array of different building heights (floor 1-9).
        /// </summary>
        public static Color[] DrawColors { 
            get { return Current.drawColors; } }
        
        
        
        /// <summary>
        /// Hard-coded maximum for <typeparamref name="DrawGrid"/>.<typeparamref name="Grid"/> dimensions.
        /// </summary>
        public static Vector2Int MaxGridDimensions { 
            get { return new Vector2Int(100, 67); } }
        
        
        
        /// <summary>
        /// <typeparamref name="Mesh"/> array containing each floor's level <typeparamref name="Mesh"/>.
        /// </summary>
        public static Mesh[] LevelMeshes { 
            get { return Current.levelMeshes; } }
        
        
        
        /// <summary>
        /// <typeparamref name="Mesh"/> array containing each floor's outline <typeparamref name="Mesh"/>.
        /// </summary>
        public static Mesh[] OutlineMeshes { 
            get { return Current.outlineMeshes; } }
        
        
        
        /// <summary>
        /// <typeparamref name="Mesh"/> array containing each floor's bottom <typeparamref name="Mesh"/>.
        /// </summary>
        public static Mesh[] BottomMeshes { 
            get { return Current.bottomMeshes; } }
        
        
        
        /// <summary>
        /// <typeparamref name="BuildingBounds"/> instance generated from <typeparamref name="KLHManager"/>.<typeparamref name="Building"/>.<typeparamref name="DrawGrid"/>.
        /// </summary>
        public static BuildingBounds Bounds { 
            get {return Current.CalculateBuildingBoundingBoxAndCenter();} }

        #endregion

        #endregion


        #region Events

        /// <summary>
        /// Called each time <typeparamref name="DrawGrid"/> is updated.
        /// </summary>
        public static Action BuildingDrawerUpdated;

        /// <summary>
        /// Called each time <typeparamref name="levelMeshes"/>,<typeparamref name="outlineMeshes"/> and <typeparamref name="bottomMeshes"/> are updated.
        /// </summary>
        public static Action BuildingMeshesUpdated;

        /// <summary>
        /// Called each time <typeparamref name="BuildingVolumeModel"/> is updated.
        /// </summary>
        public static Action VolumeModelUpdated;

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Set the story level for drawing. Called with UnityEvent from a Clickable.
        /// </summary>
        /// <param name="_val">Desired level to be set.</param>
        public void StoryButtonClicked (int _val)
        {
            currentFloor = _val;
        }
        
        /// <summary>
        /// Set the building scale for drawing. Called with UnityEvent from a Clickable.
        /// </summary>
        /// <param name="_scale">Desired scale to be set.</param>
        public void ScaleButtonClicked (int _scale)
        {
            buildingScale = _scale;
            SetDrawPlaneAndGridSize();
            SetMaterialParametersAndColors();

            BuildingDrawerUpdated?.Invoke();
        }
        
        /// <summary>
        /// Set the current Building from buildingTemplates.
        /// NOTE: This method is called from Clickables' OnClick UnityEvent.
        /// </summary>
        /// <param name="_index">Index of an object from buildingTemplates list.</param>
        public void SetBuilding (int _index)
        {
            _index = Mathf.Clamp(_index, 0, Buildings.Count - 1);
            SetBuilding(Buildings[_index]);
        }

        /// <summary>
        /// Set <paramref name="_newBuilding"/> as the current Building.
        /// </summary>
        /// <param name="_newBuilding">Building template that is used as the base.</param>
        public void SetBuilding (Building _newBuilding)
        {
            if (null != KLHManager.Building) {

                if (null == DrawGrid) { 
                    
                    DrawGrid = new DrawGrid(MaxGridDimensions.x, MaxGridDimensions.y);
                } 
            }

            Building = _newBuilding;   

            /// Reset list of undoable states when building is changed.
            ResetGridStates();
            UpdateMeshObjects();

            BuildingDrawerUpdated?.Invoke();      
        }

        #endregion
        #region MonoBehaviour Methods

        protected void Awake()
        {
            if (null == Current) { Current = this; }
            if (Current != this) { Destroy(this.gameObject); }
        }

        protected void Update ()
        {
            /*
            if (null == currentDrawAction) 
            {
                DUKE.Controls.Input source = GetSourceDevice();

                if (null != source) 
                {
                    if (source.CurrentInteractable == drawPlane.GetComponent<Interactable>()) 
                    {
                        if (source.IsDesktop && Keyboard.current.leftShiftKey.isPressed) { return; }

                        currentDrawAction = new DrawAction(source, currentFloor);
                        currentDrawAction.StartPoint = source.Hit.point;
                        currentDrawAction.EndPoint = source.Hit.point;
                        currentDrawAction.LocalStartPoint = source.Hit.point - transform.position;

                        drawIndicator.gameObject.SetActive(true);
                    }
                }
            } 
            else 
            {
                if (!currentDrawAction.Source.PrimaryInteractionHeld) 
                {
                    PaintArea(currentDrawAction);
                    currentDrawAction = null;
                    drawBoundsObj.HideShape();         
                    drawIndicator.gameObject.SetActive(false);

                    /// Scale DrawPlane back to normal size and 
                    /// enable building volume model's meshes:
                    drawPlane.GetComponent<BoxCollider>().size = new Vector3(1, 0, 1);
                    ToggleVolumeModelColliders(true);
                } 
                else if (currentDrawAction.Source.TargetTransform == drawPlane) 
                {
                    /// Scale DrawPlane to avoid the indicator getting stuck to the edges and
                    /// disable building volume model's meshes to prevent it from blocking drawing:
                    drawPlane.GetComponent<BoxCollider>().size = new Vector3(10, 0, 10);
                    ToggleVolumeModelColliders(false);
         
                    /// Convert Source's Hit point to local space and clamp it so it can't go beyond drawPlane borders.
                    Vector3 localHitPoint = transform.InverseTransformPoint(currentDrawAction.Source.Hit.point);
                    Vector3 clampedLocalHitPoint = new Vector3(
                        Mathf.Clamp(localHitPoint.x, 0, drawAreaSize.x),
                        localHitPoint.y,
                        Mathf.Clamp(localHitPoint.z, 0, drawAreaSize.y));

                    /// Update DrawAction's EndPoint.
                    currentDrawAction.DrawValue = currentFloor;
                    currentDrawAction.EndPoint = transform.TransformPoint(clampedLocalHitPoint);

                    Vector3 localDrawingStartPoint = currentDrawAction.LocalStartPoint;
                    Vector3 localMinPoint = Vector3.Min(clampedLocalHitPoint, localDrawingStartPoint);
                    Vector3 scale = clampedLocalHitPoint - localDrawingStartPoint;
                    Vector3 indicatorScale = new Vector3(Mathf.Abs(scale.x), 1f, Mathf.Abs(scale.z));

                    drawIndicator.localPosition = localMinPoint + Vector3.up * 0.001f;

                    float heightVal = KLHMath.floorHeight * currentDrawAction.DrawValue / buildingScale;
                    Vector3 lineScale = new Vector3(indicatorScale.x, heightVal, indicatorScale.z);

                    drawBoundsObj.UpdateShape(localMinPoint, lineScale, indicatorLineWidth);
                    drawBoundsObj.SetColor(currentDrawAction.DrawValue > 0 ? drawColors[currentDrawAction.DrawValue - 1] : Color.white);

                    Vector3 heightOffset = transform.up * currentDrawAction.DrawValue / BuildingScale;
                    objDimensions.UpdateDimensions(currentDrawAction.StartPoint + heightOffset, currentDrawAction.EndPoint + heightOffset);
                }
            }
            */
        }
        
        protected void OnEnable ()
        {
            KLHManager.BuildingDrawer = this;
            KLHManager.BuildingDrawerControls = controlsObject;

            drawInteractable.DrawingStarted += BeginDrawing;
            drawInteractable.DrawingOngoing += ContinueDrawing;
            drawInteractable.DrawingEnded += EndDrawing;

            InitializeBuildingDrawer();
        }

        protected void OnDisable ()
        {
            drawInteractable.DrawingStarted -= BeginDrawing;
            drawInteractable.DrawingOngoing -= ContinueDrawing;
            drawInteractable.DrawingEnded -= EndDrawing;
        }
            
        # if UNITY_EDITOR
        void OnDrawGizmos ()
        {
            if (!drawDebug) { return; }

            if (null != DrawGrid) 
            {
                for (int x = 0; x < DrawGrid.Width; x++) 
                {
                    for (int y = 0; y < DrawGrid.Depth; y++) 
                    {
                        DrawCell c = DrawGrid.GetCell(x, y);

                        if (c.Value > 0) 
                        {
                            Gizmos.color = drawColors[c.Value - 1];

                            int[] edgeHeightDifferences = DrawGrid.CellNeighbourElevationDifference(c);

                            for (int i = 0; i < 4; i++) 
                            {
                                int val = edgeHeightDifferences[i];

                                if (val > 0) 
                                {
                                    Vector3 vA = Vector3.zero;
                                    Vector3 vB = Vector3.zero;

                                    if (i == 0) 
                                    {
                                        vA = GridCoordinatesToWorldPosition(x + 0, y + 1);
                                        vB = GridCoordinatesToWorldPosition(x + 1, y + 1);
                                    } 
                                    else if (i == 1) 
                                    {
                                        vA = GridCoordinatesToWorldPosition(x + 1, y + 1);
                                        vB = GridCoordinatesToWorldPosition(x + 1, y + 0);
                                    } 
                                    else if (i == 2) 
                                    {
                                        vA = GridCoordinatesToWorldPosition(x + 1, y + 0);
                                        vB = GridCoordinatesToWorldPosition(x + 0, y + 0);
                                    } 
                                    else 
                                    {
                                        vA = GridCoordinatesToWorldPosition(x + 0, y + 0);
                                        vB = GridCoordinatesToWorldPosition(x + 0, y + 1);
                                    }

                                    vA += transform.up * (KLHMath.floorHeight * c.Value / buildingScale);
                                    vB += transform.up * (KLHMath.floorHeight * c.Value / buildingScale);

                                    Vector3 vC = vA - transform.up * (KLHMath.floorHeight * val / buildingScale);
                                    Vector3 vD = vB - transform.up * (KLHMath.floorHeight * val / buildingScale);

                                    Gizmos.DrawLine(vA, vD);
                                    Gizmos.DrawLine(vB, vC);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endif

        #endregion
        #region Building Setup Methods

        /// <summary>
        /// Setup the variables and references of BuildingDrawer.
        /// </summary>
        public void InitializeBuildingDrawer ()
        {
            currentDrawAction = null;

            SetBuilding(KLHManager.Building);
            SetBuildingMaterialInstances();
            SetDrawPlaneAndGridSize();
            SetMaterialParametersAndColors();
        }

        #endregion
        #region Draw Area Control Methods

        /// <summary>
        /// Get the current source device (VRInput or DesktopInput).
        /// </summary>
        protected DUKE.Controls.Input GetSourceDevice ()
        {
            DUKE.Controls.Input[] inputs = FindObjectsOfType<DUKE.Controls.Input>();

            foreach (DUKE.Controls.Input i in inputs) 
            {
                if (i.TargetTransform == drawPlane && i.PrimaryInteractionHeld) 
                { 
                    return i; 
                }
            }

            return null;
        }


        /// <summary>
        /// Begin the drawing process.
        /// </summary>
        protected void BeginDrawing ()
        {            
            DUKE.Controls.Input source = drawInteractable.Source;
            
            currentDrawAction = new DrawAction(source, transform, currentFloor);
            //currentDrawAction.StartPoint = source.Hit.point;
            currentDrawAction.StartPoint = source.Hit.point;
            currentDrawAction.EndPoint = currentDrawAction.StartPoint;

            drawIndicator.gameObject.SetActive(true);

            // Increase the scale of the collider in order to allow user to drag the draw point beyond the borders.
            drawPlane.GetComponent<BoxCollider>().size = new Vector3(10, 0, 10);

            dimensions = GetComponentsInChildren<Dimension>();
        }

        /// <summary>
        /// Draw by moving the pointer.
        /// </summary>
        protected void ContinueDrawing ()
        {
            if (currentDrawAction.Source.TargetTransform == drawPlane)
            {
                Vector3 rawHit = currentDrawAction.Source.Hit.point;

                // Clamp EndPoint to stay within the draw area.
                currentDrawAction.EndPoint = new Vector3(
                    Mathf.Clamp(rawHit.x, transform.position.x, transform.position.x + drawAreaSize.x),
                    rawHit.y,
                    Mathf.Clamp(rawHit.z, transform.position.z, transform.position.z + drawAreaSize.y)
                );
            }

            Vector3 delta = currentDrawAction.LocalMax - currentDrawAction.LocalMin;
            float height = KLHMath.floorHeight * currentDrawAction.DrawValue / buildingScale;
            delta.y = height;
            
            drawIndicator.localPosition = currentDrawAction.LocalMin;

            drawBoundsObj.UpdateShape(delta, indicatorLineWidth);
            drawBoundsObj.SetColor(currentDrawAction.DrawValue > 0 ? drawColors[currentDrawAction.DrawValue - 1] : Color.white);

            foreach (Dimension d in dimensions)
            {
                d.UpdateDimension(delta);
            }  
        }

        /// <summary>
        /// End the drawing process.
        /// </summary>
        protected void EndDrawing ()
        {
            PaintArea();
            currentDrawAction = null;
            drawBoundsObj.HideShape();         
            drawIndicator.gameObject.SetActive(false);

            // Scale DrawPlane back to normal size.
            drawPlane.GetComponent<BoxCollider>().size = new Vector3(1, 0, 1);
        }

        /// <summary>
        /// Transform world position to grid coordinates.
        /// </summary>
        /// <param name="_wPos"></param>
        /// <returns>World coordinate position to be converted into grid coordinates.</returns>
        protected Vector2Int WorldPositionToGridCoordinates (Vector3 _wPos)
        {
            Vector3 localPosition = transform.InverseTransformPoint(_wPos);
            Vector3 scaledLocalPosition = localPosition * buildingScale;

            Vector2Int convertedPos = new Vector2Int(
                Mathf.FloorToInt(scaledLocalPosition.x),
                Mathf.FloorToInt(scaledLocalPosition.z)
            );

            return convertedPos;
        }
        
        /// <summary>
        /// Transform grid coordinates to world position.
        /// </summary>
        /// <param name="_x">Grid coordinate position on X-axis.</param>
        /// <param name="_y">Grid coordinate position on Y-axis.</param>
        /// <param name="_returnCellCenterPosition">TRUE when the center of the <typeparamref name="DrawCell"/> should be returned.
        /// FALSE when the lower left corner should be returned instead.</param>
        /// <returns>World coordinate position of the specified <typeparamref name="DrawCell"/>.</returns>
        protected Vector3 GridCoordinatesToWorldPosition (int _x, int _y, bool _returnCellCenterPosition = false)
        {
            float adjustment = _returnCellCenterPosition ? 0.5f : 0f;
            Vector3 adjustedCoordinates = new Vector3((_x + adjustment) / buildingScale, 0f, (_y + adjustment) / buildingScale);
            Vector3 wPos = transform.TransformPoint(adjustedCoordinates);

            return wPos;
        }
        
        /// <summary>
        /// Define a rectangular area from <paramref name="_action"/>'s <paramref name="StartPoint"/> and <paramref name="EndPoint"/>.
        /// </summary>
        protected void PaintArea ()
        {
            Vector2Int startCoordinates = WorldPositionToGridCoordinates(transform.TransformPoint(currentDrawAction.LocalStartPoint));
            Vector2Int endCoordinates = WorldPositionToGridCoordinates(currentDrawAction.EndPoint);
            Vector2Int min = Vector2Int.Min(startCoordinates, endCoordinates);
            Vector2Int max = Vector2Int.Max(startCoordinates, endCoordinates);
            int maxLevel = 0;

            for (int x = min.x; x <= max.x; x++) 
            {
                for (int y = min.y; y <= max.y; y++) 
                {
                    DrawCell c = DrawGrid.GetCell(x, y);

                    if (null != c) 
                    {
                        c.Value = currentDrawAction.DrawValue;
                        if (c.Value > maxLevel) { maxLevel = c.Value; }
                    }
                }
            }

            SaveGridState();

            if (KLHManager.EditTemplates) 
            { 
                DrawGrid.SaveGrid();     
            }
            
            UpdateMeshObjects();

            BuildingDrawerUpdated?.Invoke();
        }
        
        /// <summary>
        /// Resize 'drawPlane' and the material's parameters.
        /// </summary>
        protected void SetDrawPlaneAndGridSize ()
        {
            Vector3 newScale = new Vector3(drawAreaSize.x, 1f, drawAreaSize.y);
            int areaWidth = MaxGridDimensions.x;
            int areaDepth = MaxGridDimensions.y;

            drawPlane.localScale = newScale;
            backgroundPlane.localScale = newScale;
            DrawGrid.Width = areaWidth;
            DrawGrid.Depth = areaDepth;
        }
        
        #endregion
        #region Undo / Redo Methods

        /// <summary>
        /// Save the current Grid as an instance.
        /// Trim Grids that branch from the new current. 
        /// </summary>
        protected void SaveGridState ()
        {    
            // Delete the states that have been undone. 
            // This happens when currentGridStateIndex is not equal to the last index of the gridStates list, meaning undoing has been performed.
            int removeCount = (undoGridStates.Count - 1) - currentGridStateIndex;

            for (int i = 0; i < removeCount; i++) 
            {
                int removedIndex = undoGridStates.Count - 1;
                undoGridStates.RemoveAt(removedIndex);
            }

            // Shift the states by one if the maximum has been reached.
            if (undoGridStates.Count == maxGridStates) 
            { 
                undoGridStates.RemoveAt(0);
                currentGridStateIndex--;
            }

            // Save the new state.
            currentGridStateIndex++;
            undoGridStates.Add( new DrawGridState( new DrawGrid( DrawGrid.Cells ), currentGridStateIndex.ToString() ) );
        }

        /// <summary>
        /// Return backwards on the list of saved Grids.
        /// </summary>
        public void UndoGridState ()
        {
            if (currentGridStateIndex > 0) 
            {
                currentGridStateIndex--;
                DrawGrid = new DrawGrid(undoGridStates[currentGridStateIndex].Grid);

                UpdateMeshObjects();
                BuildingDrawerUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Advance forward on the list of saved Grids.
        /// </summary>
        public void RedoGridState ()
        {
            if (currentGridStateIndex < undoGridStates.Count - 1) 
            {
                currentGridStateIndex++;

                DrawGrid = new DrawGrid(undoGridStates[currentGridStateIndex].Grid);

                UpdateMeshObjects();
                BuildingDrawerUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Reset gridStates. 
        /// </summary>
        protected void ResetGridStates ()
        {
            if (null == undoGridStates) 
            { 
                undoGridStates = new List<DrawGridState>(); 
            }

            undoGridStates.Clear();
            currentGridStateIndex = -1;
            SaveGridState();
        }

        #endregion
        #region Mesh Creation Methods

        /// <summary>
        /// Update the mesh of every level in BuildingDrawer.
        /// </summary>
        protected void UpdateMeshObjects ()
        {
            levelMeshes = new Mesh[9];
            outlineMeshes = new Mesh[9];
            bottomMeshes = new Mesh[9];

            for (int i = 1; i <= levelMeshContainer.childCount; i++) 
            {
                UpdateMeshObject(i); 
            }

            BuildingMeshesUpdated?.Invoke();
        }
        
        /// <summary>
        /// Update the mesh of a selected level in BuildingDrawer.
        /// </summary>
        /// <param name="_level">Selected level.</param>
        protected void UpdateMeshObject (int _level)
        {
            if (_level == 0) { return; }

            Mesh levelMesh = CreateDefaultMesh(_level);
            Mesh outlineMesh = CreateCleanOutlineMesh(_level);
            Mesh bottomMesh = CreateBottomMesh(_level);

            MeshFilter levelMeshFilter = levelMeshContainer.GetChild(_level - 1).GetComponent<MeshFilter>();
            MeshRenderer levelMeshRend = levelMeshContainer.GetChild(_level - 1).GetComponent<MeshRenderer>();
            MeshFilter outlineMeshFilter = outlineMeshContainer.GetChild(_level - 1).GetComponent<MeshFilter>();
            MeshRenderer outlineMeshRend = outlineMeshContainer.GetChild(_level - 1).GetComponent<MeshRenderer>(); 
            MeshFilter bottomMeshFilter = bottomMeshContainer.GetChild(_level - 1).GetComponent<MeshFilter>();
            MeshRenderer bottomMeshRend = bottomMeshContainer.GetChild(_level - 1).GetComponent<MeshRenderer>(); 

            levelMeshRend.shadowCastingMode = outlineMeshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;      

            levelMeshFilter.mesh = levelMesh;
            levelMeshes[_level - 1] = levelMesh;
            outlineMeshFilter.mesh = outlineMesh;
            outlineMeshes[_level - 1] = outlineMesh;
            bottomMeshFilter.mesh = bottomMesh;
            bottomMeshes[_level - 1] = bottomMesh;

            if (null == levelMeshRend.material) 
            {
                Material m = new Material(levelMeshMaterial);
                m.SetColor("_Color", drawColors[_level - 1]);
                levelMeshRend.material = m;
            }

            if (null == outlineMeshRend.material) 
            {
                Material m = new Material(outlineMeshMaterial);
                m.SetColor("_Color", drawColors[_level - 1]);
                outlineMeshRend.material = m;
            }

            if (null == bottomMeshRend.material) 
            {
                Material m = new Material(bottomMeshMaterial);
                m.SetColor("_Color", drawColors[_level - 1]);
                bottomMeshRend.material = m;
            }
        }
        
        /// <summary>
        /// Create a mesh of a single level (drawValue).
        /// </summary>
        /// <param name="_level"></param>
        /// <returns>Default mesh of the selected level defined by <paramref name="_level"/>.</returns>
        protected Mesh CreateDefaultMesh (int _level)
        {
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
            int vIndex = 0;
            int iIndex = 0;

            int trimWidth = trimOverflowCells ? Mathf.FloorToInt(drawAreaSize.x * buildingScale) + 1 : DrawGrid.Width;
            int trimDepth = trimOverflowCells ? Mathf.FloorToInt(drawAreaSize.y * buildingScale) + 1: DrawGrid.Depth;

            float height = _level * KLHMath.floorHeight / buildingScale;

            for (int x = 0; x < trimWidth; x++) 
            {
                for (int y = 0; y < trimDepth; y++) 
                {
                    DrawCell c = DrawGrid.GetCell(x, y);

                    if (null == c) 
                    { 
                        #if UNITY_EDITOR
                        Debug.LogError("BuildingDrawer.CreateDefaultMesh(): Mesh was null (cell = ("+x+","+y+")."); 
                        #endif
                        
                        return null; 
                    }

                    if (c.Value != _level) { continue; }

                    /// Create top mesh:
                    Vector3 vA = GridCoordinatesToWorldPosition(x + 0, y + 0) - transform.position + transform.up * height;
                    Vector3 vB = GridCoordinatesToWorldPosition(x + 0, y + 1) - transform.position + transform.up * height;
                    Vector3 vC = GridCoordinatesToWorldPosition(x + 1, y + 1) - transform.position + transform.up * height;
                    Vector3 vD = GridCoordinatesToWorldPosition(x + 1, y + 0) - transform.position + transform.up * height;
                    
                    vertices.Add(vA);
                    vertices.Add(vB);
                    vertices.Add(vC);
                    vertices.Add(vD);

                    triangles.Add(vIndex + 0);
                    triangles.Add(vIndex + 1);
                    triangles.Add(vIndex + 2);
                    triangles.Add(vIndex + 2);
                    triangles.Add(vIndex + 3);
                    triangles.Add(vIndex + 0);

                    uvs.Add(new Vector2(0,0));
                    uvs.Add(new Vector2(0,0));
                    uvs.Add(new Vector2(0,0));
                    uvs.Add(new Vector2(0,0));
                            
                    vIndex += 4;
                    iIndex += 6;
                        
                    /// Create the vertical parts.   
                    int[] neighbourHeightDiff = DrawGrid.CellNeighbourElevationDifference(c);

                    for (int i = 0; i < 4; i++) 
                    {
                        float val = neighbourHeightDiff[i] * KLHMath.floorHeight / buildingScale;

                        if (neighbourHeightDiff[i] > 0) 
                        {
                            Vector3 vE;
                            Vector3 vF;

                            if (i == 0)         { vE = vB; vF = vC; } 
                            else if (i == 1)    { vE = vC; vF = vD; } 
                            else if (i == 2)    { vE = vD; vF = vA; } 
                            else                { vE = vA; vF = vB; }

                            Vector3 vG = vE - transform.up * val;
                            Vector3 vH = vF - transform.up * val;

                            vertices.Add(vE);
                            vertices.Add(vG);
                            vertices.Add(vH);
                            vertices.Add(vF);

                            triangles.Add(vIndex + 0);
                            triangles.Add(vIndex + 1);
                            triangles.Add(vIndex + 2);
                            triangles.Add(vIndex + 2);
                            triangles.Add(vIndex + 3);
                            triangles.Add(vIndex + 0);

                            uvs.Add(new Vector2(0,0));
                            uvs.Add(new Vector2(0,0));
                            uvs.Add(new Vector2(0,0));
                            uvs.Add(new Vector2(0,0));

                            vIndex += 4;
                            iIndex += 6;
                        }              
                    }
                }
            }

            Mesh m = new Mesh();

            m.name = "Level " + _level;
            m.vertices = vertices.ToArray();
            m.triangles = triangles.ToArray();
            m.uv = uvs.ToArray();
            m.RecalculateNormals();
            m.Optimize();

            return m;
        }
        
        /// <summary>
        /// Create the mesh at the bottom of the Building.
        /// </summary>
        /// <param name="_level"></param>
        /// <returns>Bottom mesh of the selected level defined by <paramref name="_level"/>.</returns>
        protected Mesh CreateBottomMesh (int _level) 
        {
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
            int vIndex = 0;
            int iIndex = 0;

            int trimWidth = trimOverflowCells ? Mathf.FloorToInt(drawAreaSize.x * buildingScale) + 1 : DrawGrid.Width;
            int trimDepth = trimOverflowCells ? Mathf.FloorToInt(drawAreaSize.y * buildingScale) + 1: DrawGrid.Depth;

            float height = _level * KLHMath.floorHeight / buildingScale;

            for (int x = 0; x < trimWidth; x++) 
            {
                for (int y = 0; y < trimDepth; y++) 
                {
                    DrawCell c = DrawGrid.GetCell(x, y);

                    if (c.Value != _level) { continue; }
                    
                    /// Create bottom mesh:
                    Vector3 vA2 = GridCoordinatesToWorldPosition(x + 0, y + 0) - transform.position + transform.up * 0.0011f + transform.right * bottomMeshTileInset + transform.forward * bottomMeshTileInset;
                    Vector3 vB2 = GridCoordinatesToWorldPosition(x + 0, y + 1) - transform.position + transform.up * 0.0011f + transform.right * bottomMeshTileInset - transform.forward * bottomMeshTileInset;
                    Vector3 vC2 = GridCoordinatesToWorldPosition(x + 1, y + 1) - transform.position + transform.up * 0.0011f - transform.right * bottomMeshTileInset - transform.forward * bottomMeshTileInset;
                    Vector3 vD2 = GridCoordinatesToWorldPosition(x + 1, y + 0) - transform.position + transform.up * 0.0011f - transform.right * bottomMeshTileInset + transform.forward * bottomMeshTileInset;

                    vertices.Add(vA2);
                    vertices.Add(vB2);
                    vertices.Add(vC2);
                    vertices.Add(vD2);

                    triangles.Add(vIndex + 0);
                    triangles.Add(vIndex + 1);
                    triangles.Add(vIndex + 2);
                    triangles.Add(vIndex + 2);
                    triangles.Add(vIndex + 3);
                    triangles.Add(vIndex + 0);

                    uvs.Add(new Vector2(0,0));
                    uvs.Add(new Vector2(0,0));
                    uvs.Add(new Vector2(0,0));
                    uvs.Add(new Vector2(0,0));
                                        
                    vIndex += 4;
                    iIndex += 6;
                }
            }

            Mesh m = new Mesh();

            m.name = "Level " + _level;
            m.vertices = vertices.ToArray();
            m.triangles = triangles.ToArray();
            m.uv = uvs.ToArray();
            m.RecalculateNormals();
            m.Optimize();

            return m;
        }

        /// <summary>
        /// Create a clean outline mesh for the selected level.
        /// </summary>
        /// <param name="_level"></param>
        /// <returns>Outline mesh of the selected level defined by <paramref name="_level"/>.</returns>
        protected Mesh CreateCleanOutlineMesh (int _level) 
        {
            vertices.Clear();
            triangles.Clear();

            float height = _level * KLHMath.floorHeight / buildingScale;

            /// Loop through every cell.
            for (int x = 0; x < DrawGrid.Width; x++) 
            {
                for (int y = 0; y < DrawGrid.Depth; y++) 
                {
                    DrawCell originalCell = DrawGrid.Cells[x,y];                
                    
                    if (originalCell.Value != _level) { continue; }

                    /// Corner points:
                    Vector3 vA = GridCoordinatesToWorldPosition(x + 0, y + 0) - transform.position;
                    Vector3 vB = GridCoordinatesToWorldPosition(x + 0, y + 1) - transform.position;
                    Vector3 vC = GridCoordinatesToWorldPosition(x + 1, y + 1) - transform.position;
                    Vector3 vD = GridCoordinatesToWorldPosition(x + 1, y + 0) - transform.position;
                    
                    /// Get information about the cell's neighbours.
                    int[] elevDiff = DrawGrid.CellNeighbourElevationDifference(originalCell, true);
                    Vector3 v1, v2, v3;

                    /// Create horizontal runs.
                    for (int i = 0; i < 4; i++) 
                    {
                        if (i == 0)         { v1 = vB; v2 = vC; } 
                        else if (i == 1)    { v1 = vC; v2 = vD; } 
                        else if (i == 2)    { v1 = vD; v2 = vA; } 
                        else                { v1 = vA; v2 = vB; }

                        if (elevDiff[i] != 0) 
                        {
                            if (elevDiff[i] == _level) 
                            {
                                AddToHorizontalRun(v1, v2, 0, vertices, triangles, runResolution, runThickness); 
                            }   

                            AddToHorizontalRun(v1, v2, height, vertices, triangles, runResolution, runThickness); 
                            
                            int fwd = (i == 3 ? 0 : i + 1);
                            int bck = (i == 0 ? 3 : i - 1);
                            int diffA = elevDiff[i];
                            int diffB = elevDiff[fwd]; 

                            /// If adjacent sides are higher than neighbours:
                            if ((diffA != 0 && diffB != 0) && (diffA > 0 || diffB > 0)) 
                            {
                                int levelDifference = Mathf.Max(diffA, diffB);
                                v2 += transform.up * height;
                                v3 = v2 - transform.up * levelDifference * KLHMath.floorHeight / buildingScale;

                                AddToVerticalRun(v3, v2, vertices, triangles, runResolution, runThickness);
                            }
                        }           
                    }

                    /// If every direct side is of equal height:
                    if (elevDiff[0] == 0 && elevDiff[1] == 0 && elevDiff[2] == 0 && elevDiff[3] == 0) 
                    {
                        /// Check if diagonal neighbours are of same height.
                        for (int i = 4; i < 8; i++) 
                        {
                            int levelDifference = elevDiff[i];
                            v1 = vC+ transform.up * height;

                            if (i == 5)         { v1 = vD + transform.up * height; } 
                            else if (i == 6)    { v1 = vA+ transform.up * height; } 
                            else if (i == 7)    { v1 = vB+ transform.up * height; }

                            if (levelDifference > 0) 
                            {
                                v2 = v1 - transform.up * levelDifference * KLHMath.floorHeight / buildingScale;
                                AddToVerticalRun(v2, v1, vertices, triangles, runResolution, runThickness);
                            }           
                        }
                    }
                }
            }

            Mesh m = new Mesh();

            m.name = "Level " + _level;
            m.vertices = vertices.ToArray();
            m.triangles = triangles.ToArray();
            m.uv = new Vector2[m.vertices.Length];
            m.RecalculateNormals();
            m.Optimize();

            return m;
        }

        /// <summary>
        /// Create a part of a horizontal frame mesh.
        /// </summary>
        /// <param name="_vA">Point (from).</param>
        /// <param name="_vB">Point (to).</param>
        /// <param name="_height">Height.</param>
        /// <param name="_verts">A list of vertices the part is added to.</param>
        /// <param name="_tris">A list of triangles the part is added to.</param>
        /// <param name="_runResolution">Determines the cross-section shape of the run.</param>
        /// <param name="_meshThickness">Determines the cross-section thickness of the run.</param>
        protected void AddToHorizontalRun (Vector3 _vA, Vector3 _vB, float _height, List<Vector3> _verts, List<int> _tris, int _runResolution, float _meshThickness) 
        {
            Vector3 vA_top = _vA + transform.up * _height;
            Vector3 vB_top = _vB + transform.up * _height;
            Vector3 vA_bottom = _vA;
            Vector3 vB_bottom = _vB;

            Vector3 dir = (vA_top - vB_top).normalized;
            float angleStep = 360f / _runResolution;

            for (int i = 0; i < _runResolution; i++) 
            {
                /// Top run:
                _verts.Add (vA_top + Quaternion.AngleAxis((i + 0) * angleStep, dir) * transform.up * _meshThickness);
                _verts.Add (vB_top + Quaternion.AngleAxis((i + 0) * angleStep, dir) * transform.up * _meshThickness);
                _verts.Add (vB_top + Quaternion.AngleAxis((i + 1) * angleStep, dir) * transform.up * _meshThickness);
                _verts.Add (vA_top + Quaternion.AngleAxis((i + 1) * angleStep, dir) * transform.up * _meshThickness);

                _tris.Add(_verts.Count - 4);
                _tris.Add(_verts.Count - 3);
                _tris.Add(_verts.Count - 2);
                _tris.Add(_verts.Count - 2);
                _tris.Add(_verts.Count - 1);
                _tris.Add(_verts.Count - 4);

                uvs.Add(new Vector2(0,0));
                uvs.Add(new Vector2(0,0));
                uvs.Add(new Vector2(0,0));
                uvs.Add(new Vector2(0,0));
            }
        }

        /// <summary>
        /// Create a part of a vertical frame mesh.
        /// </summary>
        /// <param name="_top">Point (from).</param>
        /// <param name="_bot">Point (to).</param>
        /// <param name="_height">Height.</param>
        /// <param name="_verts">A list of vertices the part is added to.</param>
        /// <param name="_tris">A list of triangles the part is added to.</param>
        /// <param name="_runResolution">Determines the cross-section shape of the run.</param>
        /// <param name="_meshThickness">Determines the cross-section thickness of the run.</param>
        protected void AddToVerticalRun (Vector3 _top, Vector3 _bot, List<Vector3> _verts, List<int> _tris, int _runResolution, float _meshThickness) 
        {
            Vector3 dir = (_bot - _top).normalized;
            float angleStep = 360f / _runResolution;

            for (int i = 0; i < _runResolution; i++) 
            {
                /// Vertical run:
                _verts.Add (_top + Quaternion.AngleAxis((i + 0) * angleStep, dir) * transform.forward * _meshThickness);
                _verts.Add (_bot + Quaternion.AngleAxis((i + 0) * angleStep, dir) * transform.forward * _meshThickness);
                _verts.Add (_bot + Quaternion.AngleAxis((i + 1) * angleStep, dir) * transform.forward * _meshThickness);
                _verts.Add (_top + Quaternion.AngleAxis((i + 1) * angleStep, dir) * transform.forward * _meshThickness);

                _tris.Add(_verts.Count - 4);
                _tris.Add(_verts.Count - 2);
                _tris.Add(_verts.Count - 3);
                _tris.Add(_verts.Count - 2);
                _tris.Add(_verts.Count - 4);
                _tris.Add(_verts.Count - 1);   

                uvs.Add(new Vector2(0,0));
                uvs.Add(new Vector2(0,0));
                uvs.Add(new Vector2(0,0));
                uvs.Add(new Vector2(0,0));
            }      
        }

        /// <summary>
        /// Get a new V2 direction by turning 90 degrees clockwise or anti-clockwise.
        /// </summary>
        /// <param name="_currentDirection"></param>
        /// <param name="_clockwise"></param>
        /// <returns>The new direction on a two-dimensional plane.</returns>
        protected Vector2Int GetNewDirection (Vector2Int _currentDirection, bool _clockwise)
        {
            if (_clockwise) 
            {
                if (_currentDirection == Vector2Int.up)         { return Vector2Int.right; } 
                else if (_currentDirection == Vector2Int.right) { return Vector2Int.down; } 
                else if (_currentDirection == Vector2Int.down)  { return Vector2Int.left; } 
                else                                            { return Vector2Int.up; }
            } 
            else 
            {
                if (_currentDirection == Vector2Int.up)         { return Vector2Int.left; } 
                else if (_currentDirection == Vector2Int.left)  { return Vector2Int.down; } 
                else if (_currentDirection == Vector2Int.down)  { return Vector2Int.right; } 
                else                                            { return Vector2Int.up; }
            }
        }
        
        /// <summary>
        /// Calculate the bounds and center of a building.
        /// </summary>
        /// <returns>A calculated BuildingBounds instance of the Building.</returns>
        protected BuildingBounds CalculateBuildingBoundingBoxAndCenter ()
        {
            Vector2Int min = new Vector2Int(DrawGrid.Width, DrawGrid.Depth);
            Vector2Int max = new Vector2Int(0, 0);
            float height = 0f;
            bool isEmpty = true;

            for (int x = 0; x < DrawGrid.Width; x++) 
            {
                for (int y = 0; y < DrawGrid.Depth; y++) 
                {
                    DrawCell c = DrawGrid.GetCell(x, y);

                    if (null == c) 
                    { 
                        #if UNITY_EDITOR
                        Debug.LogError("BuildingDrawer.CalculateBuildingBoundingBoxAndCenter(): NULL: " + x + "," + y); 
                        #endif

                        return new BuildingBounds();
                    }
                
                    if (c.Value > 0) 
                    {
                        Vector2Int coordinates = new Vector2Int(c.X, c.Y);
                        min = Vector2Int.Min(min, coordinates);
                        max = Vector2Int.Max(max, coordinates);
                        height = Mathf.Max(height, c.Value * KLHMath.floorHeight);
                        isEmpty = false;
                    }
                }
            }

            Vector2Int dimensionsV2 = new Vector2Int(max.x - min.x, max.y - min.y);
            Vector2 centerV2 = new Vector2(min.x + dimensionsV2.x / 2f, min.y + dimensionsV2.y / 2f);
            Vector2 originV2 = -centerV2;
            Vector3 dimensionsV3 = new Vector3(dimensionsV2.x, height, dimensionsV2.y) / buildingScale;
            Vector3 originV3 = new Vector3(originV2.x, 0f, originV2.y) / buildingScale;

            return new BuildingBounds(dimensionsV3, originV3, isEmpty);
        }

        #endregion
        #region Material Setup Methods

        /// <summary>
        /// Update material parameters.
        /// </summary>
        protected void SetMaterialParametersAndColors ()
        {
            drawPlaneMaterial = Instantiate(Resources.Load("Materials/BuildingDrawer/DrawGrid") as Material);
            backgroundMaterial = Instantiate(Resources.Load("Materials/EmissiveColor") as Material);

            float xSize = drawAreaSize.x / buildingScale * 1000f;
            float ySize = drawAreaSize.y / buildingScale * 1000f;

            drawPlaneMaterial.SetColor("_LineColor", lineColor);
            drawPlaneMaterial.SetColor("_BackgroundColor", Color.clear);
            drawPlaneMaterial.SetFloat("_Line_Width", lineWidth / buildingScale);
            drawPlaneMaterial.SetFloat("_Thick_Line_Width", thickLineWidth / buildingScale);
            drawPlaneMaterial.SetFloat("_Horizontal_Segments", buildingScale * drawAreaSize.x);
            drawPlaneMaterial.SetFloat("_Horizontal_Large_Segments", buildingScale * drawAreaSize.x / thickLineInterval);
            drawPlaneMaterial.SetFloat("_Vertical_Segments", buildingScale * drawAreaSize.y);
            drawPlaneMaterial.SetFloat("_Vertical_Large_Segments", buildingScale * drawAreaSize.y / thickLineInterval);
            drawPlaneMaterial.SetVector("_Area_Size", new Vector2(xSize, ySize));
            backgroundMaterial.SetColor("_Color", baseColor);
            backgroundMaterial.SetFloat("_Intensity", 1f);
            backgroundMaterial.SetFloat("_Smoothness", 0f);
            backgroundMaterial.SetFloat("_Metallic", 0f);

            drawPlane.GetComponent<MeshRenderer>().material = drawPlaneMaterial;
            backgroundPlane.GetComponent<MeshRenderer>().material = backgroundMaterial;
        }
    
        /// <summary>
        /// Setup Materials for meshObjects.
        /// </summary>
        protected void SetBuildingMaterialInstances ()
        {
            for(int i = 0; i < levelMeshContainer.childCount; i++) 
            {
                SetMaterialInstance(levelMeshContainer, levelMeshMaterial, i);
                SetMaterialInstance(outlineMeshContainer, outlineMeshMaterial, i);
                SetMaterialInstance(bottomMeshContainer, bottomMeshMaterial, i);
            }
        }

        /// <summary>
        /// Create a new material instance.
        /// </summary>
        /// <param name="_container">The parent of objects to which new instances are added.</param>
        /// <param name="_mat">Material to be instantiated.</param>
        /// <param name="_index">Index of the object within its parent.</param>
        protected void SetMaterialInstance (Transform _container, Material _mat, int _index) 
        {
            Transform child = _container.GetChild(_index);
            MeshRenderer mRend = child.GetComponent<MeshRenderer>();
            Material m = new Material(_mat);
            m.SetColor("_Color", drawColors[_index]);
            mRend.material = m;
        }

        #endregion

        #endregion


    } ///  End of Class


} /// End of Namespace