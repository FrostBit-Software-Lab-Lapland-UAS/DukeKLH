/******************************************************************************
 * File        : CurvedUIElement_editor.cs
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
using UnityEditor;
using DUKE.KLHData;


namespace DUKE.UI {


    [CustomEditor(typeof(CurvedUIElement))]
    public class CurvedUIElement_editor : Editor {

        private CurvedUIElement t;
        private bool referencesFoldout = false;

        protected SerializedProperty enableProcessingProp;
        protected SerializedProperty referencesSetProp;
        protected SerializedProperty addCollisionProp;
        protected SerializedProperty imageResolutionProp;

        protected SerializedProperty curvedUIBaseProp;
        protected SerializedProperty rectTrnProp;
        protected SerializedProperty sourceRTProp;
        protected SerializedProperty parentRTProp;

        protected SerializedProperty drawDebugProp;
        protected SerializedProperty logDebugInfoProp;




        private void OnEnable ()
        {
            t = (CurvedUIElement)serializedObject.targetObject;

            enableProcessingProp = serializedObject.FindProperty("enableProcessing");
            referencesSetProp = serializedObject.FindProperty("referencesSet");
            addCollisionProp = serializedObject.FindProperty("addCollision");
            imageResolutionProp = serializedObject.FindProperty("imageResolution");

            curvedUIBaseProp = serializedObject.FindProperty("curvedUIBase");
            rectTrnProp = serializedObject.FindProperty("rectTrn");
            sourceRTProp = serializedObject.FindProperty("sourceRT");
            parentRTProp = serializedObject.FindProperty("parentRT");

            drawDebugProp = serializedObject.FindProperty("drawDebug");
            logDebugInfoProp = serializedObject.FindProperty("logDebugInfo");
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(enableProcessingProp);

            if (enableProcessingProp.boolValue) 
            {
                EditorGUILayout.PropertyField(referencesSetProp);
                EditorGUILayout.PropertyField(addCollisionProp);
                EditorGUILayout.PropertyField(drawDebugProp);
                EditorGUILayout.PropertyField(logDebugInfoProp);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(imageResolutionProp);
                GUILayout.Space(10);

                referencesFoldout = EditorGUILayout.Foldout(referencesFoldout, "References", true);

                if (referencesFoldout) 
                {
                    EditorGUILayout.PropertyField(curvedUIBaseProp);
                    EditorGUILayout.PropertyField(rectTrnProp);
                    EditorGUILayout.PropertyField(sourceRTProp);
                    EditorGUILayout.PropertyField(parentRTProp);
                    GUILayout.Space(20);

                    if (GUILayout.Button("Reset References of children", GUILayout.Height(30))) 
                    {
                        t.ResetReferencesOfChildren();
                    }
                }
            } 
            else 
            {
                EditorGUILayout.HelpBox("Processing has been disabled.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }






    } /// End of Class


} /// End of Namespace