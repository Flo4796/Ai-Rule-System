using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RuleCreator
{
    public static RootShell CreateRoot(Vector2 position, Action<Port2> OnPortClick, Action<int,int,Vector2> OnRootUpdate ,int mandatoryID, int QualityID, GUISkin skin)
    {
        RootNodeStyle style = new RootNodeStyle("Root");
        RootShell root = new RootShell
        {
            Id = -1,
            Style = style,
            Rect = style.Rect,
            Skin = skin,
            OnUpdateRoot = OnRootUpdate
        };
        root.Rect.position = position;
        root.MandatoryID = mandatoryID;
        root.QualityID = QualityID;
        root.Port0 = new Port2
        {
            type = PortType.Input,
            MyNode = root,
            nodeSkin = skin,
            OnClickPort = OnPortClick,
            rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
            isDecision = true
        };
        root.Port1 = new Port2
        {
            type = PortType.Input,
            MyNode = root,
            nodeSkin = skin,
            OnClickPort = OnPortClick,
            rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
            isDecision = true
        };
        root.Port2 = new Port2
        {
            type = PortType.Input,
            MyNode = root,
            nodeSkin = skin,
            OnClickPort = OnPortClick,
            rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
            isDecision = false
        };
        return root;
    }

    public  static ActionShell CreateActionNode(Vector2 position, Action<Port2> OnPortClick, Action<Action, Vector2> OnActionUpdate,Action action ,GUISkin skin)
    {
        ActionStyle style = new ActionStyle(action.Name);
        ActionShell crAct = new ActionShell
        {
            Id = -1,
            Action = action,
            OnUpdateAction = OnActionUpdate,
            Skin = skin,
            Style = style,
            Rect = style.Rect
        };
        crAct.Rect.position = position;
        crAct.Port0 = new Port2
        {
            type = PortType.Output,
            MyNode = crAct,
            nodeSkin = skin,
            OnClickPort = OnPortClick,
            rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
            isDecision = false
        };
        return crAct;
    }
    public static NodeShell CreateNewDecisionNode(int id, Statement statement, Action<int, Statement, int[], NodeShell.Data, Vector2> OnRuleUpdate, Action<Port2> OnPortClick, GUISkin skin)
    {
        NodeShell crNode = new NodeShell
        {
            Id = id,
            Asset = statement,
            Inputs = new int[0],
            Container = new NodeShell.Data(),
            Skin = skin,
            OnUpdateRule = OnRuleUpdate
        };
        switch (statement.Type)
        {
            case StatementType.unknown:
                break;
            case StatementType.Inequality:
            case StatementType.Gate:
                BoolNodeStyle boolStyle = new BoolNodeStyle(statement.Name, statement.Type);
                crNode.Style = boolStyle;
                crNode.Rect = boolStyle.Rect;
                crNode.CurrentExpanedHeight = boolStyle.MinExpandedHeight;
                crNode.Port0 = new Port2
                {
                    type = PortType.Input,
                    MyNode = crNode,
                    nodeSkin = skin,
                    OnClickPort = OnPortClick,
                    rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
                    isDecision = true
                };
                break;
            case StatementType.Generator:
                GenNodeStyle genStyle = new GenNodeStyle("FlatValue");
                crNode.Style = genStyle;
                crNode.Rect = genStyle.Rect;
                crNode.CurrentExpanedHeight = genStyle.MinExpandedHeight;
                break;

            case StatementType.Mutator:
                MutNodeStyle mutStyle = new MutNodeStyle(statement.Name);
                crNode.Style = mutStyle;
                crNode.Rect = mutStyle.Rect;
                crNode.CurrentExpanedHeight = mutStyle.MinExpandedHeight;
                crNode.Port0 = new Port2
                {
                    type = PortType.Input,
                    MyNode = crNode,
                    nodeSkin = skin,
                    OnClickPort = OnPortClick,
                    rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight)
                };
                break;
            case StatementType.Evaluation:
                EvNodeStyle evStyle = new EvNodeStyle(statement.name);
                crNode.Style = evStyle;
                crNode.Rect = evStyle.Rect;
                crNode.CurrentExpanedHeight = evStyle.MinExpandedHeight;
                break;
        }
        crNode.Port1 = new Port2
        {
            type = PortType.Output,
            MyNode = crNode,
            nodeSkin = skin,
            Color = crNode.Style.Color,
            OnClickPort = OnPortClick,
            rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
            isDecision = true
        };
        return crNode;
    }

    public static void ChangeStyle(Statement statement, ref NodeShell node)
    {
        switch (statement.Type)
        {
            case StatementType.unknown:
                break;
            case StatementType.Inequality:
            case StatementType.Gate:
                node.Style = new BoolNodeStyle(statement.Name, statement.Type);
                break;
            case StatementType.Generator:
                if(statement.GetType() == typeof(FlatValueStatement))
                    node.Style = new GenNodeStyle("Flat Value");
                break;
            case StatementType.Mutator:
                break;
            case StatementType.Evaluation:
                break;
        }
    }
}
