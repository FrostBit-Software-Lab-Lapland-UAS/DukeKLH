/******************************************************************************
 * File        : HybridDevice_editor.cs
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


/// <summary>
/// Editor script for overriding default inspector of HybridDevice. 
/// Updates the calculation values when inspector is open.
/// </summary>
[CustomEditor(typeof(HeatingMethod))]
public class HybridDevice_editor : Editor
{
    HeatingMethod device;


    public override void OnInspectorGUI ()
    {
        if (null == device) { device = (HeatingMethod)target; }

        base.OnInspectorGUI();

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Initial Investment Cost:      " + device.InvestmentCost);
        EditorGUILayout.LabelField("Annual Operating Cost:      " + device.AnnualOperatingCost);
    }
}



/// <summary>
/// Editor script for overriding default inspector of HeatPumpDevice. 
/// Updates the calculation values when inspector is open.
/// </summary>
[CustomEditor(typeof(AirsourceHeatPumpHeating))]
public class HeatPumpDevice_editor : Editor {
    AirsourceHeatPumpHeating device;

    public override void OnInspectorGUI ()
    {
        if (null == device) { device = (AirsourceHeatPumpHeating)target; }

        base.OnInspectorGUI();

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Initial Investment Cost:      " + device.InvestmentCost);
        EditorGUILayout.LabelField("Annual Operating Cost:      " + device.AnnualOperatingCost); 
    }
}



/// <summary>
/// Editor script for overriding default inspector of GeothermalDevice. 
/// Updates the calculation values when inspector is open.
/// </summary>
[CustomEditor(typeof(GeothermalHeating))]
public class GeothermalDevice_editor : Editor {
    GeothermalHeating device;

    public override void OnInspectorGUI ()
    {
        if (null == device) { device = (GeothermalHeating)target; }

        base.OnInspectorGUI();

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Initial Investment Cost:      " + device.InvestmentCost);
        EditorGUILayout.LabelField("Annual Operating Cost:      " + device.AnnualOperatingCost);
    }
}



/// <summary>
/// Editor script for overriding default inspector of HeatRecoveryDevice. 
/// Updates the calculation values when inspector is open.
/// </summary>
[CustomEditor(typeof(HeatRecoveryHeating))]
public class HeatRecoveryDevice_editor : Editor {

    HeatRecoveryHeating device;


    public override void OnInspectorGUI ()
    {
        if (null == device) { device = (HeatRecoveryHeating)target; }

        base.OnInspectorGUI();

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Initial Investment Cost:      " + device.InvestmentCost);
        EditorGUILayout.LabelField("Annual Operating Cost:      " + device.AnnualOperatingCost);
    }
}