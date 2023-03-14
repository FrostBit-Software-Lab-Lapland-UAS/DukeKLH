/******************************************************************************
 * File        : CurvedUIBase.cs
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


namespace DUKE.UI {


    /// <summary>
    /// Generate a curved plane that works as a base for curved UI elements.
    /// </summary>
    [ExecuteAlways]
    public class CurvedUIBase : MonoBehaviour 
    {
        #region Variables

        #region General Variables

        [Space(10f)]
        [SerializeField] Vector2Int gridResolution;
        [SerializeField] float canvasScale = 0.001f;  /// Default scale of the canvas objects attached to this object. Used for calculating the mesh resolution of UI elements.
        [SerializeField] float height;
        [SerializeField] float curveRadius;
        [SerializeField] float chordAngle;
        Vector3[,] curvedPoints;

        #endregion
        #region Debug Variables
        
        #if UNITY_EDITOR
        [Space(10f), SerializeField] bool drawGrid = true;
        #endif
        
        [SerializeField] Color gridColor;
        [SerializeField] Transform testPointTransform;

        #if UNITY_EDITOR
        [SerializeField] bool drawDebug = true;
        #endif

        #endregion
        
        #endregion


        #region Properties

        /// <summary>
        /// Local coordinate position of the origin (center) point.
        /// </summary>
        public Vector3 OriginPoint { 
            get { return Vector3.forward * -curveRadius; } }

        /// <summary>
        /// World coordinate position of the origin (center) point.
        /// </summary>
        public Vector3 OriginPointGlobal { 
            get { return transform.TransformPoint(OriginPoint); } }

        /// <summary>
        /// Current canvas scale.
        /// </summary>
        public float CanvasScale {
            get { return canvasScale; } }

        /// <summary>
        /// Radius of the curve. 
        /// </summary>
        public float Radius {
            get { return curveRadius; } }

        /// <summary>
        /// Circumfence of the curve if it was 360 degrees wide.
        /// </summary>
        public float Circumfence { 
            get { return 2 * Mathf.PI * curveRadius; } }

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        #if UNITY_EDITOR
        private void OnDrawGizmos ()
        {
            if (!drawDebug && !drawGrid) return;
            if (gridResolution.x <= 0 || gridResolution.y <= 0) return;

            /// Render the grid visuals.
            if (drawGrid) 
            {
                curvedPoints = CreateCurvedGrid();

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(OriginPointGlobal, 0.25f);
                Gizmos.color = gridColor;

                for (int x = 0; x <= gridResolution.x; x++) 
                {
                    for (int y = 0; y <= gridResolution.y; y++) 
                    {
                        if (x > 0 && y > 0) 
                        {
                            Vector3 pointA = transform.position + transform.TransformDirection(curvedPoints[x, y]);
                            Vector3 pointB = transform.position + transform.TransformDirection(curvedPoints[x - 1, y]);
                            Vector3 pointC = transform.position + transform.TransformDirection(curvedPoints[x, y - 1]);
                            Vector3 pointD = transform.position + transform.TransformDirection(curvedPoints[x - 1, y - 1]);

                            Gizmos.DrawLine(pointA, pointB);
                            Gizmos.DrawLine(pointA, pointC);
                            Gizmos.DrawLine(pointB, pointD);
                            Gizmos.DrawLine(pointC, pointD);
                        }
                    }
                }
            }

            /// Render debug visuals.
            if (drawDebug && null != testPointTransform) 
            {
                Vector3 directionToTransform = (OriginPointGlobal - testPointTransform.position).normalized/*)*/ * -1;
                Vector3 wPos = testPointTransform.position;

                /// Line from TestObject to away from OriginPoint:
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(OriginPointGlobal, OriginPointGlobal + directionToTransform * 10f);

                /// Orange sphere (old method):
                Gizmos.color = new Color(1f, 0.5f, 0.25f);
                Gizmos.DrawWireSphere(GetPositionOnCurvedPlane(wPos, curveRadius), 0.125f);

                /// Line from OriginPoint to grid surface:
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(OriginPointGlobal, wPos);

                /// TestObject box:
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(wPos, Vector3.one * 0.25f);

                /// Purple box:
                Gizmos.color = new Color(1f, 0f, 1f);
                Gizmos.DrawWireCube(GetPositionOnCurvedPlane(wPos, curveRadius), Vector3.one * 0.25f);

                /// Yellowish-green box:
                Gizmos.color = new Color(0.5f, 1f, 0f);
                Gizmos.DrawWireCube(GetPositionOnCurvedPlane(wPos, curveRadius, true, false), Vector3.one * 0.25f);

                /// Purple normal line:
                Gizmos.color = new Color(0.5f, 0.25f, 1f);
                Vector3 constrainedWPos = GetPositionOnCurvedPlane(wPos, curveRadius);
                Gizmos.DrawLine(constrainedWPos, constrainedWPos + GetNormalOnCurvedPlane(constrainedWPos));

                /// Black normal line:
                Gizmos.color = Color.black;
                Gizmos.DrawLine(wPos, wPos + GetNormalOnCurvedPlane(wPos));
            }
        }
        #endif

        #endregion
        #region Location and Constraint Methods
        
        /// <summary>
        /// Convert angles to a local coordinate position.
        /// </summary>
        /// <param name="_angles">Angles on X- and Y-axii.</param>
        /// <returns>Local coordinate position.</returns>
        public Vector2 GetLocalPositionFromAngles (Vector2 _angles)
        {
            float x = Circumfence * ((_angles.x > 180f ? _angles.x - 360f : _angles.x) / 360f);
            float y = Mathf.Tan(_angles.y * Mathf.Deg2Rad) * curveRadius;

            return new Vector2(x,y);
        }

        /// <summary>
        /// Project a world position to curve. Position is used as relative to this object's transform, 
        /// which means that positional Z is used to modify the radius of the calculation while X and Y control the position on the curved plane. 
        /// This method is used by CurvedUIElements to calculate their shape.
        /// </summary>
        /// <param name="_wPointPos">World position of the vertex (usually mesh.vertices[x]).</param>
        /// <param name="_wCenterPos">Center position of the source CurvedUIElement. Source is the topmost CurvedUIElement of the hierarchy.</param>
        /// <returns>World position of the vertex projected to the curve.</returns>
        public Vector3 ProjectPointToCurvedPlane (Vector3 _flatWorldPos, Vector3 _wCenterPos)
        {
            float trueRadius = curveRadius + transform.InverseTransformPoint(_flatWorldPos).z;
            Vector2 angles = GetAnglesOfFlatPoint(_flatWorldPos, _wCenterPos);
            Vector2 coordinates = GetCoordinatesFromAngles(angles, false);

            return GetPositionOnCurvedPlaneFromCoordinates(coordinates, trueRadius, true);
        }

        /// <summary>
        /// Project a world position to curve. Position is used as relative to this object's transform, 
        /// which means that positional Z is used to modify the radius of the calculation while X and Y control the position on the curved plane. 
        /// This method is used by CurvedUIElements to calculate their shape.
        /// </summary>
        /// <param name="_flatWorldPos">World position of the vertex (usually mesh.vertices[x]).</param>
        /// <param name="_wCenterPos">Center position of the source CurvedUIElement. Source is the topmost CurvedUIElement of the hierarchy.</param>
        /// <returns>World position of the vertex projected to the curve.</returns>
        public Vector3 ProjectPointToCurvedPlane (Vector3 _flatWorldPos, Vector3 _wObjCenterPos, Vector3 _wCenterPos, bool _logInfo = false)
        {
            /// Convert coordinates to CurvedUIBase's local coordinate system:
            Vector3 localPoint = transform.InverseTransformPoint(_flatWorldPos);
            Vector3 localObjCenter = transform.InverseTransformPoint(_wObjCenterPos);
            Vector3 localSourceCenter = transform.InverseTransformPoint(_wCenterPos);

            /// Each instance of curveRadius was replaced with cRadius.
            float cRadius = curveRadius;
            float trueRadius = cRadius + localPoint.z;
            float angleDivider = trueRadius / cRadius;

            float baseCircumfence = 2 * Mathf.PI * cRadius;
            float centerBaseAngle = 360f * (localSourceCenter.x / baseCircumfence);
            float pointBaseAngle = 360f * (localPoint.x / baseCircumfence);
            float baseAngleFromCenter = (pointBaseAngle - centerBaseAngle);
            float pointAdjustedAngleFromCenter = baseAngleFromCenter / angleDivider;

            float xAngle = centerBaseAngle + pointAdjustedAngleFromCenter;

            #if UNITY_EDITOR
            if (_logInfo) {

                Debug.Log(
                    "angle: " + xAngle
                    + "  |  pAngleAdjusted: " + pointAdjustedAngleFromCenter
                    + "  |  sourceCenterAngle: " + centerBaseAngle
                    + "  |  pBaseAngle: " + pointBaseAngle
                    + "  |  angDiv: " + angleDivider
                    + "  |  trueRad: " + trueRadius
            ); }
            #endif

            float yAngle = Mathf.Atan2(localPoint.y, curveRadius) * Mathf.Rad2Deg;
            Vector2 angles = new Vector2 (xAngle, yAngle);
            Vector2 coordinates = GetCoordinatesFromAngles(angles, false);

            return GetPositionOnCurvedPlaneFromCoordinates(coordinates, trueRadius, true);
        }

        /// <summary>
        /// Calculate the angles of a position.
        /// </summary>
        /// <param name="_flatWorldPos">World coordinate position of a point.</param>
        /// <param name="_wCenterPos">World coordinate position of the centerpoint of the curve (UI center position, NOT the middle of the ring).</param>
        /// <returns>Angles (X,Y) of a position in relation to the center position of the curve.</returns>
        public Vector2 GetAnglesOfFlatPoint (Vector3 _flatWorldPos, Vector3 _wCenterPos)
        {
            /// Convert coordinates to CurvedUIBase's local coordinate system:
            Vector3 localPoint = transform.InverseTransformPoint(_flatWorldPos);
            Vector3 localSourceCenter = transform.InverseTransformPoint(_wCenterPos);

            float baseCircumfence = 2 * Mathf.PI * curveRadius;
            float centerBaseAngle = 360f * (localSourceCenter.x / baseCircumfence);
            float pointBaseAngle = 360f * (localPoint.x / baseCircumfence);
            float trueRadius = curveRadius + localPoint.z;
            float angleDivider = trueRadius / curveRadius;
            float pointAdjustedAngleFromCenter = (pointBaseAngle - centerBaseAngle) / angleDivider;
            float xAngle = centerBaseAngle + pointAdjustedAngleFromCenter;
            float yAngle = Mathf.Atan2(localPoint.y, curveRadius) * Mathf.Rad2Deg;

            Vector2 angles = new Vector2(xAngle, yAngle);

            return angles;
        }

        /// <summary>
        /// Calculate the angles of a point in world coordinates. 
        /// This method can be used to get accurate coordinates of a point on the curve.
        /// </summary>
        /// <param name="_worldPos">World coordinate position.</param>
        /// <param name="_shiftOutOfNegativeRange">Add 360 degrees to an angle if it is below 0.</param>
        /// <returns>Angles (X,Y) of a position in relation to the center position of the curve.</returns>
        public Vector2 GetAnglesOfWorldPoint (Vector3 _worldPos, bool _shiftOutOfNegativeRange = true)
        {
            Vector3 localPoint = transform.InverseTransformPoint(_worldPos);
            Vector3 leveledLocalPoint = new Vector3(localPoint.x, 0f, localPoint.z);

            float xAngle = Vector3.SignedAngle(
                Vector3.forward, 
                leveledLocalPoint - OriginPoint, 
                Vector3.up);
                  
            float yAngle = Mathf.Atan2(localPoint.y, curveRadius) * Mathf.Rad2Deg;

            if (_shiftOutOfNegativeRange && xAngle < 0f) { xAngle += 360f; }

            return new Vector2(xAngle, yAngle);
        }
        
        /// <summary>
        /// Project a world position to curve.
        /// </summary>
        /// <param name="_worldPosition">World coordinate point to be curved.</param>
        /// <param name="_radius">Radius of the curve.</param>
        /// <param name="_constrainToGrid">Whether to clamp the point to within the limits defined in <typeparamref name="CurvedUIBase"/>.</param>
        /// <param name="_adjustY"></param>
        Vector3 GetPositionOnCurvedPlane (Vector3 _worldPosition, float _radius, bool _constrainToGrid = true, bool _adjustY = true)
        {
            Vector3 localPosition = transform.InverseTransformPoint(_worldPosition);
            Vector3 direction = (localPosition - OriginPoint).normalized;
            Vector3 leveledDirection = new Vector3(direction.x, 0f, direction.z).normalized;
            float totalLeveledDistance = Vector3.Distance(OriginPoint, new Vector3(localPosition.x, OriginPoint.y, localPosition.z));
            float leveledDistanceRatio = curveRadius / totalLeveledDistance;
            float heightDifference = (localPosition.y - OriginPoint.y) * (_adjustY ? 1f : leveledDistanceRatio);

            if (_constrainToGrid) {
                float xAngle = Mathf.Clamp(Vector3.SignedAngle(Vector3.forward, leveledDirection, Vector3.up), -chordAngle / 2f, chordAngle / 2f);
                leveledDirection = Quaternion.AngleAxis(xAngle, Vector3.up) * Vector3.forward;
                heightDifference = Mathf.Clamp(heightDifference, -height / 2f, height / 2f);
            }

            Vector3 leveledConstrainedPosition = OriginPoint + leveledDirection * _radius;
            Vector3 fullConstrainedPosition = leveledConstrainedPosition + Vector3.up * heightDifference;

            return transform.TransformPoint(fullConstrainedPosition);
        }
        
        /// <summary>
        /// Find the coordinates of the provided percentage values within the grid. (0,0) is the left bottom corner and (1,1) is the right top corner.
        /// </summary>
        /// <param name="_coordinates01"></param>
        public Vector3 GetPositionOnCurvedPlaneFromCoordinates (Vector2 _coordinates01, float _radius, bool _worldCoordinates = true)
        {
            float anglePercent = (-chordAngle / 2f) + chordAngle * _coordinates01.x;
            float heightPercent = (-height / 2f) + height * _coordinates01.y;
            Vector3 point = OriginPoint + Quaternion.AngleAxis(anglePercent, Vector3.up) * (Vector3.forward * _radius) + Vector3.up * heightPercent;

            if (_worldCoordinates) return transform.TransformPoint(point);
            else return point;
        }
        
        /// <summary>
        /// Calculate the normal direction on the curve. Default direction is from the plane towards center, ignoring Y.
        /// The position provided as parameter does not have to be constrained on the curve in order to return the normal direction. 
        /// </summary>
        /// <param name="_worldPosition"></param>
        /// <returns></returns>
        public Vector3 GetNormalOnCurvedPlane (Vector3 _worldPosition)
        {
            Vector3 localPosition = transform.InverseTransformPoint(_worldPosition);
            Vector3 leveledOrigin = new Vector3(OriginPoint.x, localPosition.y, OriginPoint.z);
            Vector3 normalDirection = -(localPosition - leveledOrigin).normalized;
            return transform.TransformDirection(normalDirection);
        }
        
        /// <summary>
        /// Calculate the 2D coordinates of the position on the grid. 
        /// </summary>
        /// <param name="_angles"></param>
        /// <param name="_clamp"></param>
        /// <returns></returns>
        public Vector2 GetCoordinatesFromAngles (Vector2 _angles, bool _clamp = true)
        {
            float yRad = _angles.y * Mathf.Deg2Rad;
            float xCoord = (chordAngle / 2f + _angles.x) / chordAngle;
            float yCoord = 0.5f + Mathf.Tan(yRad) * curveRadius / height;

            if (_clamp) return new Vector2(Mathf.Clamp(xCoord, 0f, 1f), Mathf.Clamp(yCoord, 0f, 1f));
            else return new Vector2(xCoord, yCoord);
        }

        #endregion
        #region Utility Methods
        
        /// <summary>
        /// Attempt to find a CurvedUIBase from the provided <paramref name="_finder"/> Transform's hierarchy.
        /// </summary>
        /// <param name="_finder"></param>
        /// <returns></returns>
        public static CurvedUIBase FindBaseFromHierarchy (Transform _finder)
        {
            Transform parent = _finder;
            CurvedUIBase cGrid = null;

            while (null != parent) {

                if (parent.TryGetComponent(out cGrid))  { return cGrid; } 
                else                                    { parent = parent.parent; }
            }

            return cGrid;
        }

        #endregion
        #region Debug Methods

        /// <summary>
        /// Create and return a curved grid.
        /// This 2D array is used for rendering the debug visual grid.
        /// </summary>
        /// <param name="_gridResolution"></param>
        /// <param name="_height"></param>
        /// <param name="_curveRadiusFromOrigin"></param>
        /// <param name="_chordAngle"></param>
        /// <returns></returns>
        Vector3[,] CreateCurvedGrid (bool _worldCoordinates = false)
        {
            gridResolution = new Vector2Int(Mathf.Clamp(gridResolution.x, 1, 1024), Mathf.Clamp(gridResolution.y, 1, 1024));
            Vector3[,] grid = new Vector3[gridResolution.x + 1, gridResolution.y + 1];

            for (int x = 0; x <= gridResolution.x; x++) {
                for (int y = 0; y <= gridResolution.y; y++) {

                    Vector2 coords01 = new Vector2((float)x / gridResolution.x, (float)y / gridResolution.y);
                    grid[x, y] = GetPositionOnCurvedPlaneFromCoordinates(coords01, curveRadius, _worldCoordinates);
                }
            }
            return grid;
        }

        /// <summary>
        /// NOTE: This method is currently unused. Delete it at some point.
        /// Create a mesh from the debug visual grid.
        /// </summary>
        /// <param name="_gridResolution"></param>
        /// <param name="_height"></param>
        /// <param name="_curveRadiusFromOrigin"></param>
        /// <param name="_chordAngle"></param>
        /// <returns></returns>
        void CreateCurvedMesh (Vector2Int _gridResolution)
        {
            MeshFilter filter;
            MeshRenderer rend;
            if (!TryGetComponent(out filter)) filter = gameObject.AddComponent<MeshFilter>();
            if (!TryGetComponent(out rend)) rend = gameObject.AddComponent<MeshRenderer>();

            /// Build the mesh. 
            /// This could (and probably should) be done simultaneously with the grid construction, but separating them makes the code easier to read.
            Vector3[] vertices = new Vector3[_gridResolution.x * _gridResolution.y];
            int[] triangles = new int[(_gridResolution.x - 1) * (_gridResolution.y - 1) * 6];
            Vector2[] uvs = new Vector2[vertices.Length];
            int vertIndex = 0;
            int triIndex = 0;

            for (int x = 0; x < _gridResolution.x; x++) {
                for (int y = 0; y < _gridResolution.y; y++) {

                    vertices[vertIndex] = curvedPoints[x, y];
                    uvs[vertIndex] = new Vector2(1f - ((float)x / (_gridResolution.x - 1)), (float)y / (_gridResolution.y - 1));
                    vertIndex++;

                    if (x > 0 && y > 0) {

                        triangles[triIndex + 0] = (x - 1) * _gridResolution.y + (y - 1);
                        triangles[triIndex + 1] = x * _gridResolution.y + (y - 1);
                        triangles[triIndex + 2] = x * _gridResolution.y + y;

                        triangles[triIndex + 3] = x * _gridResolution.y + y;
                        triangles[triIndex + 4] = (x - 1) * _gridResolution.y + y;
                        triangles[triIndex + 5] = (x - 1) * _gridResolution.y + (y - 1);

                        triIndex += 6;
                    }
                }
            }

            Mesh m = new Mesh();
            m.vertices = vertices;
            m.triangles = triangles;
            m.uv = uvs;
            m.name = "CurvedPlane";
            m.RecalculateNormals();
            m.Optimize();

            filter.mesh = m;
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace