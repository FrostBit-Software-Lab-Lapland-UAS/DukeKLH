/******************************************************************************
 * File        : DrawAction.cs
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


namespace DUKE {

    /// <summary>
    /// Progress states of a <typeparamref name="DrawAction"/>.
    /// </summary>
    public enum DrawActionState 
    {  
        Start,
        Ongoing,
        End,
        None
    }


    /// <summary>
    /// A container for an action that involves drawing on a plane.
    /// </summary>
    [System.Serializable]
    public class DrawAction 
    {    
        #region Variables

        [SerializeField] DrawActionState state;
        [SerializeField] DUKE.Controls.Input source;
        [SerializeField] Transform parent;
        [SerializeField] int drawValue;
        [SerializeField] Vector3 startPoint;
        //[SerializeField] Vector3 localStartPoint;
        [SerializeField] Vector3 endPoint;

        #endregion


        #region Properties

        /// <summary>
        /// Current <typeparamref name="Input"/> source.
        /// </summary>
        public DUKE.Controls.Input Source 
        { 
            get { return source; } 
        }
        
        /// <summary>
        /// Current progress state.
        /// </summary>
        public DrawActionState State 
        { 
            get { return state; } 
        }

        /// <summary>
        /// World position of the starting point.
        /// </summary>
        public Vector3 StartPoint 
        { 
            get { return new Vector3(startPoint.x, parent.position.y, startPoint.z); } 
            set { startPoint = value; LocalStartXZ = new Vector2(value.x - parent.position.x, value.z - parent.position.z); } 
        }
        
        private Vector2 LocalStartXZ
        {
            get;
            set;
        }

        /// <summary>
        /// <paramref name="StartPoint"/> converted to local coordinates.
        /// </summary>
        public Vector3 LocalStartPoint 
        { 
            get { return new Vector3 (LocalStartXZ.x, 0f, LocalStartXZ.y); }
        }
        
        /// <summary>
        /// World position of the end point.
        /// </summary>
        public Vector3 EndPoint 
        { 
            get { return new Vector3(endPoint.x, parent.position.y, endPoint.z); } 
            set { endPoint = value; } 
        }

        /// <summary>
        /// <paramref name="EndPoint"/> converted to local coordinates.
        /// </summary>
        public Vector3 LocalEndPoint
        {
            get { return new Vector3(EndPoint.x - parent.position.x, 0f, EndPoint.z - parent.position.z); }
        }

        public Vector3 LocalMin
        {
            get { return Vector3.Min(LocalStartPoint, LocalEndPoint); }
        }

        public Vector3 LocalMax
        {
            get { return Vector3.Max(LocalStartPoint, LocalEndPoint); }
        }

        public Vector3 Delta
        {
            get { return EndPoint - StartPoint; }
        }
        
        /// <summary>
        /// Current value (<typeparamref name="BuildingDrawer"/>.<typeparamref name="CurrentFloor"/>).
        /// </summary>
        public int DrawValue 
        { 
            get { return drawValue; } 
            set { drawValue = value; } 
        }
        
        /// <summary>
        /// TRUE if <typeparamref name="Source"/> is not null.
        /// </summary>
        public bool HasSource 
        { 
            get { return Source != null; } 
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Empty constructor of <typeparamref name="DrawAction"/>.
        /// </summary>
        public DrawAction () { }

        /// <summary>
        /// Constructor of <typeparamref name="DrawAction"/>.
        /// </summary>
        public DrawAction (DUKE.Controls.Input _source, Transform _parent, int _val)
        {
            source = _source;
            parent = _parent;
            state = DrawActionState.Start;
            startPoint = endPoint = Vector3.zero;
            drawValue = _val;
        }

        #endregion


    } /// End of Class


} /// End of Namespace