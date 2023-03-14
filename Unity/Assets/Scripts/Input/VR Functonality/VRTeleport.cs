/******************************************************************************
 * File        : VRTeleport.cs
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
using UnityEngine;


namespace DUKE.Controls {


    /// <summary>
    /// Allow user to move around in the environment with teleportation.
    /// WARNING: This component has been abandoned and is not used currently in the project.
    /// It does not currently have 
    /// </summary>
    [RequireComponent(typeof(VRInput))]
    public class VRTeleport : MonoBehaviour 
    {
        #region Variables

        VRInput input;
        Transform player;
        Transform arcObj;
        Transform teleportRing;
        MeshFilter mFilter;
        MeshRenderer mRenderer;

        /// <summary>
        /// <typeparamref name="Layers"/> to which teleporting is possible.
        /// </summary>
        public LayerMask teleportableLayers;

        /// <summary>
        /// <typeparamref name="Layers"/> that block the teleportation path.
        /// </summary>
        public LayerMask collisionLayers;

        /// <summary>
        /// <typeparamref name="Material"/> of the arc <typeparamref name="Mesh"/>.
        /// </summary>
        public Material arcMaterial;

        /// <summary>
        /// <typeparamref name="Color"/> of the arc.
        /// </summary>
        public Color arcColor;

        /// <summary>
        /// TRUE if teleportation is allowed.
        /// </summary>
        public bool allowTeleportation = false;

        /// <summary>
        /// Increases the reach of the teleportation.
        /// </summary>
        public float arcVelocity;

        /// <summary>
        /// Maximum arc step count for calculating the arc.
        /// </summary>
        public int maxSteps;

        /// <summary>
        /// Length of a single arc step.
        /// </summary>
        public float stepLength = 0.01f;

        /// <summary>
        /// Vertex count of an arc's cross section.
        /// </summary>
        public int cylinderResolution;

        /// <summary>
        /// Width of the arc at the start point.
        /// </summary>
        public float startWidth;

        /// <summary>
        /// Width of the arc at the end point.
        /// </summary>
        public float endWidth;

        #if UNITY_EDITOR
        [SerializeField] bool drawDebug;
        #endif


        Vector3 smoothedPointingDirection;
        Vector3[] arcPoints;
        Vector3[] vertices;
        
        #endregion


        #region Properties

        /// <summary>
        /// TRUE if the current teleportation spot is valid.
        /// </summary>
        public bool IsValidTeleportSpot { 
            get; 
            private set; }

        /// <summary>
        /// TRUE if the arc collided with an object that belongs to <paramref name="collisionLayers"/>.
        /// </summary>
        public bool LineCollidedWithBlocker { 
            get; 
            private set; }

        /// <summary>
        /// Current teleport position.
        /// </summary>
        public Vector3 TeleportPoint { 
            get { return teleportRing.position; } }

        /// <summary>
        /// Forward direction of the teleportation action at the arc's end point.
        /// </summary>
        public Vector3 TeleportForwardDirection { 
            get { return teleportRing.forward; } }

        #endregion

        #region Methods

        #region MonoBehaviour Methods
        
        void Start ()
        {
            GetReferences();

            if (maxSteps < 3)                   maxSteps = 3;
            if (cylinderResolution % 2 == 0)    cylinderResolution++;

            smoothedPointingDirection = transform.forward;

        }
        
        void Update ()
        {
            if (!allowTeleportation) return;

            smoothedPointingDirection = Vector3.Lerp(smoothedPointingDirection, transform.forward, 0.2f);
            CalculateArc();
            GenerateLineMesh();
        }
        
        #if UNITY_EDITOR
        void OnDrawGizmos ()
        {
            if (!drawDebug)             { return; }
            if (!Application.isPlaying) { return; }

            if (null != arcPoints) 
            {
                if (arcPoints.Length > 0) 
                {
                    for (int i = 1; i < arcPoints.Length; i++) 
                    {
                        Gizmos.color = (i % 2 == 0 ? Color.black : Color.white);
                        Gizmos.DrawLine(arcPoints[i - 1], arcPoints[i]);
                    }
                } 
            }

            if (null != vertices) 
            {
                for (int i = 0; i < vertices.Length; i++) 
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(vertices[i], 0.01f);
                }
            } 
        }
        #endif

        #endregion
        #region Arc Calculation and Visual Representation Methods 

        /// <summary>
        /// Get the required references.
        /// </summary>
        void GetReferences ()
        {
            player = GameObject.Find("Player (VR)").transform;
            input = GetComponent<VRInput>();
        }
        
        /// <summary>
        /// Create a container object for the arc.
        /// </summary>
        void CreateArcMeshObject ()
        {
            arcObj = new GameObject("Teleport Arc").transform;
            teleportRing = Instantiate(Resources.Load("Prefabs/TeleportRing") as GameObject).transform;
            teleportRing.parent = arcObj;
            mFilter = arcObj.gameObject.AddComponent<MeshFilter>();
            mRenderer = arcObj.gameObject.AddComponent<MeshRenderer>();
            mRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        
        /// <summary>
        /// Calculate the points of the arc.
        /// </summary>
        void CalculateArc ()
        {
            Ray r;
            RaycastHit h = new RaycastHit();
            Vector3[] points = new Vector3[maxSteps];
            int iterations = 1;
            points[0] = transform.position + transform.forward * 0.1f;

            for(int i = 1; i < maxSteps; i++) {

                iterations++;
                Vector3 point = points[i-1] + (transform.forward * arcVelocity * stepLength) + (Vector3.down * 9.81f * stepLength * (i * stepLength));
                r = new Ray(points[i - 1], (point - points[i - 1]));
                float rayLength = Vector3.Distance(point, points[i - 1]);

                /// Cast every point twice, First for teleportable layers. 
                /// After that for any layer that prevents normal movement.
                /// (We don't want the player to teleport through a wall.)
                if (Physics.Raycast(r, out h, rayLength, teleportableLayers)) 
                {
                    if(!IsValidTeleportSpot) 
                    {
                        IsValidTeleportSpot = true;
                        LineCollidedWithBlocker = false;
                        StartCoroutine(ScaleTeleportRing(true));
                    } 
                    else 
                    {
                        points[i] = h.point;
                        SetRingPosAndRot(h.point);
                    }

                    break;
                }
                else if (Physics.Raycast(r, out h, rayLength, collisionLayers)) 
                {
                    points[i] = h.point;

                    if(IsValidTeleportSpot) 
                    {
                        StartCoroutine(ScaleTeleportRing(false));
                        IsValidTeleportSpot = false;
                        LineCollidedWithBlocker = true;
                    }

                    break;
                } 
                else 
                {
                    points[i] = point;
                }

                if(i == maxSteps-1) 
                {
                    if (IsValidTeleportSpot)
                    {
                        IsValidTeleportSpot = false;
                        LineCollidedWithBlocker = false;
                        teleportRing.localScale = Vector3.zero;
                        teleportRing.gameObject.SetActive(false);

                        if(null != arcObj) 
                        {
                            Destroy(arcObj.gameObject);
                            arcObj = null;
                        }
                    }
                }
            }

            arcPoints = new Vector3[iterations];

            for(int i = 0; i < iterations; i++) 
            {
                arcPoints[i] = points[i];
            }
        }
        
        /// <summary>
        /// Create the arc <typeparamref name="Mesh"/>. 
        /// </summary>
        void GenerateLineMesh ()
        {
            if (!IsValidTeleportSpot && !LineCollidedWithBlocker) { return; }

            Mesh m = new Mesh();
            vertices = new Vector3[cylinderResolution * arcPoints.Length];
            int[] triangles = new int[cylinderResolution * arcPoints.Length * 6];
            float angleStep = 360f / cylinderResolution;

            for(int i = 0; i < arcPoints.Length; i++) 
            {
                float ratioOfTotalDistance = i / (float)arcPoints.Length;
                float width = startWidth + (ratioOfTotalDistance * (endWidth - startWidth));
                Vector3 centerPoint = arcPoints[i];
                Vector3 dir;

                if (i == 0) 
                { 
                    dir = (arcPoints[i] - arcPoints[i + 1]).normalized; 
                }
                else if (i == arcPoints.Length - 1) 
                { 
                    dir = (arcPoints[i - 1] - arcPoints[i]).normalized; 
                }
                else 
                { 
                    dir = ((arcPoints[i - 1] - arcPoints[i]) + (arcPoints[i] - arcPoints[i + 1])).normalized; 
                }
                
                Vector3 cross = Vector3.Cross(dir, transform.right);
                
                for(int j = 0; j < cylinderResolution; j++) 
                {
                    vertices[i * cylinderResolution + j] = centerPoint + Quaternion.AngleAxis(j * angleStep, dir) * (cross * (width / 2f));
                }

                if(i > 0) 
                {
                    for (int j = 0; j < cylinderResolution; j++) 
                    {
                        int triangleStartIndex = (i - 1) * cylinderResolution * 6 + j * 6;
                        int prevA = (i - 1) * cylinderResolution + j + 0;
                        int prevB = j == cylinderResolution - 1 
                            ? ((i - 1) * cylinderResolution + 0) 
                            : ((i - 1) * cylinderResolution + j + 1
                        );
                        int curA = i * cylinderResolution + j + 0;

                        int curB = j == cylinderResolution - 1
                            ? (i * cylinderResolution + 0)
                            : (i * cylinderResolution + j + 1);

                        triangles[triangleStartIndex + j * 6 + 0] = prevA;
                        triangles[triangleStartIndex + j * 6 + 1] = curA;
                        triangles[triangleStartIndex + j * 6 + 2] = prevB;
                        triangles[triangleStartIndex + j * 6 + 3] = prevB;
                        triangles[triangleStartIndex + j * 6 + 4] = curA;
                        triangles[triangleStartIndex + j * 6 + 5] = curB;
                    }
                }
            }

            m.vertices = vertices;
            m.triangles = triangles;
            m.RecalculateNormals();
            m.Optimize();

            mFilter.mesh = m;
            mRenderer.material = arcMaterial;
            mRenderer.material.color = arcColor;
        }
        
        /// <summary>
        /// Set the position and rotation of the teleportation end point.
        /// </summary>
        /// <param name="_pos"></param>
        void SetRingPosAndRot(Vector3 _pos)
        {
            if (null == teleportRing) { return; }

            Vector3 levelForward = new Vector3(transform.forward.x, 0f, transform.forward.z);
            Vector3 inputDirection = new Vector3(-input.Primary2DAxisValue.x, 0f, input.Primary2DAxisValue.y);
            float signedAngle = Vector3.SignedAngle(inputDirection, levelForward, Vector3.up);
            Vector3 finalDirection = Quaternion.AngleAxis(signedAngle, Vector3.up) * Vector3.forward;

            Quaternion rot = Quaternion.LookRotation(finalDirection, Vector3.up);
 
            teleportRing.position = _pos;
            teleportRing.localRotation = rot;
        }
        
        /// <summary>
        /// Loop the scale of the teleportation end point.
        /// </summary>
        /// <param name="_toVisible"></param>
        /// <returns></returns>
        IEnumerator ScaleTeleportRing (bool _toVisible)
        {
            if (null == teleportRing) { yield return null; }
            
            if(_toVisible) 
            { 
                teleportRing.gameObject.SetActive(true); 
            }

            float targetScale = _toVisible ? 1f : 0f;
            float currentScale = teleportRing.localScale.x;

            while(currentScale != targetScale) 
            {
                if (null == teleportRing) 
                {
                    yield return null;
                } 
                else 
                {
                    float scaleDelta = Time.deltaTime * 5f * (_toVisible ? 1f : -1f);
                    currentScale = Mathf.Clamp(currentScale + scaleDelta, 0f, 1f);
                    teleportRing.localScale = Vector3.one * currentScale;

                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }

            if (!_toVisible) 
            { 
                teleportRing.gameObject.SetActive(false); 
            }
        }

        #endregion
        #region Teleport Functionality Methods

        /// <summary>
        /// Teleport to the position.
        /// </summary>
        /// <param name="_pos">Position to teleport to.</param>
        /// <param name="_fwd">Rotation at the arrival at <paramref name="_pos"/>.</param>
        void TeleportToPoint (Vector3 _pos, Vector3 _fwd)
        {
            player.position = _pos;
            player.rotation = Quaternion.LookRotation(_fwd, Vector3.up);
        }
        
        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace