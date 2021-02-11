using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Represents an editor window that displays all objects in a project that contain a <see cref="RuleController"/>.
/// </summary>
public class LibraryWindow : EditorWindow
{
    [SerializeField] GUISkin AdelicSkin;
    #region Fields
    SerializedObject[] capturedControllerObj;
    EditorFilters appliedSeachFilters;
    SerializedObject selectedControllerObj;
    SerializedProperty selectedRuleObj;
    SessionData MyData;
    #endregion

    [MenuItem("Adelic Systems/RuleSystem Library")]
    static void OpenWindow()
    {
        LibraryWindow window = GetWindow<LibraryWindow>();
        window.minSize = WindowUtils.LibMinWindowSize;
        window.Show();
    }

    private void OnEnable()
    {
        // catch refrences
        appliedSeachFilters = new EditorFilters();
        MyData = new SessionData();
        // Reload Profiles
        capturedControllerObj = RuleSystemUtil.CollectRuleSytemObjects();
    }

    private void OnGUI()
    {
        // Update Profiles
        UpdateCapturedObjs();
        // DrawOBJ w/ controllers
        var potentialOpenedProfile = LibraryLayoutUtil.DrawControllerLayout(capturedControllerObj, WindowUtils.ControlLayout, ref MyData ,ref appliedSeachFilters, AdelicSkin);
        if(potentialOpenedProfile != null) 
        { 
            selectedControllerObj = potentialOpenedProfile;
            MyData.selectedController = selectedControllerObj;
        }
        // If open Profile > DrawOpenProfile
        if (selectedControllerObj != null)
        {
            var potentialOpenedRule = LibraryLayoutUtil.DrawProfileLayout(selectedControllerObj, WindowUtils.ProfileLayout, ref MyData, AdelicSkin);
            if (potentialOpenedRule != null) 
            { 
                selectedRuleObj = potentialOpenedRule;
                MyData.selectedRule = selectedRuleObj;
            }
            selectedRuleObj = LibraryLayoutUtil.DrawPropertyInfoLayout(selectedControllerObj, selectedRuleObj, WindowUtils.PropertyLayout,OnOpenRuleWindowRequest, ref MyData, AdelicSkin);
        }
        else
        {
            GUILayout.BeginArea(WindowUtils.ProfileLayout);
            GUILayout.Label("No Profile Selected...");
            GUILayout.EndArea();
        }

        // Handle Events > open profile, Copy profile

        if(GUI.changed)
        {
            // Reload Profiles.
            capturedControllerObj = RuleSystemUtil.CollectRuleSytemObjects();
            // save Apply moddified Changes
            ApplyModificationCapturedObjs();
        }
    }


    /// <summary>
    /// Calls SerializedObject.Update() on each captured controller.
    /// </summary>
    private void UpdateCapturedObjs()
    {
        foreach (SerializedObject controller in capturedControllerObj)
        {
            controller.Update();
        }
    }
    /// <summary>
    /// Calls SerializedObject.ApplyModifiedProperties() on each captured controller.
    /// </summary>
    private void ApplyModificationCapturedObjs()
    {
        foreach (SerializedObject controller in capturedControllerObj)
        {
            controller.ApplyModifiedProperties();
        }
    }

    private void OnOpenRuleWindowRequest(SerializedProperty rule)
    {
        RuleWindow2 window = CreateWindow<RuleWindow2>();
        window.minSize = WindowUtils.RuleMinWindowSize;
        window.SubjectRule = rule;
        window.Initialize();
    }
}
