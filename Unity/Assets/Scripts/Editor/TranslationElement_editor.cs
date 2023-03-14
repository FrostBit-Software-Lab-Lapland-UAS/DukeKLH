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
using DUKE;


[CustomEditor(typeof(TranslationElement))]
public class TranslationElement_editor : Editor
{
    TranslationElement t;
    bool idFoldout = true;



    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        t = (TranslationElement)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));

        GUILayout.Space(10);

        idFoldout = EditorGUILayout.Foldout(idFoldout, "List of all IDs");

        if (idFoldout){
            
            string[] allIds = TranslatorSO.GetTranslationIdList();

            EditorGUILayout.BeginHorizontal();

            for (int x=0; x<allIds.Length; x ++ ) {

                if (x % 3 == 0) {

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                if (allIds[x] == t.ID)  { GUI.contentColor = new Color(0f, 0.75f, 0.25f, 1f); }
                else                    { GUI.contentColor = new Color(0.9f, 0.9f, 0.9f, 1f); }

                if (GUILayout.Button(allIds[x])) {
                    
                    serializedObject.FindProperty("id").stringValue = allIds[x];
                    t.SetTranslationText();  
                    break;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
                
        serializedObject.ApplyModifiedProperties();
    }


} /// End of Class