/******************************************************************************
 * File        : Building_editor.cs
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
using DUKE.KLHData;


[CustomEditor(typeof(Building))]
public class Building_editor : Editor
{

    Building t;


    private void OnEnable ()
    {
        t = (Building)serializedObject.targetObject;
    }


    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Grid",GUILayout.Height(50));

        if (null == t.DrawGrid) {

            EditorGUILayout.LabelField("Grid is NULL");

        } else {

            EditorGUILayout.LabelField("width: " + t.DrawGrid.Width);
            EditorGUILayout.LabelField("Depth: " + t.DrawGrid.Depth);
            EditorGUILayout.LabelField("Cells: " + (t.DrawGrid.NullCells ? "NULL" : "OK"));
            EditorGUILayout.LabelField("List length: " + (null == t.DrawGrid.CellsList ? "NULL" : t.DrawGrid.CellsList.Count));
        }


    }
}