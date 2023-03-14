/******************************************************************************
 * File        : LookPoint.cs
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
    /// A point on the floor to which the user can move to by clicking it.
    /// Forward direction is specified.
    /// </summary>
    public class LookPoint : MonoBehaviour
    {
        [SerializeField] bool preventRotation = false;



        /// <summary>
        /// TRUE if rotation should be prevented.
        /// </summary>
        public bool PreventRotation 
        { 
            get { return preventRotation; } 
        }



        /// <summary>
        /// Move Player to this transform's position (and rotation if preventRotation=FALSE).
        /// </summary>
        public void Move ()
        {
            Debug.Log("LookPoint.Move()...");

            if (preventRotation) 
            {  
                SceneController.DoFade<Vector3, bool>(
                    true, 
                    KLHManager.FadeDuration, 
                    KLHManager.TeleportPlayer, 
                    transform.position, 
                    true
                );
            } 
            else 
            {      
                SceneController.DoFade<Transform, bool>(
                    true, 
                    KLHManager.FadeDuration, 
                    KLHManager.TeleportPlayer, 
                    transform, 
                    true
                ); 
            }        
        }


    } /// End of Class


} /// End of Namespace