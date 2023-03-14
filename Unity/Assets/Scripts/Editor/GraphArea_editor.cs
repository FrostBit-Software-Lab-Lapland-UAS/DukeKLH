/******************************************************************************
 * File        : GraphArea_editor.cs
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


namespace DUKE.UI {


    /// <summary>
    /// Custom inspector for GraphArea.
    /// </summary>
    [CustomEditor(typeof(GraphArea)), CanEditMultipleObjects]
    public class GraphArea_editor : Editor 
    {
        GraphArea t;

        Dictionary<int, bool> graphSettingsDictionary = new Dictionary<int, bool>();
        bool showPrimarySettings = true;
        bool showGraphCreationSettings = false;



        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            t = (GraphArea)serializedObject.targetObject;
        }

        /// <summary>
        /// Display custom Inspector GUI.
        /// </summary>
        public override void OnInspectorGUI ()
        {
            serializedObject.Update();

            GUILayout.Space(10f);

            if (showPrimarySettings) {

                if (GUILayout.Button("Show Reference Settings", GUILayout.Width(150), GUILayout.Height (40))) {
                    showPrimarySettings = false;
                }

                GUILayout.Space(10);

                EditorGUILayout.LabelField("Primary Settings", EditorStyles.boldLabel);

                GUILayout.Space(10);

                EditorGUILayout.LabelField("Margins",EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("marginBottom"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("marginTop"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("marginLeft"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("marginRight"));

                GUILayout.Space(20);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontalRange"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("verticalRange"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("barDisplayType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gridLineColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gridBackgroundColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gridBorderWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gridLineWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gridHorizontalSegments"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gridVerticalSegments"));

                GUILayout.Space(20);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("gridName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gridHorizontalUnit"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gridVerticalUnit"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("displayNameAndUnits"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("displayHorizontalValues"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("displayVerticalValues"));

                if (t.displayVerticalValues) {
                    
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("verticalValuesAtHorizontalAlignment"));
                }
                //EditorGUILayout.PropertyField(serializedObject.FindProperty("columnNames"));

                GUILayout.Space(20);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideBarDimensions"));

                if (t.overrideBarDimensions) { 

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("customBarWidth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("customBarDepth"));
                    //EditorGUILayout.PropertyField(serializedObject.FindProperty("customBarMesh"));
                    t.UpdateBarsCustomValues();
                }

                GUILayout.Space(20);

            } else {

                if (GUILayout.Button("Show Primary Settings", GUILayout.Width(150), GUILayout.Height(40))) {
                    showPrimarySettings = true;
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("topValueLabels"));
            } 

            serializedObject.ApplyModifiedProperties();

            t.UpdateBackgroundGridVisuals();
        }
    }


}