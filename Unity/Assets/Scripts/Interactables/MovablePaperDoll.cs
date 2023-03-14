/******************************************************************************
 * File        : MovablePaperDoll.cs
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


namespace DUKE.Controls {


    public class MovablePaperDoll : Movable
    {
        [SerializeField] float smoothing = 0.01f;
        [SerializeField] Transform rayTrackingPlane;
        Vector3 targetPoint;
        Vector3 touchPoint;



        public override void BeginInteraction(Input _source)
        {
            base.BeginInteraction(_source);
            targetPoint = InteractableTransform.position;
            touchPoint = vrInput.TouchPointPosition;
        }



        protected override void VRInteractionUpdate()
        {
            if (IsOverlap)
            {
                Vector3 newPos = vrInput.TouchPointPosition;
                 MoveObjectWithDelta(newPos-touchPoint);
                 touchPoint = newPos;
            }
            else if (IsRaycast)
            {
                if (vrInput.PointerTarget == rayTrackingPlane || vrInput.PointerTarget == transform)
                {
                    targetPoint = vrInput.Hit.point;
                }

                Vector3 interpolatedPoint = Vector3.Lerp(InteractableTransform.position, targetPoint, smoothing);
                Vector3 delta = interpolatedPoint - InteractableTransform.position;

                MoveObjectWithDelta(delta);
            }
        }

        protected override void DesktopInteractionUpdate()
        {
            if (interactionMode.HasFlag(InteractionMode.Raycast))
            {
                if (mkbInput.PointerTarget == rayTrackingPlane || mkbInput.PointerTarget == transform)
                {
                    targetPoint = mkbInput.Hit.point;
                }

                Vector3 interpolatedPoint = Vector3.Lerp(InteractableTransform.position, targetPoint, smoothing);
                Vector3 delta = interpolatedPoint - InteractableTransform.position;

                MoveObjectWithDelta(delta);
            }
        }


    } /// End of Class


} /// End of Namespace