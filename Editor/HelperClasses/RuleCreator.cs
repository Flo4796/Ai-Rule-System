using System;
using UnityEngine;

namespace AdelicSystem.RuleAI.Editor
{

    /// <summary>
    /// Represents static helper class for creating <see cref="NodeShell"/> within <see cref="RuleWindow2"/.>
    /// </summary>
    public static class RuleCreator
    {
        /// <summary>
        /// Creates a Root node.
        /// </summary>
        /// <param name="position">Grid position. </param>
        /// <param name="OnPortClick">Event-handle for <see cref="Port"/> clicks. </param>
        /// <param name="OnRootUpdate">Event-handle for updating <see cref="Rule"/> data. </param>
        /// <param name="mandatoryID">Representing Mandatory <see cref="Decision"/> identifier. </param>
        /// <param name="QualityID">Representing Wuality decision identifier. </param>
        /// <param name="skin"><see cref="GUISkin"/> of nodes. </param>
        /// <returns></returns>
        public static RootShell CreateRoot(Vector2 position, Action<Port> OnPortClick, Action<int, int, Vector2> OnRootUpdate, Action<Thread> OnRemoveThread, int mandatoryID, int QualityID, Action action, GUISkin skin)
        {
            RootNodeStyle style = new RootNodeStyle("Root");
            RootShell root = new RootShell
            {
                Id = -1,
                Style = style,
                Rect = style.Rect,
                Skin = skin,
                Action = action,
                OnUpdateRoot = OnRootUpdate
            };
            root.Rect.position = position;
            root.MandatoryID = mandatoryID;
            root.QualityID = QualityID;
            root.Port0 = new Port
            {
                Name = "Mandatory",
                type = PortType.Input,
                MyNode = root,
                nodeSkin = skin,
                OnClickPort = OnPortClick,
                OnRemoveThread = OnRemoveThread,
                rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
                isDecision = true
            };
            root.Port1 = new Port
            {
                Name = "Quality",
                type = PortType.Input,
                MyNode = root,
                nodeSkin = skin,
                OnClickPort = OnPortClick,
                OnRemoveThread = OnRemoveThread,
                rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
                isDecision = true
            };
            root.Port2 = new Port
            {
                Name = "Action",
                type = PortType.Input,
                MyNode = root,
                nodeSkin = skin,
                OnClickPort = OnPortClick,
                OnRemoveThread = OnRemoveThread,
                rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
                isDecision = false
            };
            return root;
        }

        /// <summary>
        /// Creates an Action node.
        /// </summary>
        /// <param name="position">Grid position. </param>
        /// <param name="OnPortClick">Event-handle for <see cref="Port"/> clicks. </param>
        /// <param name="OnActionUpdate">Event-hanlde for updating <see cref="Action"/> data. </param>
        /// <param name="OnRemoveNode">Event-handle for removing this node. </param>
        /// <param name="action">Action asset represented by this node. </param>
        /// <param name="skin"><see cref="GUISkin"/> of nodes. </param>
        /// <returns></returns>
        public static ActionShell CreateActionNode(Vector2 position, Action<Port> OnPortClick, Action<Action, Vector2> OnActionUpdate, Action<NodeShell> OnRemoveNode, Action<Thread> OnRemoveThread, Action action, GUISkin skin)
        {
            ActionStyle style = new ActionStyle(action.Name);
            ActionShell crAct = new ActionShell
            {
                Id = -1,
                Action = action,
                OnUpdateAction = OnActionUpdate,
                OnRemoveNode = OnRemoveNode,
                Skin = skin,
                Style = style,
                Rect = style.Rect
            };
            crAct.Rect.position = position;
            crAct.Port0 = new Port
            {
                Name = action.Name,
                type = PortType.Output,
                MyNode = crAct,
                nodeSkin = skin,
                OnClickPort = OnPortClick,
                OnRemoveThread = OnRemoveThread,
                rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
                isDecision = false
            };
            return crAct;
        }
        /// <summary>
        /// Creates a node (specificly used for <see cref="Decision"/> nodes).
        /// </summary>
        /// <param name="id">Decision identifier. </param>
        /// <param name="statement">Decision asset represented by this node. </param>
        /// <param name="OnRuleUpdate">Event-handle for updating <see cref="Decision"/> data. </param>
        /// <param name="OnPortClick">Event-handle for <see cref="Port"/> clicks. </param>
        /// <param name="OnRemoveNode">Event-handle for removing this node. </param>>
        /// <param name="skin"><see cref="GUISkin"/> of nodes. </param>
        /// <returns></returns>
        public static NodeShell CreateNewDecisionNode(int id, Statement statement, Action<int, Statement, int[], NodeShell.Data, Vector2> OnRuleUpdate, Action<Port> OnPortClick, Action<NodeShell> OnRemoveNode, Action<Thread> OnRemoveThread, GUISkin skin)
        {
            NodeShell crNode = new NodeShell
            {
                Id = id,
                Asset = statement,
                Inputs = new int[0],
                Container = new NodeShell.Data(),
                Skin = skin,
                OnUpdateRule = OnRuleUpdate,
                OnRemoveNode = OnRemoveNode
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
                    crNode.Port0 = new Port
                    {
                        type = PortType.Input,
                        Name = statement.Name,
                        MyNode = crNode,
                        nodeSkin = skin,
                        OnClickPort = OnPortClick,
                        OnRemoveThread = OnRemoveThread,
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
                    crNode.Port0 = new Port
                    {
                        type = PortType.Input,
                        Name = statement.Name,
                        MyNode = crNode,
                        nodeSkin = skin,
                        OnClickPort = OnPortClick,
                        OnRemoveThread = OnRemoveThread,
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
            crNode.Port1 = new Port
            {
                Name = statement.Name,
                type = PortType.Output,
                MyNode = crNode,
                nodeSkin = skin,
                Color = crNode.Style.Color,
                OnClickPort = OnPortClick,
                OnRemoveThread = OnRemoveThread,
                rect = new Rect(0, 0, skin.button.fixedWidth, skin.button.fixedHeight),
                isDecision = true
            };
            return crNode;
        }

        /// <summary>
        /// Swiches decision nodes existing style.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="node"></param>
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
                    if (statement.GetType() == typeof(FlatValueStatement))
                        node.Style = new GenNodeStyle("Flat Value");
                    break;
                case StatementType.Mutator:
                    break;
                case StatementType.Evaluation:
                    break;
            }
        }
    }
}