using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static RuleEditorSettings;
/// <summary>
/// Represents Draw functions for all layout area's in both <see cref="LibraryWindow"/> and <see cref="RuleWindow2"/>.
/// </summary>
public static class LibraryLayoutUtil
{
    const string ruleTitleFormat = "{0} ({1})";

    /// <summary>
    /// Draw's button list of objects with a <see cref="RuleController"/> in defined area.
    /// </summary>
    /// <param name="controllerObjs">Array of controllers that should be drawn.</param>
    /// <param name="layoutArea">Designated area where controllers are drawn into.</param>
    /// <param name="appliedFilters">Search filters applied to Library.</param>
    /// <param name="skin"><see cref="GUISkin"/> style for GUI elements.</param>
    /// <returns>Serialized controller if user selects one. Else null is returned.</returns>
    public static SerializedObject DrawControllerLayout(SerializedObject[] controllerObjs, Rect layoutArea,ref SessionData data ,ref EditorFilters appliedFilters, GUISkin skin)
    {
        List<SerializedObject> controllersToDraw = new List<SerializedObject>();

        GUILayout.BeginArea(layoutArea,skin.window);
        // Draw Searchbar
        appliedFilters = DrawFilterableSearchbar(appliedFilters, RuleEditorSettings.Instance.ProfileFilters, skin);
        // apply filters
        controllersToDraw = FilterControllers(controllerObjs, appliedFilters);

        GUILayout.Space(15);

        // Draw  controllers.
        foreach (TypeFilter controllerType in Enum.GetValues(typeof(TypeFilter)))
        {
            List<SerializedObject> controllersByType = new List<SerializedObject>();
            foreach (SerializedObject controller in controllersToDraw)
            {
                if((controller.targetObject as RuleController).Profile.MyFilters.Type == controllerType)
                {
                    controllersByType.Add(controller);
                }
            }
            if (controllersByType.Count > 0)
            {
                if(!data.isProfileTypeToggled.ContainsKey(controllerType))
                { data.isProfileTypeToggled.Add(controllerType, false); }
                data.isProfileTypeToggled[controllerType] = GUILayout.Toggle(data.isProfileTypeToggled[controllerType],controllerType.ToString(),skin.toggle ,GUILayout.Height(20));
                GUILayout.Label("", skin.GetStyle("HorizontalDivider"));
                if (data.isProfileTypeToggled[controllerType])
                {
                    foreach (SerializedObject controller in controllersByType)
                    {
                        var prevBG = GUI.backgroundColor;
                        if(data.selectedController != null && data.selectedController.targetObject == controller.targetObject)
                        {
                            GUI.backgroundColor = Color.green;
                        }
                        if (GUILayout.Button(controller.targetObject.name, skin.button, GUILayout.Height(25)))
                        {
                            return controller;
                        }
                        GUI.backgroundColor = prevBG;
                    }
                }
            }
        }
        GUILayout.EndArea();
        return null;
    }

    /// <summary>
    /// Draws opened <see cref="BehaviourProfile"/>. Shows Rules and Rule-sets.
    /// </summary>
    /// <param name="profileToDisplay"><see cref="SerializedObject"/> of opened target profile.</param>
    /// <param name="layoutArea">Rectangle containing size and position of this area within an editorwindow. </param>
    /// <param name="data"><see cref="SessionData"/> container used by an editorwindow. </param>
    /// <param name="skin"><see cref="GUISkin"/> of area layout. </param>
    /// <returns>Selected <see cref="Rule"/> property. </returns>
    public static SerializedProperty DrawProfileLayout(SerializedObject profileToDisplay, Rect layoutArea, ref SessionData data, GUISkin skin)
    {
        SerializedProperty profileProp = profileToDisplay.FindProperty("Profile");
        if(profileProp == null)
        {
            (profileToDisplay.targetObject as RuleController).Profile = new BehaviourProfile();
        }
        List<SerializedProperty> ruleSetsProp = new List<SerializedProperty>();
        GUILayout.BeginArea(layoutArea, skin.window);
        if(profileProp.hasVisibleChildren)
        {
            foreach (SerializedProperty child in profileProp.GetChildren())
            {
                if(child.type == "Rule")
                {
                    var Rule = child.Copy();
                    ruleSetsProp.Add(Rule);
                }
            }
        }
        if(ruleSetsProp.Count > 0)
        {
            Vector2[] setAnchors = WindowUtils.RuleSetLayout.GetAsTileAnchors(ruleSetsProp.Count);
            for (int i = 0; i < ruleSetsProp.Count; i++)
            {
                SerializedProperty selectedRule = DrawRuleSetLayout(ruleSetsProp[i], setAnchors[i],ref data, skin);
                if(selectedRule != null) 
                {
                    data.ToolbarIndex = 1;
                    return selectedRule; 
                }
            }
        }
        GUILayout.EndArea();
        return null;
    }

    /// <summary>
    /// Draws info panel containing profile information.
    /// </summary>
    /// <param name="displayProfile"><see cref="SerializedObject"/> of opened target profile. </param>
    /// <param name="selectedRule"><see cref="Rule"/> that is selected. Shows rule information.</param>
    /// <param name="layoutArea">Rectangle containing size and position of this area within an editorwindow. </param>
    /// <param name="OnOpenRule">Action-event handles opening a <see cref="RuleWindow2"/>.</param>
    /// <param name="data"><see cref="SessionData"/> container used by an editorwindow.</param>
    /// <param name="skin"><see cref="GUISkin"/> of area layout. </param>
    /// <returns><see cref="SerializedProperty"/> of opended rule. </returns>
    public static SerializedProperty DrawPropertyInfoLayout(SerializedObject displayProfile, SerializedProperty selectedRule, Rect layoutArea,Action<SerializedProperty> OnOpenRule ,ref SessionData data ,GUISkin skin)
    {
        GUILayout.BeginArea(layoutArea, skin.window);
        data.ToolbarIndex = GUILayout.Toolbar(data.ToolbarIndex, new string[] { "Profile", "Edit Rule" });
        switch (data.ToolbarIndex)
        {
            case 0:
                ProfileData(displayProfile, skin);
                break;
            case 1:
                if(selectedRule != null)
                {
                    selectedRule = EditRuleData(selectedRule,ref data,OnOpenRule,skin);
                }
                else
                {
                    GUILayout.Space(20);
                    GUILayout.Label("No rule selected...");
                }
                break;
        }
        GUILayout.EndArea();
        return selectedRule;
    }


    /// <summary>
    /// Creates a searchbar that recognizes <see cref="EditorFilter"/> and returns updated <see cref="EditorFilters"/>.
    /// </summary>
    /// <param name="filters"> Object containing filters for searchbar</param>
    /// <param name="skin">Custom editor skin</param>
    /// <returns>updated filters</returns>
    private static EditorFilters DrawFilterableSearchbar(EditorFilters filters, Filter[] availableFilters ,GUISkin skin)
    {
        GUILayout.BeginHorizontal(GUILayout.Height(25));
        GUILayout.Label("", skin.GetStyle("Search"), GUILayout.Width(25));
        filters.Search = GUILayout.TextField(filters.Search, skin.textField, GUILayout.Width(150));
        if(GUILayout.Button("",skin.GetStyle("FilterManager"), GUILayout.Width(20), GUILayout.Height(25)))
        {
            EditorLabelManager window = EditorWindow.GetWindow<EditorLabelManager>();
            window.ShowUtility();
        }
        GUILayout.EndHorizontal();

        if (filters.Search != string.Empty)
        {
            foreach (Filter filter in availableFilters)
            {
                if (filter.Name.ToUpper().Contains(filters.Search.ToUpper()))
                {
                    var prevBg = GUI.backgroundColor;
                    GUI.backgroundColor = filter.Color;
                    Vector2 textDimension = skin.GetStyle("Filter").CalcSize(new GUIContent(filter.Name));
                    GUILayout.Label("Found Filters: ");
                    if (GUILayout.Button(filter.Name, skin.GetStyle("Filter"),GUILayout.Width(150), GUILayout.Width(textDimension.x)))
                    {
                        filters.Filters.Add(filter);
                        filters.Search = string.Empty;
                    }
                    GUI.backgroundColor = prevBg;
                }
            }
        }

        if (filters.Filters.Count > 0)
        {
            GUILayout.Label("Filters:");
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(150));
            foreach (Filter appliedFilter in filters.Filters)
            {
                Vector2 textDimension = skin.GetStyle("Filter").CalcSize(new GUIContent(appliedFilter.Name));
                var prevBg = GUI.backgroundColor;
                GUI.backgroundColor = appliedFilter.Color;
                if (GUILayout.Button(appliedFilter.Name, skin.GetStyle("Filter"),GUILayout.Width(textDimension.x)))
                {
                    filters.Filters.Remove(appliedFilter);
                    break;
                }
                GUI.backgroundColor = prevBg;
            }
            GUILayout.EndHorizontal();
        }
        filters.Search.Trim();
        return filters;
    }

    /// <summary>
    /// Creates a <see cref="SerializedObject"/> list of controllers when <see cref="EditorFilters"/> are applied.
    /// </summary>
    /// <param name="controllerPool">Pool of controllers that should be filtered.</param>
    /// <param name="filters"></param>
    /// <returns>Filtered List.</returns>
    private static List<SerializedObject> FilterControllers(SerializedObject[] controllerPool, EditorFilters filters)
    {
        List<SerializedObject> filteredControllers = new List<SerializedObject>();
        List<SerializedObject> searchedControllers = new List<SerializedObject>();
        if (filters.Filters.Count > 0)
        {
            foreach (SerializedObject controller in controllerPool)
            {
                var objFilters = (controller.targetObject as RuleController).Profile.MyFilters;
                foreach (Filter appliedFilter in filters.Filters)
                {
                    if (objFilters.Filters.Contains(appliedFilter))
                    {
                        filteredControllers.Add(controller);
                        break;
                    }
                }
            }
        }
        if(filters.Search.Length > 0)
        {
            List<SerializedObject> controllersToSearch = new List<SerializedObject>();
            // if no filter. then search through all controllers
            if(filteredControllers.Count == 0)
            {
                controllersToSearch.AddRange(controllerPool);
            }
            else
            {
                controllersToSearch.AddRange(filteredControllers);
                filteredControllers.Clear();
            }    
            foreach (SerializedObject controller in controllersToSearch)
            {
                if(controller.targetObject.name.ToUpper().Contains(filters.Search.ToUpper()))
                {
                    searchedControllers.Add(controller);
                }
            }
        }
        if (filteredControllers.Count > 0)
        {
            return filteredControllers;
        }
        if(searchedControllers.Count> 0)
        {
            return searchedControllers;
        }
        else if(filters.Filters.Count > 0 || filters.Search.Length > 0 )
        { 
            return new List<SerializedObject>(); 
        }
        else
        {
            return new List<SerializedObject>(controllerPool);
        }
    }
    
    /// <summary>
    /// Creates formated ruleset displays.
    /// </summary>
    /// <param name="ruleSetArray"></param>
    /// <param name="anchor"></param>
    /// <param name="skin"></param>
    /// <returns></returns>
    private static SerializedProperty DrawRuleSetLayout(SerializedProperty ruleSetArray, Vector2 anchor, ref SessionData data ,GUISkin skin)
    {
        Rect mySetRect = new Rect(anchor, WindowUtils.RuleSetLayout.size);
        GUILayout.BeginArea(mySetRect, skin.box);
        GUILayout.BeginHorizontal(new GUIContent(string.Format(ruleTitleFormat, ruleSetArray.displayName, ruleSetArray.arraySize)),skin.GetStyle("Header"));


        if (GUILayout.Button(new GUIContent("", "Add new Rule"),skin.GetStyle("AddBtn")))
        {
            ruleSetArray.InsertArrayElementAtIndex(ruleSetArray.arraySize);
            ClearRuleProperty(ruleSetArray.GetArrayElementAtIndex(ruleSetArray.arraySize - 1));
            ruleSetArray.serializedObject.ApplyModifiedProperties();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(2);

        for (int i = 0; i < ruleSetArray.arraySize; i++)
        {
            var prevBG = GUI.backgroundColor;
            if(data.selectedRule != null && SerializedProperty.EqualContents(data.selectedRule, ruleSetArray.GetArrayElementAtIndex(i)))
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button(ruleSetArray.GetArrayElementAtIndex(i).FindPropertyRelative("RuleName").stringValue, skin.GetStyle("Btn2"), GUILayout.Height(25)))
            {
                return ruleSetArray.GetArrayElementAtIndex(i);
            }
            GUI.backgroundColor = prevBG;
        }
        GUILayout.EndArea();



        return null;
    }

    /// <summary>
    /// Draws layout for Profile data tab.
    /// </summary>
    /// <param name="displayProfile">Profile to display. </param>
    /// <param name="skin"><see cref="GUISkin"/> of area layout.</param>
    private static void ProfileData(SerializedObject displayProfile, GUISkin skin)
    {
        GUILayout.Label("Profile info: ");
        GUILayout.Label("", skin.GetStyle("HorizontalDivider"));
        GUILayout.Space(10);
        GUILayout.BeginHorizontal(GUILayout.Height(20));
        GUILayout.Label("Ruled object name: ");
        displayProfile.targetObject.name = EditorGUILayout.TextField(displayProfile.targetObject.name);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Set Profile type: ");
        (displayProfile.targetObject as RuleController).Profile.MyFilters.Type = (TypeFilter)EditorGUILayout.EnumPopup((displayProfile.targetObject as RuleController).Profile.MyFilters.Type);
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Add Filters");
        (displayProfile.targetObject as RuleController).Profile.MyFilters = DrawFilterableSearchbar(((displayProfile.targetObject as RuleController).Profile.MyFilters),RuleEditorSettings.Instance.ProfileFilters, skin);
        GUILayout.Label("", skin.GetStyle("HorizontalDivider"), GUILayout.Height(2));
        GUILayout.Space(10);
        GUILayout.EndVertical();
        string absolute = Application.dataPath.Replace("Assets", "");
        if (GUILayout.Button("Reset"))
        {
            if(EditorUtility.DisplayDialog("Reset profile.", "Do you realy want to reset the entire profile?", "Reset Profile."))
            {
                (displayProfile.targetObject as RuleController).Profile = new BehaviourProfile();
                displayProfile.ApplyModifiedProperties();
            }
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save as Preset"))
        {
            var path = EditorUtility.SaveFilePanel("Save this profile as Preset", absolute + WindowUtils.ProfilePresetFolder, displayProfile.targetObject.name + "'s profile", "profile");
            if (path != "")
            {
                var json = JsonUtility.ToJson((displayProfile.targetObject as RuleController).Profile);
                File.WriteAllText(path, json);
            }
        }
        if(GUILayout.Button("Load from Preset"))
        {

            var path = EditorUtility.OpenFilePanel("Load a profile from preset", absolute + WindowUtils.ProfilePresetFolder, "profile");
            if (path != "")
            {
                var json = File.ReadAllText(path);
                (displayProfile.targetObject as RuleController).Profile = JsonUtility.FromJson<BehaviourProfile>(json);
                displayProfile.ApplyModifiedProperties();
                displayProfile.Update();
            }
        }
        GUILayout.EndHorizontal();
    }
    /// <summary>
    /// Draws layout for selected rule tab.
    /// </summary>
    /// <param name="rule">Selected rule property. </param>
    /// <param name="data"><see cref="SessionData"/> refrence of an editorwindow.</param>
    /// <param name="OnOpenRule">Action-Event handle for opening <see cref="RuleWindow2"/>.</param>
    /// <param name="skin"><see cref="GUISkin"/> of area layout.</param>
    /// <returns></returns>
    private static SerializedProperty EditRuleData(SerializedProperty rule, ref SessionData data,Action<SerializedProperty> OnOpenRule ,GUISkin skin)
    {
        string[] parentPath = rule.propertyPath.Remove(rule.propertyPath.IndexOf("Array") - 1).Split('.');
        int index = int.Parse(rule.propertyPath.Substring(rule.propertyPath.IndexOf('[') + 1).Replace("]", ""));

        GUILayout.Label("Rule Info:");
        GUILayout.Label("", skin.GetStyle("HorizontalDivider"));
        GUILayout.Space(10);
        GUILayout.BeginHorizontal(GUILayout.Height(20));
        GUILayout.Label("RuleName: ");
        rule.FindPropertyRelative("RuleName").stringValue = GUILayout.TextField(rule.FindPropertyRelative("RuleName").stringValue);
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Description:");
        rule.FindPropertyRelative("Description").stringValue = GUILayout.TextArea(rule.FindPropertyRelative("Description").stringValue, GUILayout.Height(60));
        GUILayout.EndVertical();
        GUILayout.Space(10);
        GUILayout.Label("Edit Options");
        GUILayout.Label("", skin.GetStyle("HorizontalDivider"));
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Open Rule"))
        {
            //OpenRuleWIndow
            OnOpenRule.Invoke(rule);

        }
        if(GUI.changed)
        {
            data.RuleEditUnsaved = true;
        }
        if(data.RuleEditUnsaved)
        {
            if (GUILayout.Button("Save Rule"))
            {
                rule.serializedObject.ApplyModifiedProperties();
                data.RuleEditUnsaved = false;
            }
        }
        if(GUILayout.Button("Remove Rule"))
        {
            if(EditorUtility.DisplayDialog("Removing this Rule", "Are You shure you want to remove this rule? Changes can't be undone u know.","Remove Rule"))
            {
                SerializedProperty ruleArray = rule.serializedObject.FindProperty(parentPath[0]).FindPropertyRelative(parentPath[1]);
                ruleArray.DeleteArrayElementAtIndex(index);
                rule.serializedObject.ApplyModifiedProperties();
                return null;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        string absolute = Application.dataPath.Replace("Assets", "");
        if (GUILayout.Button("Save as Preset"))
        {
            // Save Rule
            rule.serializedObject.ApplyModifiedProperties();
            data.RuleEditUnsaved = false;
            // json

            var filePath = EditorUtility.SaveFilePanel("Save rule as preset", absolute + WindowUtils.RulePresetFolder, rule.displayName, "RulePreset");
            WindowUtils.RulePresetFolder = filePath.Replace(rule.displayName + ".json", "");
            string jsonRule = JsonUtility.ToJson(RuleSystemUtil.DeserializeRule(rule, parentPath[1], index));
            File.WriteAllText(filePath, jsonRule);
        }
        if (GUILayout.Button("Load from Preset"))
        {
            //fromjson
            var filepath = EditorUtility.OpenFilePanel("Load rule from preset", absolute + WindowUtils.RulePresetFolder, "RulePreset");
            string jsonRule = File.ReadAllText(filepath);
            Rule myRule = JsonUtility.FromJson<Rule>(jsonRule);
            RuleSystemUtil.SerializeRule(myRule, rule, parentPath[1], index);
            rule.serializedObject.ApplyModifiedProperties();
            rule.serializedObject.Update();
        }
        GUILayout.EndHorizontal();
        return rule;

    }

    /// <summary>
    /// Clears out ruleProperty so new rule isn't duplicate of last rule in set.
    /// </summary>
    /// <param name="ruleProperty">Rule property to clear. </param>
    private static void ClearRuleProperty(SerializedProperty ruleProperty)
    {
        ruleProperty.FindPropertyRelative("RuleName").stringValue = "New Rule";
        ruleProperty.FindPropertyRelative("Description").stringValue = "";
        ruleProperty.FindPropertyRelative("RootGridPosition").vector2Value = Vector2.zero;
        ruleProperty.FindPropertyRelative("ActionGridPosition").vector2Value = Vector2.zero;
        ruleProperty.FindPropertyRelative("QualityId").intValue = -1;
        ruleProperty.FindPropertyRelative("MandatoryId").intValue = -1;
        ruleProperty.FindPropertyRelative("MyDecisions").ClearArray();
        ruleProperty.FindPropertyRelative("MyAction").objectReferenceValue = null;
        ruleProperty.FindPropertyRelative("Quality").floatValue = -1f;
    }
}
