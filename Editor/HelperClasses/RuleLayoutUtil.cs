using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RuleLayoutUtil
{
    public static void DrawContentLibraryLayout(Dictionary<StatementType, List<Statement>> statementLibrary, Action<Statement> OnAddStatementRequest,
                                                 Dictionary<ActionType, List<Action>> actionLibrary, Action<Action> OnSetActionRequest,
                                                 Rect layoutArea, ref SessionData data, GUISkin skin)
    {
        GUILayout.BeginArea(layoutArea, skin.window);
        GUILayout.Space(10);
        data.ToolbarIndex = GUILayout.Toolbar(data.ToolbarIndex, new string[] { "Rule Info", "Statements", "Actions" });

        switch (data.ToolbarIndex)
        {
            case 1:
                // draw statements.
                drawStatementLibrary(statementLibrary, OnAddStatementRequest,ref data, skin);
                break;
                case 2:
                // draw actions.
                drawActionLibrary(actionLibrary, OnSetActionRequest, ref data, skin);
                break;
        }

        GUILayout.EndArea();
    }


    private static void drawStatementLibrary(Dictionary<StatementType, List<Statement>> statementLibrary, Action<Statement> OnAddStatementRequest,ref SessionData data, GUISkin skin)
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

   private static void drawActionLibrary(Dictionary<ActionType, List<Action>> actionLibrary, Action<Action> OnSetActionRequest, ref SessionData data, GUISkin skin)
    {
        foreach (ActionType CategoryOfAction in actionLibrary.Keys)
        {
            if (!data.isActionTypeToggled.ContainsKey(CategoryOfAction)){ data.isActionTypeToggled.Add(CategoryOfAction, false); }
            data.isActionTypeToggled[CategoryOfAction] = GUILayout.Toggle(data.isActionTypeToggled[CategoryOfAction], 
                CategoryOfAction.ToString(), skin.toggle, GUILayout.Height(15));
            GUILayout.Space(5);
            if(data.isActionTypeToggled[CategoryOfAction])
            {
                foreach (Action action in actionLibrary[CategoryOfAction])
                {
                    if(GUILayout.Button(new GUIContent(action.Name, action.Description), skin.button, GUILayout.Height(20)))
                    {
                        // sent request
                        OnSetActionRequest(action);
                    }
                }
            }
        }
    }
}
