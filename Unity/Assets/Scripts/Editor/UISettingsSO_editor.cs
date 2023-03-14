/******************************************************************************
 * File        : UISettingsSO_editor.cs
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


[CustomEditor(typeof(UIPaletteSO))]
public class UISettingsSO_editor : Editor
{
    UIPaletteSO t;

    SerializedProperty primaryColorProp;
    SerializedProperty secondaryColorProp;
    SerializedProperty tertiaryColorProp;
    SerializedProperty accentAColorProp;
    SerializedProperty accentBColorProp;

    bool[] selectedColors;
    SerializedProperty[] properties;

    bool colorsSwapped;

    private void OnEnable()
    {
        t = (UIPaletteSO)serializedObject.targetObject;

        primaryColorProp = serializedObject.FindProperty("primaryColor");
        secondaryColorProp = serializedObject.FindProperty("secondaryColor");
        tertiaryColorProp = serializedObject.FindProperty("tertiaryColor");
        accentAColorProp = serializedObject.FindProperty("accentColorA");
        accentBColorProp = serializedObject.FindProperty("accentColorB");

        properties = new SerializedProperty[5] { primaryColorProp, secondaryColorProp, tertiaryColorProp, accentAColorProp, accentBColorProp };
        selectedColors = new bool[properties.Length];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.HelpBox("T�h�n ohjeteksti�", MessageType.Info);
        for (int i = 0; i < 5; i++)
        {
            EditorGUILayout.BeginHorizontal();
            selectedColors[i] = EditorGUILayout.Toggle(selectedColors[i], GUILayout.MaxWidth(50));
            EditorGUILayout.PropertyField(properties[i]);
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        DrawPropertiesExcluding(serializedObject, "primaryColor", "secondaryColor", "tertiaryColor", "accentColorA", "accentColorB");
        SwapColors();
        serializedObject.ApplyModifiedProperties();
        
        if (colorsSwapped)
        {
            colorsSwapped = false;
            foreach (UISettingsProvider uiSettingsP in FindObjectsOfType<UISettingsProvider>())
            {
                uiSettingsP.SettingsChanged();
            }
        }
    }

    void SwapColors()
    {
        List<int> indexes = new List<int>();

        for (int i=0; i<selectedColors.Length; i++)
        {
            if (selectedColors[i]) { indexes.Add(i); }
        }

        if (indexes.Count > 1)
        {
            Color tempColor = properties[indexes[0]].colorValue;
            properties[indexes[0]].colorValue = properties[indexes[1]].colorValue;
            properties[indexes[1]].colorValue = tempColor;

            selectedColors[indexes[0]] = false;
            selectedColors[indexes[1]] = false;

            colorsSwapped = true;

           
        }

    }
}
