/******************************************************************************
 * File        : Interacrable_editor.cs
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
using DUKE.Controls;


[CustomEditor(typeof(Interactable))]
public class Interactable_editor : Editor
{
    protected SerializedProperty isInteractableProp;
    protected SerializedProperty interactableTransformProp;
    protected SerializedProperty vrInteractionButtonProp;
    protected SerializedProperty desktopInteractionModifierFlagsProp;
    protected SerializedProperty interactionModeProp;
    protected SerializedProperty drawDebugProp;
    protected SerializedProperty configurationInfoProp;
    protected bool displayConfigurationInfo = false;



    protected virtual void OnEnable ()
    {
        isInteractableProp = serializedObject.FindProperty("isInteractable");
        interactableTransformProp = serializedObject.FindProperty("interactableTransform");
        vrInteractionButtonProp = serializedObject.FindProperty("vrInteractionButton");
        desktopInteractionModifierFlagsProp = serializedObject.FindProperty("desktopInteractionModifierFlags");
        interactionModeProp = serializedObject.FindProperty("interactionMode");
        drawDebugProp = serializedObject.FindProperty("drawDebug");
        configurationInfoProp = serializedObject.FindProperty("configurationInfo");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(isInteractableProp);

        if (isInteractableProp.boolValue) 
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hoveringInputs"));
            EditorGUILayout.PropertyField(drawDebugProp);
            displayConfigurationInfo = EditorGUILayout.Toggle("Display Configuration Info", displayConfigurationInfo);
        }

        serializedObject.ApplyModifiedProperties();
    }


}///  End of Class




[CustomEditor(typeof(Clickable)), CanEditMultipleObjects]
public class Clickable_editor : Interactable_editor 
{
    private Clickable clickable;

    protected SerializedProperty onClickStartedProp;
    protected SerializedProperty onClickProp;
    protected SerializedProperty clickAudioProp;
    protected SerializedProperty requireClickStartedToActivateProp;
    protected SerializedProperty isToggleableProp;
    protected SerializedProperty isToggledProp;
    protected SerializedProperty isPartOfGroupProp;
    protected SerializedProperty groupCanBeOffProp;
    protected SerializedProperty isMultiToggleableInGroupProp;



    protected override void OnEnable ()
    {
        clickable = (Clickable)serializedObject.targetObject;

        base.OnEnable();

        onClickStartedProp = serializedObject.FindProperty("onClickStarted");
        onClickProp = serializedObject.FindProperty("onClick");
        clickAudioProp = serializedObject.FindProperty("clickAudio");
        requireClickStartedToActivateProp = serializedObject.FindProperty("requireClickStartedToActivate");
        isToggleableProp = serializedObject.FindProperty("isToggleable");
        isToggledProp = serializedObject.FindProperty("isToggled");
        isPartOfGroupProp = serializedObject.FindProperty("isPartOfGroup");
        groupCanBeOffProp = serializedObject.FindProperty("groupCanBeOff");
        isMultiToggleableInGroupProp = serializedObject.FindProperty("isMultiToggleableInGroup");

        clickable.ConfigInfo =
            "Configuration of Clickable: " +
            "\n" +
            "\n" +
            "Set InteractableTransform if the object you wish to click is NOT this object. " +
            "\n" +
            "\n" +
            "For VR, set interaction button, preferably TriggerClick. " +
            "\n" +
            "For Desktop, set Interaction Modifier flags if you want to click the object only when said flags are active." +
            "\n" +
            "\n" +
            "Setup OnClick UnityEvent(s)." +
            "\n" +
            "\n" +
            "Configure toggle options.";
    }

    public override void OnInspectorGUI ()
    {
        EditorGUILayout.LabelField(clickable.IsCurved ? "Using Curved UI" : "Not using Curved UI");
        GUILayout.Space(10);


        base.OnInspectorGUI();
        
        serializedObject.Update();

        if (isInteractableProp.boolValue) {

            if (displayConfigurationInfo) {

                GUILayout.Space(10);
                EditorGUILayout.HelpBox(configurationInfoProp.stringValue, MessageType.Info);
            }

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(interactableTransformProp);

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(clickAudioProp);

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(onClickStartedProp);
            EditorGUILayout.PropertyField(onClickProp);

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(requireClickStartedToActivateProp);
            EditorGUILayout.PropertyField(isToggleableProp);

            if (isToggleableProp.boolValue) {

                if (displayConfigurationInfo) {

                    GUILayout.Space(5);

                    EditorGUILayout.HelpBox(
                        "Toggleable " +
                        "\n" +
                        "Does the UI Image color change to indicate the Clickable being the currently chosen option? " +
                        "\n" +
                        "\n" +
                        "Is Toggled " +
                        "\n" +
                        "Is the Clickable currently toggled ON?" +
                        "\n" +
                        "\n" +
                        "Is Part Of Group " +
                        "\n" +
                        "Does the Clickable belong to a collection of Clickables that can be toggled?" +
                        "\n" +
                        "\n" +
                        "Group Can Be Off " +
                        "\n" +
                        "Can the last Clickable of the group be toggled off?" +
                        "\n" +
                        "\n" +
                        "Is Multi Toggleable In Group " +
                        "\n" +
                        "Can more than one Clickable in the group be toggled ON at the same time?"
                        , MessageType.Info);
                }

                GUILayout.Space(5);

                EditorGUILayout.PropertyField(isToggledProp);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("changeColorOnToggle"));
                EditorGUILayout.PropertyField(isPartOfGroupProp);
                EditorGUILayout.PropertyField(groupCanBeOffProp);
                EditorGUILayout.PropertyField(isMultiToggleableInGroupProp);
            }

        } else {

            EditorGUILayout.HelpBox("Interaction is currently disabled.", MessageType.Warning);
        }

        GUILayout.Space(20);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("baseColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("toggleColor"));

        serializedObject.ApplyModifiedProperties();       
    }
    

} /// End of Class



[CustomEditor(typeof(Movable))]
public class Movable_editor : Interactable_editor 
{
    private Movable movable;

    protected SerializedProperty freeAxiiProp;
    protected SerializedProperty axisOrientationProp;
    protected SerializedProperty limitMovementDistanceProp;
    protected SerializedProperty movementLimitsProp;
    protected SerializedProperty limitRatiosProp;
    protected SerializedProperty allowedPosMovementProp;
    protected SerializedProperty allowedNegMovementProp;



    protected override void OnEnable ()
    {
        movable = (Movable)serializedObject.targetObject;
        
        base.OnEnable();

        freeAxiiProp = serializedObject.FindProperty("freeAxii");
        axisOrientationProp = serializedObject.FindProperty("axisOrientation");
        limitMovementDistanceProp = serializedObject.FindProperty("limitMovementDistance");
        movementLimitsProp = serializedObject.FindProperty("movementLimits");
        limitRatiosProp = serializedObject.FindProperty("limitRatios");
        allowedPosMovementProp = serializedObject.FindProperty("allowedMovToPosDir");
        allowedNegMovementProp = serializedObject.FindProperty("allowedMovToNegDir");


        movable.ConfigInfo =
             "Configuration \n of Movable: " +
                "\n" +
                "\n" +
                "Set InteractableTransform if the object you wish to move is NOT this object. " +
                "\n" +
                "\n" +
                "For VR, set interaction button, preferably Grip. " +
                "\n" +
                "For Desktop, set Interaction Modifier flags if you want to move the object only when said flags are active." +
                "\n" +
                "\n" +
                "Select which axii the object should move along and whether the movement happens in world or local coordinates." +
                "\n" +
                "\n" +
                "Select whether or not to limit the movement per axis.";
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (isInteractableProp.boolValue) {

            if (displayConfigurationInfo) {

                GUILayout.Space(10);
                EditorGUILayout.HelpBox(configurationInfoProp.stringValue, MessageType.Info);
            }

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(interactableTransformProp);
            
            GUILayout.Space(5);

            EditorGUILayout.PropertyField(vrInteractionButtonProp);
            EditorGUILayout.PropertyField(desktopInteractionModifierFlagsProp);
            EditorGUILayout.PropertyField(interactionModeProp);

            GUILayout.Space(20);

            EditorGUILayout.PropertyField(freeAxiiProp);
            EditorGUILayout.PropertyField(axisOrientationProp);

            GUILayout.Space(10f);

            EditorGUILayout.PropertyField(limitMovementDistanceProp);

            if (limitMovementDistanceProp.boolValue) {

                if (displayConfigurationInfo) {

                    EditorGUILayout.HelpBox(
                        "Movement Limits are in meters (total distance). " +
                        "For example (0,1,2) means the object can't move on X axis (even if that axis is free)," +
                        "but it can move a one meter on Y and two meters on Z axii." +
                        "\n" +
                        "\n" +
                        "Limit Ratios define the position within the limits. Following the above example, an object with (0.5,0.5,0.5)" +
                        "is at dead center of its movable area, so the object could move 0.5m up or down and one meter to either direction along Z axis."

                        , MessageType.Info);
                }

                //EditorGUILayout.PropertyField(movementLimitsProp);
                //EditorGUILayout.PropertyField(limitRatiosProp);
                EditorGUILayout.PropertyField(allowedPosMovementProp);
                EditorGUILayout.PropertyField(allowedNegMovementProp);

                if (Application.isPlaying && movable.LimitMovement) {

                    if (GUILayout.Button("Refresh Movement Limits", GUILayout.Width(300), GUILayout.Height(40))) {

                        movable.RefreshMovementLimits();
                    }
                }
            }

        }

        serializedObject.ApplyModifiedProperties();
    }


} /// End of Class



[CustomEditor(typeof(Highlightable))]
public class Highlightable_editor: Interactable_editor
{
    private Highlightable highlightable;

    

    protected override void OnEnable ()
    {
        highlightable = (Highlightable)serializedObject.targetObject;

        base.OnEnable();

        highlightable.ConfigInfo =
             "Configuration of Highlightable: " +
                "\n" +
                "\n" +
                "Set InteractableTransform if the interaction object is NOT this object. " +
                "\n" +
                "\n" +
                "Select HighlightModes to activate when highlighting over the object.\n\n" +
                "UnityEvent \n" +
                "Use UnityEvents when highlighting starts and/or ends. \n\n" +
                "Material \n" +
                "Change the material of the object when it is highlighted.\n" +
                "Return to the original material when highlighting ends. \n\n" +
                "Scale \n" +
                "Change the scale of the object when it is highlighted. \n" +
                "Return to the original scale when highlighting ends. \n\n" +
                "GraphText \n" +
                "Display information of a graph bar or point on a curved UI display.\n\n" +
                "Tooltip \n" +
                "Display a tooltip with settings configured on a separate TooltipSettings component.";
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (isInteractableProp.boolValue) {

            if (displayConfigurationInfo) {

                GUILayout.Space(10);
                EditorGUILayout.HelpBox(configurationInfoProp.stringValue, MessageType.Info);
            }

            GUILayout.Space(20);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("highlightMode"));

            if (highlightable.UsingUnityEvents) {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnHighlightBegin"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnHighlightEnd"));
            }

            if (highlightable.UsingMaterial) {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("highlightMaterial"));
            }
            
            if (highlightable.UsingScale) {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("highlightScale"));
            }

            if (highlightable.UsingGraphText) {

            }

            if (highlightable.UsingTooltip) {

                if (highlightable.TryGetComponent(out DUKE.UI.TooltipSettings ttSettings)) {

                    EditorGUILayout.HelpBox(
                        "Configure Tooltip through the attached TooltipSettings component.",
                        MessageType.Info
                    );

                } else {

                    EditorGUILayout.HelpBox(
                        "Couldn't find TooltipSettings component.",
                        MessageType.Warning
                    );
                }
            }         
        } else {

            EditorGUILayout.HelpBox("Interaction is currently disabled.", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();      
    }


} /// End of Class



[CustomEditor(typeof(AreaSelector))]
public class AreaSelector_editor : Interactable_editor 
{
    private AreaSelector areaSelector;
    private bool debugFoldout = false;


    protected SerializedProperty onClickStartedProp;
    protected SerializedProperty onClickProp;
    protected SerializedProperty clickAudioProp;
    protected SerializedProperty requireClickStartedToActivateProp;
    protected SerializedProperty updateOnDragProp;
    protected SerializedProperty marginProp;
    protected SerializedProperty displayPointerProp;
    protected SerializedProperty foregroundImageProp;

    protected SerializedProperty outerMinAnglesProp;
    protected SerializedProperty outerMaxAnglesProp;
    protected SerializedProperty innerMinAnglesProp;
    protected SerializedProperty innerMaxAnglesProp;
    protected SerializedProperty hitAnglesProp;



    protected override void OnEnable ()
    {
        areaSelector = (AreaSelector)serializedObject.targetObject;

        base.OnEnable();

        onClickStartedProp = serializedObject.FindProperty("onClickStarted");
        onClickProp = serializedObject.FindProperty("onClick");
        clickAudioProp = serializedObject.FindProperty("clickAudio");
        requireClickStartedToActivateProp = serializedObject.FindProperty("requireClickStartedToActivate");
        updateOnDragProp = serializedObject.FindProperty("updateOnDrag");
        marginProp = serializedObject.FindProperty("margin");
        displayPointerProp = serializedObject.FindProperty("displayPointer");
        foregroundImageProp = serializedObject.FindProperty("foregroundImg");

        outerMinAnglesProp = serializedObject.FindProperty("outerMinAngles");
        outerMaxAnglesProp = serializedObject.FindProperty("outerMaxAngles");
        innerMinAnglesProp = serializedObject.FindProperty("innerMinAngles");
        innerMaxAnglesProp = serializedObject.FindProperty("innerMaxAngles");
        hitAnglesProp = serializedObject.FindProperty("hitAngles");

        areaSelector.ConfigInfo =
             "Configuration of AreaSelector: " +
                "\n" +
                "\n" +
                "Set InteractableTransform if the interaction object is NOT this object. " +
                "\n" +
                "\n" +
                "Set BackgroundImg if you want the selection area to have an image within it." +
                "\n" +
                "\n" +
                "Set Margin if you want the BackgroundImg to be smaller than the selection area. " +
                "Coordinates are confined to BackgroundImg's area, but inputs are registered within the selection area.";
    }

    public override void OnInspectorGUI ()
    {
        EditorGUILayout.LabelField(areaSelector.IsCurved ? "Using Curved UI" : "Not using Curved UI");
        GUILayout.Space(10);    

        base.OnInspectorGUI();

        serializedObject.Update();

        if (isInteractableProp.boolValue) {

            if (displayConfigurationInfo) {

                GUILayout.Space(10);
                EditorGUILayout.HelpBox(configurationInfoProp.stringValue, MessageType.Info);
            }

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(interactableTransformProp);

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(clickAudioProp);

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(onClickStartedProp);
            EditorGUILayout.PropertyField(onClickProp);

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(foregroundImageProp);
            
            if (areaSelector.HasForegroundImage) {

                EditorGUILayout.PropertyField(marginProp);
            }

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(requireClickStartedToActivateProp);
            EditorGUILayout.PropertyField(updateOnDragProp);
            EditorGUILayout.PropertyField(displayPointerProp);

            GUILayout.Space(10);

            debugFoldout = EditorGUILayout.Foldout(debugFoldout, "Debug Values", true);

            if (debugFoldout) {

                EditorGUILayout.LabelField("Outer MIN: " + areaSelector.OuterMinAngles);
                EditorGUILayout.LabelField("Outer MAX: " + areaSelector.OuterMaxAngles);
                EditorGUILayout.LabelField("Inner MIN: " + areaSelector.InnerMinAngles);
                EditorGUILayout.LabelField("Inner MAX: " + areaSelector.InnerMaxAngles);
                EditorGUILayout.LabelField("Hit: " + areaSelector.HitAngles);
                EditorGUILayout.LabelField("Coordinates: " + areaSelector.Ratio);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }


} /// End of Class