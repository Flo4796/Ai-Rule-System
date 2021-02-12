
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdelicSystem.RuleAI.Editor
{
    public class EditorLabelManager : EditorWindow
    {
        public GUISkin AdelicSkin;

        private void OnEnable()
        {
            this.minSize = new Vector2(250f, 300f);
            maxSize = minSize;
        }

        private void OnGUI()
        {
            GUILayout.Label("Profile");
            if (RuleEditorSettings.Instance.ProfileFilters != null)
            {
                int labelToRemove = -1;
                for (int i = 0; i < RuleEditorSettings.Instance.ProfileFilters.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    var bgPrev = GUI.backgroundColor;
                    GUI.backgroundColor = RuleEditorSettings.Instance.ProfileFilters[i].Color;
                    RuleEditorSettings.Instance.ProfileFilters[i].Name = GUILayout.TextField(RuleEditorSettings.Instance.ProfileFilters[i].Name, AdelicSkin.textField, GUILayout.Width(200));
                    GUI.backgroundColor = bgPrev;
                    RuleEditorSettings.Instance.ProfileFilters[i].Color = EditorGUILayout.ColorField(new GUIContent(), RuleEditorSettings.Instance.ProfileFilters[i].Color, false, true, false, GUILayout.Width(15));
                    if (GUILayout.Button(new GUIContent("", "Remove Label"), AdelicSkin.GetStyle("RemoveBtn")))
                    {
                        labelToRemove = i;
                    }
                    GUILayout.EndHorizontal();
                }
                if (labelToRemove > -1)
                {
                    ArrayUtility.RemoveAt<Filter>(ref RuleEditorSettings.Instance.ProfileFilters, labelToRemove);
                }
            }
            if (GUILayout.Button("+"))
            {
                ArrayUtility.Add<Filter>(ref RuleEditorSettings.Instance.ProfileFilters, new Filter());

            }
            RuleEditorSettings.Instance.Save();
        }

    }
}
