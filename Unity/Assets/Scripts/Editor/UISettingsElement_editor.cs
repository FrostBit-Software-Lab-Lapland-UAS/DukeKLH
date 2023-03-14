/******************************************************************************
 * File        : UISettingsElement_editor.cs
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
using DUKE.UI;


[CustomEditor(typeof(UISettingsElement)), CanEditMultipleObjects]
public class UISettingsElement_editor : Editor
{
    #region Variables

    UISettingsElement t;
    List<UISettingsElement> ts;

    SerializedProperty uiColorModeProp;
    SerializedProperty customColorProp;
    SerializedProperty uiFontModeProp;

    #endregion


    #region Methods

    private void OnEnable ()
    {
        uiColorModeProp = serializedObject.FindProperty("uiColorMode");
        customColorProp = serializedObject.FindProperty("customColor");
        uiFontModeProp = serializedObject.FindProperty("uiFontMode");

        t = (UISettingsElement)serializedObject.targetObject;
        ts = new List<UISettingsElement>();
        Object[] tsObjects = serializedObject.targetObjects;

        for (int i = 0; i < tsObjects.Length; i++) 
            { ts.Add((UISettingsElement)tsObjects[i]); }
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.Space(10f);

        /// Modify default label width.
        float defaultLabelWidth = EditorGUIUtility.labelWidth;
        float defaultFieldWidth = EditorGUIUtility.fieldWidth;
        EditorGUIUtility.labelWidth = 120f;
        EditorGUIUtility.fieldWidth = 200f;

        EditorGUILayout.PropertyField(uiColorModeProp, GUILayout.ExpandWidth(false));

        if (t.UIColorMode == UIColorMode.Custom)    { EditorGUILayout.PropertyField(customColorProp, GUILayout.ExpandWidth(false)); }
 
        GUILayout.Space(20);

        if (null != t.SettingsProvider) { EditorGUILayout.LabelField("Provider", t.SettingsProvider.name);}
        if (null != t.UIPalette)        { EditorGUILayout.LabelField("UI Palette", t.UIPalette.name); }

        EditorGUIUtility.labelWidth = defaultLabelWidth;
        EditorGUIUtility.fieldWidth = defaultFieldWidth;

        EditorGUILayout.Space(10f);

        serializedObject.ApplyModifiedProperties();

        //t.SetVisuals();

        foreach (UISettingsElement element in ts) { 
            
            if (!Application.isPlaying) { element.SetVisuals(); }
        }
    }

    #endregion


} /// End of Class