using UnityEditor;
using UnityEngine;

namespace AdelicSystem.RuleAI.Editor
{
    [CustomPropertyDrawer(typeof(Rule))]
    public class RuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            string ruleName = property.FindPropertyRelative("RuleName").stringValue;

            if (GUI.Button(position,new GUIContent(string.Format("Open Rule ({0})", ruleName), "Open this rule in RuleSystem Editor")))
            {
                RuleWindow2.RequestRuleWindow(property, false);
            }

            EditorGUI.EndProperty();
        }
    }
}
