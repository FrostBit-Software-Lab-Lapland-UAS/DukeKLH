/******************************************************************************
 * File        : CurvedUIElement.cs
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
using System;


namespace DUKE.UI {


    /// <summary>
    /// Store quad information to help in mesh subdivision.
    /// </summary>
    public struct Quad 
    {
        #region Variables

        /// <summary>
        /// Vertices of the <typeparamref name="Quad"/>.
        /// </summary>
        public UIVertex[] verts;

        /// <summary>
        /// Triangles of the <typeparamref name="Quad"/>.
        /// </summary>
        public int[] tris;

        #endregion


        #region Properties

        /// <summary>
        /// Width of the <typeparamref name="Quad"/>.
        /// </summary>
        public float Width { 
            get { return CalculateWidth(); } }

        /// <summary>
        /// Height of the <typeparamref name="Quad"/>.
        /// </summary>
        public float Height { 
            get { return CalculateHeight(); } }

        /// <summary>
        /// Center point of the <typeparamref name="Quad"/>.
        /// </summary>
        public Vector3 Center { 
            get { return CalculateCenter(); } }

        #endregion


        #region Methods

        /// <summary>
        /// Calculate the center point.
        /// </summary>
        /// <returns></returns>
        Vector3 CalculateCenter ()
        {
            return Vector3.Lerp(verts[0].position, verts[2].position, 0.5f);
        }

        /// <summary>
        /// Calculate <typeparamref name="Quad"/> width.
        /// </summary>
        /// <returns>Width of the <typeparamref name="Quad"/>.</returns>
        float CalculateWidth ()
        {
            if (null == verts) return 0f;
            else return verts[2].position.x - verts[0].position.x;
        }

        /// <summary>
        /// Calculate <typeparamref name="Quad"/> height.
        /// </summary>
        /// <returns>Height of the <typeparamref name="Quad"/>.</returns>
        float CalculateHeight ()
        {
            if (null == verts) return 0f;
            else return verts[2].position.y - verts[0].position.y;
        }

        /// <summary>
        /// Calculate the number of <typeparamref name="Quads"/>.
        /// </summary>
        /// <param name="_treshold"></param>
        /// <returns>The number of <typeparamref name="Quads"/> on two axii.</returns>
        public Vector2Int QuadCount (float _treshold)
        {
            if (null == verts || null == tris) return Vector2Int.zero;

            float width = Width;
            float height = Height;
            int xAmount = 1 + Mathf.FloorToInt(width / _treshold);
            int yAmount = 1 + Mathf.FloorToInt(height / _treshold);

            return new Vector2Int(xAmount, yAmount);
        }
    
        #endregion
    

    } /// End of Struct



    /// <summary>
    /// Store information of <typeparamref name="Rect"/> and <typeparamref name="RectTransform"/>, and compare them to figure out if the object requires updating.
    /// </summary>
    public struct RectInfo 
    {
        #region Variables

        #region Vector2 Variables

        /// <summary>
        /// Center of the <typeparamref name="Rect"/> in pixel coordinates.
        /// </summary>
        public Vector2 rCenter;
        
        
        /// <summary>
        /// Position of the <typeparamref name="Rect"/> in pixel coordinates.
        /// </summary>
        public Vector2 rPosition;
        
        /// <summary>
        /// Size of the <typeparamref name="Rect"/>.
        /// </summary>
        public Vector2 rSize;
        
        /// <summary>
        /// Minimum coordinates of the <typeparamref name="Rect"/>.
        /// </summary>
        public Vector2 rMin;
        
        
        /// <summary>
        /// Maximum coordinates of the <typeparamref name="Rect"/>.
        /// </summary>
        public Vector2 rMax;

        #endregion
        #region Float Variables

        /// <summary>
        /// Width of the <typeparamref name="Rect"/>.
        /// </summary>
        public float rWidth;
        
        
        /// <summary>
        /// Height of the <typeparamref name="Rect"/>.
        /// </summary>
        public float rHeight;
        
        
        /// <summary>
        /// X-axis pivot of the <typeparamref name="Rect"/>.
        /// </summary>
        public float rX;
        
        /// <summary>
        /// Y-axis pivot of the <typeparamref name="Rect"/>.
        /// /// </summary>
        public float rY;
        
        /// <summary>
        /// Minimum X-axis anchor point of the <typeparamref name="Rect"/>.
        /// </summary>
        public float rXMin;
        
        /// <summary>
        /// Maximum X-axis anchor point of the <typeparamref name="Rect"/>.
        /// </summary>
        public float rXMax;
        
        /// <summary>
        /// Minimum Y-axis anchor point of the <typeparamref name="Rect"/>.
        /// </summary>
        public float rYMin;
        
        /// <summary>
        /// Maximum Y-axis anchor point of the <typeparamref name="Rect"/>.
        /// </summary>
        public float rYMax;

        #endregion

        #endregion


        #region Properties

        /// <summary>
        /// Rotation of the <typeparamref name="RectTransform"/>.
        /// </summary>
        public Quaternion rtRotation;
        
        /// <summary>
        /// Position of the <typeparamref name="RectTransform"/> in pixel coordinates.
        /// </summary>
        public Vector3 rtPosition;
            
        /// <summary>
        /// Local position of the <typeparamref name="RectTransform"/> in pixel coordinates.
        /// </summary>
        public Vector3 rtLocalPosition;     
        
        /// <summary>
        /// Local scale of the <typeparamref name="RectTransform"/>.
        /// </summary>
        public Vector3 rtLocalScale;
         
        /// <summary>
        /// World position of the <typeparamref name="RectTransform"/>.
        /// </summary>
        public Vector3 worldPos;
        
        /// <summary>
        /// Minimum anchor of the <typeparamref name="RectTransform"/>.
        /// </summary>
        public Vector2 rtAnchorMin;
        
        /// <summary>
        /// Maximum anchor the <typeparamref name="RectTransform"/>.
        /// </summary>
        public Vector2 rtAnchorMax;

        #endregion


        #region Methods

        /// <summary>
        /// Set the values of this <typeparamref name="RectInfo"/> based on the <paramref name="_rt"/>.
        /// </summary>
        /// <param name="_rt"><typeparamref name="RectTransform"/> from which values are copied.</param>
        public void SetValues (RectTransform _rt)
        {
            Rect r = _rt.rect;

            rCenter = r.center;
            rPosition = r.position;
            rSize = r.size;
            rMin = r.min;
            rMax = r.max;
            rWidth = r.width;
            rHeight = r.height;
            rX = r.x;
            rXMin = r.xMin;
            rXMax = r.xMax;
            rY = r.y;
            rYMin = r.yMin;
            rYMax = r.yMax;

            rtRotation = _rt.rotation;
            rtPosition = _rt.position;
            worldPos = _rt.TransformPoint(_rt.position);
            rtLocalPosition = _rt.localPosition;
            rtLocalScale = _rt.localScale;
            rtAnchorMin = _rt.anchorMin;
            rtAnchorMax = _rt.anchorMax;
        }

        /// <summary>
        /// Set the values of this <typeparamref name="RectInfo"/> based on <paramref name="_other"/>.
        /// </summary>
        /// <param name="_other">Another <typeparamref name="RectInfo"/> to copy values from.</param>
        public void SetValues (RectInfo _other)
        {
            rCenter = _other.rCenter;
            rPosition = _other.rPosition;
            rSize = _other.rSize;
            rMin = _other.rMin;
            rMax = _other.rMax;
            rWidth = _other.rWidth;
            rHeight = _other.rHeight;
            rX = _other.rX;
            rXMin = _other.rXMin;
            rXMax = _other.rXMax;
            rY = _other.rY;
            rYMin = _other.rYMin;
            rYMax = _other.rYMax;

            rtRotation = _other.rtRotation;
            rtPosition = _other.rtPosition;
            worldPos = _other.worldPos;
            rtLocalPosition = _other.rtLocalPosition;
            rtLocalScale = _other.rtLocalScale;
            rtAnchorMin = _other.rtAnchorMin;
            rtAnchorMax = _other.rtAnchorMax;
        }

        /// <summary>
        /// Check if values of this and another RectInfo are identical.
        /// </summary>
        /// <param name="_other">Another <typeparamref name="RectInfo"/> to compare against.</param>
        /// <returns>TRUE if values of this instance and <paramref name="_other"/> are equal.</returns>
        public bool ValuesAreEqualTo (RectInfo _other)
        {
            if (rCenter == _other.rCenter
                && rPosition == _other.rPosition
                && rSize == _other.rSize
                && rMin == _other.rMin
                && rMax == _other.rMax
                && rWidth == _other.rWidth
                && rHeight == _other.rHeight
                && rX == _other.rX
                && rXMin == _other.rXMin
                && rXMax == _other.rXMax
                && rY == _other.rY
                && rYMin == _other.rYMin
                && rYMax == _other.rYMax

                && rtRotation == _other.rtRotation
                && rtPosition == _other.rtPosition
                && worldPos == _other.worldPos
                && rtLocalPosition == _other.rtLocalPosition
                && rtLocalScale == _other.rtLocalScale
                && rtAnchorMin == _other.rtAnchorMin
                && rtAnchorMax == _other.rtAnchorMax) {

                return true;
            } 

            return false;
        }
        
        /// <summary>
        /// Set values to zero.
        /// </summary>
        public void ResetValues()
        {
            rCenter = new Vector2(0, 0);
            rPosition = new Vector2(0, 0);
            rSize = new Vector2(0, 0);
            rMin = new Vector2(0, 0);
            rMax = new Vector2(0, 0);
            rWidth = 0f;
            rHeight = 0f;
            rX = 0f;
            rXMin = 0f;
            rXMax = 0f;
            rY = 0f;
            rYMin = 0f;
            rYMax = 0f;

            rtRotation = new Quaternion(0, 0, 0, 0);
            rtPosition = new Vector3(0, 0, 0);
            rtLocalPosition = new Vector3(0, 0, 0);
            rtLocalScale = new Vector3(0, 0, 0);
            rtAnchorMin = new Vector2(0, 0);
            rtAnchorMax = new Vector2(0, 0);
        }
    
        #endregion
    

    } /// End of Struct



    /// <summary>
    /// Enable curving of an UI element that uses Image or TextMeshProUGUI component to render onto a canvas.
    /// Curved
    /// </summary>
    [ExecuteAlways]
    [Serializable]
    public class CurvedUIElement : BaseMeshEffect 
    {
        #region Variables

        #region General Variables

        [Header("General Settings")]
        [SerializeField] bool enableProcessing = true;
        [SerializeField] bool referencesSet = false;
        [SerializeField] bool addCollision = false;
        MeshCollider mCol;
        [SerializeField, Range(0, 20)] int imageResolution = 10;

        [SerializeField, HideInInspector] Graphic imgGraphic;
        [SerializeField] CurvedUIBase curvedUIBase;
        [SerializeField] RectTransform rectTrn;
        [SerializeField] RectTransform sourceRT;
        [SerializeField] RectTransform parentRT;
        [SerializeField, HideInInspector] Image.Type imgType;
        [SerializeField, HideInInspector] Mesh curvedMesh;
        [SerializeField, HideInInspector] RectInfo rectInfo;
        [SerializeField, HideInInspector] RectInfo oldRectInfo;

        #endregion
        #region TMPro variables

        [SerializeField, HideInInspector] CanvasRenderer cRend;
        [SerializeField, HideInInspector] TextMeshProUGUI textObj;

        #endregion
        #region Debug 

        [SerializeField] bool logDebugInfo = false;
        [SerializeField, HideInInspector] List<UIVertex> debugVertices = new List<UIVertex>();
        
        #if UNITY_EDITOR
        [SerializeField] bool drawDebug = false;
        #endif

        #endregion

        #endregion


        #region Properties

        /// <summary>
        /// CurvedUIBase "parent" of this instance.
        /// </summary>
        public CurvedUIBase CurvedUIBase {
            get { return curvedUIBase; }
            set { curvedUIBase = value; } }

        /// <summary>
        /// <typeparamref name="Graphic"/> to be curved (<typeparamref name="Image"/> or <typeparamref name="TextMeshProUgui"/>).
        /// </summary>
        public Graphic ImgGraphic {
            get { return imgGraphic; } }
        
        /// <summary>
        /// Curved mesh.
        /// </summary>
        public Mesh CurvedMesh {
            get { return curvedMesh; } }
        
        /// <summary>
        /// <typeparamref name="RectTransform"/> of the source object.
        /// </summary>
        public RectTransform SourceRT {
            get { return sourceRT; } 
            set { sourceRT = value; } }
        
        /// <summary>
        /// <typeparamref name="RectTransform"/> of the parent object.
        /// </summary>
        public RectTransform ParentRT {
            get { return parentRT; }
            set { parentRT = value; } }
        
        /// <summary>
        /// TRUE if references have been set.
        /// </summary>
        public bool ReferencesSet {
            get { return referencesSet; }
            set { referencesSet = value; } }
        
        /// <summary>
        /// TRUE if <typeparamref name="Graphic"/> is <typeparamref name="Image"/>.
        /// </summary>
        protected bool GraphicIsImage {
            get { return imgGraphic.GetType() == typeof(Image); } }
          
        /// <summary>
        /// TRUE if <typeparamref name="Graphic"/> is <typeparamref name="TextMeshProUGUI"/>.
        /// </summary>
        protected bool GraphicIsText {
            get { return imgGraphic.GetType() == typeof(TextMeshProUGUI); } }
        
        /// <summary>
        /// Minimum angles (corner). 
        /// </summary>
        public Vector2 AngleMin {
            get;
            protected set; }
        
        /// <summary>
        /// Maximum angles (corner).
        /// </summary>
        public Vector2 AngleMax {
            get;
            protected set; }

        #endregion


        #region  Events

        /// <summary>
        /// Called each time <paramref name="CurvedMesh"/> is updated.
        /// </summary>
        public Action<Mesh> CurvedMeshUpdated;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        protected override void Awake ()
        {
            base.Awake();

            TryGetReferences();
        }

        protected override void OnEnable ()
        {
            base.OnEnable();

            #if UNITY_EDITOR
            if (logDebugInfo) { Debug.Log("Enabled " + name); }
            #endif
    
            rectInfo.SetValues(rectTrn);
            oldRectInfo.ResetValues();

            TryRefreshGraphic();
        }

        protected override void OnDisable ()
        {
            #if UNITY_EDITOR
            if (logDebugInfo) { Debug.Log("Disabled " + name); }
            #endif

            /// Send message to ParentCO about this gameObject being disabled.
            /// ParentCO then ensures every sibling gets information about disabling even when the UI uses Layout Groups.
            /// (Layout Groups prevent reading changes in RectTransform's position data.)
            if (null != parentRT) { parentRT.GetComponent<CurvedUIElement>().OnChildDisabled(this); }
            ReferencesSet = false;
        }

        private void Update ()
        {
            if (!ReferencesSet) 
            { 
                TryGetReferences(); 
            }

            rectInfo.SetValues(rectTrn);

            if (IsRefreshRequired()) 
            { 
                TryRefreshGraphic(); 
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos ()
        {
            if (!enableProcessing || !drawDebug) { return; }

            Vector3 offset = transform.position - (rectInfo.rtPosition / 1000);

            if (imgGraphic.GetType() == typeof(TextMeshProUGUI)) 
            {
                for (int i = 1; i < textObj.mesh.vertexCount; i++) 
                {
                    Vector3 pA = textObj.mesh.vertices[i - 1] / 1000 + offset;
                    Vector3 pB = textObj.mesh.vertices[i] / 1000 + offset;

                    Gizmos.color = new Color(0.25f, 0.25f, 1f);
                    Gizmos.DrawLine(pA, pB);
                }
            } 
            else if (null != debugVertices) 
            {
                if (debugVertices.Count >= 2) 
                {
                    for (int i = 1; i < debugVertices.Count; i++) 
                    {
                        Vector3 vA = debugVertices[i - 1].position / 1000 + offset;
                        Vector3 vB = debugVertices[i + 0].position / 1000 + offset;
                        Gizmos.color = Color.black;
                        Gizmos.DrawLine(vA, vB);
                    }
                }
            }

            if (null != curvedMesh) 
            {
                Gizmos.color = Color.red;
                for (int i = 1; i < curvedMesh.vertices.Length; i++) 
                {
                    Gizmos.DrawLine(curvedMesh.vertices[i - 1] / 1000 + offset, curvedMesh.vertices[i] / 1000 + offset);
                }
            }
        }
        #endif

        #endregion
        #region Setup and Updating Methods

        /// <summary>
        /// Attempt to get imgGraphic.
        /// </summary>
        /// <returns>True if Graphic was found.</returns>
        bool TryGetGraphic ()
        {
            bool gotGraphic = TryGetComponent(out imgGraphic);

            #if UNITY_EDITOR
            if (!gotGraphic) { Debug.LogError("CurvedUIElement.TryGetGraphic(): Graphic is not set! (" + transform.name + ")"); }
            #endif

            return gotGraphic;
        }

        /// <summary>
        /// Attempt to get references for required components.
        /// </summary> 
        /// <returns>True if references were found succesfully.</returns>
        public bool TryGetReferences ()
        {
            if (!TryGetGraphic()) { return false; }

            if (addCollision)
            {
                if (!transform.TryGetComponent(out mCol)) 
                {
                    mCol = gameObject.AddComponent<MeshCollider>();
                }
            }
            else
            {
                if (transform.TryGetComponent(out mCol))
                {
                    if (mCol.sharedMesh == null)
                    {
                        Debug.Log("Should destroy "+mCol.name+"?");
                        //Destroy(mCol);
                    }
                }
            }
            
            if (GraphicIsText) 
            {
                if (null == textObj) 
                {
                    textObj = imgGraphic as TextMeshProUGUI;

                    #if UNITY_EDITOR
                    if (logDebugInfo) { Debug.Log("CurvedUIElement.TryGetReferences(): TextObj was NULL, got textObj from imgGraphic!"); }
                    #endif
                }
                if (null == textObj.textInfo) 
                {
                    #if UNITY_EDITOR
                    if (logDebugInfo) { Debug.Log("CurvedUIElement.TryGetReferences(): Couldn't get textInfo!"); }
                    #endif
                    return false;
                } 
                else if (null == textObj.textInfo.meshInfo[0].mesh) 
                {
                    #if UNITY_EDITOR
                    if (logDebugInfo) { Debug.Log("CurvedUIElement.TryGetReferences(): MeshInfo[0] was NULL!"); }
                    #endif
                    return false;
                } 
                else if (textObj.textInfo.meshInfo[0].mesh.vertexCount == 0) 
                {
                    #if UNITY_EDITOR
                    if (logDebugInfo) { Debug.Log("CurvedUIElement.TryGetReferences(): MeshInfo[0].vertexCount = 0!"); }
                    #endif
                    return false;
                }
            }

            GetSourceAndParentCO();

            CurvedUIBase = CurvedUIBase.FindBaseFromHierarchy(transform);

            if (null == rectTrn) { rectTrn = (RectTransform)transform; }
            if (null == cRend)   { cRend = GetComponent<CanvasRenderer>(); }

            bool returnValue =
                null != curvedUIBase &&
                null != rectTrn &&
                null != cRend &&
                null != sourceRT &&
                null != parentRT;

            #if UNITY_EDITOR
            if (logDebugInfo) { Debug.Log("CurvedUIElement.TryGetReferences(): Return value = " + returnValue); }
            #endif

            ReferencesSet = returnValue;

            return  returnValue;
        }

        /// <summary>
        /// Get references to CurvedObjects that are either topmost in hierarchy or immediately above this instance.
        /// Subscribe to latter's ForceRefreshChildren Action.
        /// </summary>
        protected void GetSourceAndParentCO ()
        {
            #if UNITY_EDITOR
            if (logDebugInfo) { Debug.Log("Searching for sourceCO and/or parentCO."); }
            #endif

            CurvedUIElement parentCUI = null;
            CurvedUIElement sourceCUI = null;

            bool findParent = parentCUI == null;
            bool findSource = sourceCUI == null;

            if (null != transform.parent && (findParent || findSource)) {

                Transform t = transform.parent;
                int safefail = 100;

                while (null != t && safefail > 0) {

                    safefail--;

                    if (t.TryGetComponent(out CurvedUIElement CO)) {

                        if (findParent && parentCUI == null) {

                            parentCUI = CO;

                            #if UNITY_EDITOR
                            if (logDebugInfo) { Debug.Log("parentCO set to " + parentCUI.name); }
                            #endif
                        }

                        if (findSource) {

                            sourceCUI = CO;

                            #if UNITY_EDITOR
                            if (logDebugInfo) { Debug.Log("sourceCO set to " + sourceCUI.name); }
                            #endif
                        }
                    }

                    t = t.parent;
                }

                if (null == sourceCUI) { sourceCUI = this; }
                if (null == parentCUI) { parentCUI = this; }

                sourceRT = (RectTransform)sourceCUI.transform;
                parentRT = (RectTransform)parentCUI.transform;

                #if UNITY_EDITOR
                if (safefail == 0) { Debug.LogError("CurvedUIElement.GetSourceAndParentCO(): Safefail failed!"); }
                #endif
            }
        }

        /// <summary>
        /// Receive message when a child transform with CurvedObject is disabled.
        /// Relay the message to said transform's siblings in order to update their positions.
        /// (Layout Groups prevent reading changes in RectTransform's position data.)
        /// </summary>
        /// <param name="_childCO"></param>
        public void OnChildDisabled (CurvedUIElement _childCO)
        {
            foreach (Transform child in transform) 
            {
                if (_childCO.transform == child) { continue; }

                _childCO.TryRefreshGraphic();
            }
        }

        public void ResetReferencesOfChildren ()
        {
            TryGetReferences();

            CurvedUIElement[] childElements = GetComponentsInChildren<CurvedUIElement>(true);

            foreach (CurvedUIElement c in childElements) 
            {
                c.TryGetReferences();
            }
        }

        #endregion
        #region Graphic Refresh Methods

        /// <summary>
        /// Update graphics mesh.
        /// </summary>
        /// <returns>True if refreshing of the mesh is needed.</returns>
        public bool IsRefreshRequired ()
        {
            if (!enableProcessing) { return false; }

            if (!referencesSet)
            {
                #if UNITY_EDITOR
                if (logDebugInfo) { Debug.Log(transform.name + ": RectInfo not equal."); }
                #endif
                return false; 
            }

            if (!oldRectInfo.ValuesAreEqualTo(rectInfo)) 
            {
                #if UNITY_EDITOR
                if (logDebugInfo) { Debug.Log(transform.name + ": RectInfo not equal."); }
                #endif
                return true; 
            }

            if (GraphicIsText && textObj.textInfo.textComponent.havePropertiesChanged) 
            {
                #if UNITY_EDITOR
                if (logDebugInfo) { Debug.Log("TextContent changed."); }
                #endif
                return true; 
            }
 
            return false;
        }

        /// <summary>
        /// Update imgGraphic to curve by recalculating the vertices of the mesh.
        /// </summary>
        protected void TryRefreshGraphic ()
        {
            if (!ReferencesSet) 
            { 
                if (!TryGetReferences()) { return; } 
            }

            /// Update is needed if the execution reaches this point.
            if (GraphicIsText) 
            {
                ModifyTextMesh();
            } 
            else if (GraphicIsImage) 
            {
                imgGraphic.SetAllDirty();
            } 
            
            #if UNITY_EDITOR
            else 
            {
                Debug.Log("Handling of imgGraphic's type (" + imgGraphic.GetType().ToString() + ") has not been implemented yet.");
            }

            if (logDebugInfo) 
            {
                if (null == CurvedMesh) { Debug.Log("No Curved Mesh"); } 
                else                    { Debug.Log("Has Curved Mesh"); }
            }
            #endif
        }

        #endregion
        #region Text Mesh Modification Methods
       
        /// <summary>
        /// Recalculate the mesh of TMPro's textInfo.meshInfo[0].
        /// NOTE: In case of special characters not found on the default material ([0]), 
        /// additional code might be required to handle submeshes.
        /// </summary>
        void ModifyTextMesh ()
        {
            #if UNITY_EDITOR
            if (logDebugInfo) { Debug.Log("ModifyTextMesh..."); }
            #endif

            textObj.ForceMeshUpdate();
            textObj.textInfo.meshInfo[0].mesh = ChangeTMProVertexPositions();

            cRend.SetMesh(textObj.textInfo.meshInfo[0].mesh);

            oldRectInfo.SetValues(rectInfo);

            curvedMesh = MeshUtility.CopyMesh(textObj.textInfo.meshInfo[0].mesh);

            if (Application.isPlaying) 
            { 
                CurvedMeshUpdated?.Invoke(curvedMesh); 

                if (addCollision && null != mCol)
                {
                    mCol.sharedMesh = curvedMesh;
                }
            } 
        }

        /// <summary>
        /// Create a new mesh and set its vertices to match the curve.
        /// </summary>
        /// <param name="_createCopy">Does the method modify and return the original instance or a copy of it?</param>
        /// <returns>Mesh of textInfo.meshInfo[0] with vertex positions projected to the curve.</returns>
        Mesh ChangeTMProVertexPositions (bool _createCopy = false)
        {
            Mesh m = _createCopy
                ? MeshUtility.CopyMesh(textObj.textInfo.meshInfo[0].mesh)
                : textObj.textInfo.meshInfo[0].mesh;

            Vector3[] verts = new Vector3[m.vertices.Length];

            for (int i = 0; i < verts.Length; i++) {

                Vector3 flatWorldPosition = transform.TransformPoint(m.vertices[i]);
                Vector3 worldCenter = sourceRT.TransformPoint(sourceRT.rect.center);
                Vector3 curvedWorldPosition = curvedUIBase.ProjectPointToCurvedPlane(flatWorldPosition, worldCenter);
                Vector3 localPosition = transform.InverseTransformPoint(curvedWorldPosition);
                verts[i] = localPosition;

                if (logDebugInfo) {

                    Debug.DrawLine(flatWorldPosition, worldCenter, Color.red);
                    Debug.DrawLine(curvedWorldPosition, worldCenter, Color.green);
                }
            }

            m.vertices = verts;

            return m;
        }

        #endregion
        #region Image Mesh Modification

        /// <summary>
        /// Modify BaseMeshEffect-inherited UI mesh that is built with VertexHelper.
        /// </summary>
        /// <param name="vh"></param>
        public override void ModifyMesh (VertexHelper vh)
        {
            if (!enableProcessing)  { return; }
            if (!referencesSet)     { return; }

            List<UIVertex> uiVerts = new List<UIVertex>();

            vh.GetUIVertexStream(uiVerts);
            vh.Clear();
            SubdivideMesh(uiVerts);
            ChangeVertexPositions(uiVerts);
            vh.AddUIVertexTriangleStream(uiVerts);

            if (null == curvedMesh) curvedMesh = new Mesh();
            vh.FillMesh(curvedMesh);

            cRend.SetMesh(curvedMesh);

            if (Application.isPlaying) 
            { 
                CurvedMeshUpdated?.Invoke(curvedMesh); 

                if (addCollision && null != mCol)
                {
                    mCol.sharedMesh = curvedMesh;
                }       
            }

            oldRectInfo.SetValues(rectInfo);
        }
        
        /// <summary>
        /// Subdivide the graphic mesh to enable smooth curving.
        /// </summary>
        /// <param name="_verts"></param>
        void SubdivideMesh (List<UIVertex> _verts)
        {
            Image img = imgGraphic as Image;
            imgType = img.type;

            /// The original graphic has the following amount of vertices:
            /// * Simple: 6
            /// * Sliced: 54
            /// * Tiled: MANY (not usable at this point)
            /// * Filled: 6
            /// 
            /// Currently the curve is only affecting the z-position on x-axis. Because of that we only need to divide the meshes if they are horizontally aligned.
            /// Wwe can skip quads with width less than a treshold value.
            if (imgType == Image.Type.Tiled) {
                #if UNITY_EDITOR
                Debug.Log("Image.Type of " + imgGraphic.transform.name + " is Tiled, which doesn't need to be subdivided.");
                #endif
                return;
            }

            #if UNITY_EDITOR
            if (null == curvedUIBase) { Debug.Log(transform.name + " has NULL Grid!"); }
            #endif

            /// Get start parameters for mesh calculation.
            float pixelTreshold = 1f / curvedUIBase.CanvasScale / imageResolution;
            List<Quad> originalQuads = new List<Quad>();
            List<Quad> newQuads = new List<Quad>();
            List<Quad> finalQuads = new List<Quad>();
            int xVertCount = 0;

            for (int i = 0; i < _verts.Count; i += 6) {

                Quad quad = new Quad();
                quad.verts = new UIVertex[4];
                quad.tris = new int[6];

                quad.verts[0] = _verts[i + 0];
                quad.verts[1] = _verts[i + 1];
                quad.verts[2] = _verts[i + 2];
                quad.verts[3] = _verts[i + 4];

                quad.tris[0] = 0;
                quad.tris[1] = 1;
                quad.tris[2] = 2;
                quad.tris[3] = 2;
                quad.tris[4] = 4;
                quad.tris[5] = 0;

                originalQuads.Add(quad);

                /// Only look at x-axis. If the required count for this resolution is more than one, subdivision is required.
                int xDivisions = quad.QuadCount(pixelTreshold).x;

                if (xDivisions > 1) {

                    float increment = 1f / xDivisions;

                    for (int x = 0; x < xDivisions; x++) {

                        float ratioA = (x + 0) * increment;
                        float ratioB = (x + 1) * increment;

                        Quad nQuad = new Quad();
                        nQuad.verts = new UIVertex[4];
                        nQuad.tris = new int[6];

                        nQuad.verts[0] = GenerateUIVertexForQuad(quad, nQuad, ratioA, 0, 0);
                        nQuad.verts[1] = GenerateUIVertexForQuad(quad, nQuad, ratioA, 1, 1);
                        nQuad.verts[2] = GenerateUIVertexForQuad(quad, nQuad, ratioB, 1, 1);
                        nQuad.verts[3] = GenerateUIVertexForQuad(quad, nQuad, ratioB, 0, 0);

                        nQuad.tris[0] = 0;
                        nQuad.tris[1] = 1;
                        nQuad.tris[2] = 2;
                        nQuad.tris[3] = 2;
                        nQuad.tris[4] = 3;
                        nQuad.tris[5] = 0;

                        newQuads.Add(nQuad);
                        finalQuads.Add(nQuad);
                    }

                    xVertCount += xDivisions;

                } else {

                    xVertCount++;
                    finalQuads.Add(quad);
                }
            }

            /// Quads have been subdivided. Next step is to turn them back into UIVertices.
            List<UIVertex> finalVerts = new List<UIVertex>();

            for (int i = 0; i < finalQuads.Count; i++) {

                Quad q = finalQuads[i];

                finalVerts.Add(q.verts[0]);
                finalVerts.Add(q.verts[1]);
                finalVerts.Add(q.verts[2]);
                finalVerts.Add(q.verts[2]);
                finalVerts.Add(q.verts[3]);
                finalVerts.Add(q.verts[0]);
            }

            _verts.Clear();
            _verts.AddRange(finalVerts);

            debugVertices.Clear();

            for (int i = 0; i < _verts.Count; i++) {

                debugVertices.Add(_verts[i]);
            }
        }

        /// <summary>
        /// Generate a new UIVertex to fill subdivided mesh with.
        /// </summary>
        /// <param name="_originalQuad"></param>
        /// <param name="_newQuad"></param>
        /// <param name="_ratio"></param>
        /// <param name="_xTop"></param>
        /// <param name="_yTop"></param>
        /// <returns></returns>
        UIVertex GenerateUIVertexForQuad (Quad _originalQuad, Quad _newQuad, float _ratio, int _xTop, int _yTop)
        {
            UIVertex v = new UIVertex();

            v.position = Vector3.Lerp(_originalQuad.verts[_xTop].position, _originalQuad.verts[3 - _yTop].position, _ratio);
            v.normal = Vector3.Lerp(_originalQuad.verts[_xTop].normal, _originalQuad.verts[3 - _yTop].normal, _ratio);
            v.tangent = Vector3.Lerp(_originalQuad.verts[_xTop].tangent, _originalQuad.verts[3 - _yTop].tangent, _ratio);
            v.uv0 = Vector2.Lerp(_originalQuad.verts[_xTop].uv0, _originalQuad.verts[3 - _yTop].uv0, _ratio);
            v.color = Color.Lerp(_originalQuad.verts[_xTop].color, _originalQuad.verts[3 - _yTop].color, _ratio);

            return v;
        }

        /// <summary>
        /// Apply the curvature to the mesh.
        /// </summary>
        /// <param name="_verts"></param>
        void ChangeVertexPositions (List<UIVertex> _verts)
        {
            for (int i = 0; i < _verts.Count; i++) {

                UIVertex v = _verts[i];

                bool log = (logDebugInfo && i == 0);

                Vector3 flatWorldPosition = transform.TransformPoint(v.position);
                Vector3 worldObjCenter = rectTrn.TransformPoint(rectTrn.rect.center);
                Vector3 worldCenter = sourceRT.TransformPoint(sourceRT.rect.center);
                Vector3 curvedWorldPosition = curvedUIBase.ProjectPointToCurvedPlane(flatWorldPosition, worldCenter);

                v.normal = curvedUIBase.GetNormalOnCurvedPlane(transform.TransformDirection(v.normal));
                v.position = transform.InverseTransformPoint(curvedWorldPosition);

                _verts[i] = v;
                
                #if UNITY_EDITOR
                if (log) {

                    Debug.Log("vPos: " + v.position 
                        + "  |  flatWorldPos: " + flatWorldPosition 
                        + "  |  curveLocalFlatWorldPos: " + curvedUIBase.transform.InverseTransformPoint(flatWorldPosition)
                        + "  |  curveLocalFlatWorldPosV2: " + curvedUIBase.transform.InverseTransformPoint((Vector2)flatWorldPosition));

                    Debug.DrawLine(flatWorldPosition, worldCenter, Color.red);
                    Debug.DrawLine(curvedWorldPosition, worldCenter, Color.green);
                }
                #endif
            }
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace