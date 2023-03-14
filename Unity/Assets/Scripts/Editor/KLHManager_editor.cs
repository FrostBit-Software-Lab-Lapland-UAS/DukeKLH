/******************************************************************************
 * File        : KLHManager_editor.cs
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
using DUKE;


[CustomEditor(typeof(KLHManager))]
public class KLHManager_editor : Editor
{
    #region Variables

    KLHManager t;

    bool referencesFoldout = false;

    #endregion


    #region Methods

    private void OnEnable ()
    {
        t = (KLHManager)serializedObject.targetObject;
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.Space(10f);
 
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("language"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultPalette"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeDuration"));

        EditorGUILayout.Space(10f);

        referencesFoldout = EditorGUILayout.Foldout(referencesFoldout, "References", true);

        if (referencesFoldout) {

            EditorGUILayout.PropertyField(serializedObject.FindProperty("mainMenuUI"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("buildingDrawer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("buildingDrawerUI"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("visualisationUI"));
        }

        EditorGUILayout.Space(20f);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("editTemplates"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("resetTemplates"));   
        EditorGUILayout.PropertyField(serializedObject.FindProperty("building"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("buildings"));

        GUILayout.Space(10);

        serializedObject.ApplyModifiedProperties();
    }

    #endregion


} /// End of Class