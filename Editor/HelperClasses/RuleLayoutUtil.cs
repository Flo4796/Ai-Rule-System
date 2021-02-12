using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdelicSystem.RuleAI.Editor
{
    /// <summary>
    /// Represents Draw functions for all layout area's for specificly <see cref="RuleWindow2"/>.
    /// </summary>
    public static class RuleLayoutUtil
    {
        /// <summary>
        /// Function drawing GUILayout of available <see cref="Statement"/> and/or <see cref="Action"/> assets within project.
        /// </summary>
        /// <param name="statementLibrary">Sorted collection of captured Statements. </param>
        /// <param name="OnAddStatementRequest">Action-Event handle for adding Statment node. </param>
        /// <param name="actionLibrary">Sorted collection of captured Actions. </param>
        /// <param name="OnSetActionRequest">Action-Event handle for adding Action node. </param>
        /// <param name="ruleProperty">Rule property. </param>
        /// <param name="layoutArea">Rectangle representing Layout area. </param>
        /// <param name="data"><see cref="SessionData"/>refrence from an editorwindow. </param>
        /// <param name="skin">Node <see cref="GUISkin"/> refrence. </param>
        public static void DrawContentLibraryLayout(Dictionary<StatementType, List<Statement>> statementLibrary, Action<Statement> OnAddStatementRequest,
                                                     Dictionary<ActionType, List<Action>> actionLibrary, Action<Action> OnSetActionRequest, SerializedProperty ruleProperty,
                                                     Rect layoutArea, ref SessionData data, GUISkin skin)
        {
            GUILayout.BeginArea(layoutArea, skin.window);
            GUILayout.Space(10);
            data.ToolbarIndex = GUILayout.Toolbar(data.ToolbarIndex, new string[] { "Rule Info", "Statements", "Actions" });

            switch (data.ToolbarIndex)
            {
                case 0:
                    // draw rule Info.
                    drawRuleInfo(ruleProperty, skin);
                    break;
                case 1:
                    // draw statements.
                    drawStatementLibrary(statementLibrary, OnAddStatementRequest, ref data, skin);
                    break;
                case 2:
                    // draw actions.
                    drawActionLibrary(actionLibrary, OnSetActionRequest, ref data, skin);
                    break;
            }

            GUILayout.EndArea();
        }


        /// <summary>
        /// Drawing GUILayout for available Statements.
        /// </summary>
        /// <param name="statementLibrary">Sorted captured statements. </param>
        /// <param name="OnAddStatementRequest">Action-Event handle for adding Statment node.</param>
        /// <param name="data"></param>
        /// <param name="skin"></param>
        private static void drawStatementLibrary(Dictionary<StatementType, List<Statement>> statementLibrary, Action<Statement> OnAddStatementRequest, ref SessionData data, GUISkin skin)
        {
            foreach (StatementType CategoryOfStatement in statementLibrary.Keys)
            {
                if (!data.isStatementTypeToggled.ContainsKey(CategoryOfStatement)) { data.isStatementTypeToggled.Add(CategoryOfStatement, false); }
                data.isStatementTypeToggled[CategoryOfStatement] = GUILayout.Toggle(data.isStatementTypeToggled[CategoryOfStatement],
                    CategoryOfStatement.ToString(), skin.toggle, GUILayout.Height(15));
                GUILayout.Space(5);
                if (data.isStatementTypeToggled[CategoryOfStatement])
                {
                    foreach (Statement statement in statementLibrary[CategoryOfStatement])
                    {
                        if (GUILayout.Button(new GUIContent(statement.Name, statement.Description), skin.button, GUILayout.Height(20)))
                        {
                            // sent Request
                            OnAddStatementRequest.Invoke(statement);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Drawing GUILayout for available Actions. 
        /// </summary>
        /// <param name="actionLibrary">Sorted captured actions. </param>
        /// <param name="OnSetActionRequest">Action-Event handle for adding Action node</param>
        /// <param name="data"></param>
        /// <param name="skin"></param>
        private static void drawActionLibrary(Dictionary<ActionType, List<Action>> actionLibrary, Action<Action> OnSetActionRequest, ref SessionData data, GUISkin skin)
        {
            foreach (ActionType CategoryOfAction in actionLibrary.Keys)
            {
                if (!data.isActionTypeToggled.ContainsKey(CategoryOfAction)) { data.isActionTypeToggled.Add(CategoryOfAction, false); }
                data.isActionTypeToggled[CategoryOfAction] = GUILayout.Toggle(data.isActionTypeToggled[CategoryOfAction],
                    CategoryOfAction.ToString(), skin.toggle, GUILayout.Height(15));
                GUILayout.Space(5);
                if (data.isActionTypeToggled[CategoryOfAction])
                {
                    foreach (Action action in actionLibrary[CategoryOfAction])
                    {
                        if (GUILayout.Button(new GUIContent(action.Name, action.Description), skin.button, GUILayout.Height(20)))
                        {
                            // sent request
                            OnSetActionRequest(action);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Draws info properties of the rule.
        /// </summary>
        /// <param name="ruleProperty">Current open Rule</param>
        /// <param name="skin"></param>
        private static void drawRuleInfo(SerializedProperty ruleProperty, GUISkin skin)
        {
            GUILayout.Space(5);
            GUILayout.Label("Name: ");
            ruleProperty.FindPropertyRelative("RuleName").stringValue = GUILayout.TextField(ruleProperty.FindPropertyRelative("RuleName").stringValue);
            GUILayout.Space(5);
            GUILayout.Label("Description: ");
            ruleProperty.FindPropertyRelative("Description").stringValue = GUILayout.TextArea(ruleProperty.FindPropertyRelative("Description").stringValue, GUILayout.Height(50));
            ruleProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}