using System;
using UnityEditor;
using UnityEngine;

namespace AdelicSystem.RuleAI.Editor
{

    /// <summary>
    /// Represents base nodes on a <see cref="RuleWindow2"/>.
    /// </summary>
    public class NodeShell
    {
        /// <summary>
        /// Struct containing instance-specific data in decision/action nodes.
        /// </summary>
        public struct Data
        {
            public float FloatValue;
        }

        #region General Fields
        public bool isDragged;
        public bool isSelected;
        public Rect Rect;
        public float CurrentExpanedHeight;
        public NodeStyle Style;
        public GUISkin Skin;
        public bool Toggle;
        public Data Container;
        public Port Port0;
        public Port Port1;
        public Port Port2;
        public Action<int, Statement, int[], Data, Vector2> OnUpdateRule;
        public Action<NodeShell> OnRemoveNode;
        public bool activated = false;
        #endregion

        #region Decision Fields
        public int Id;
        public Statement Asset;
        public int[] Inputs;
        #endregion


        /// <summary>
        /// Draw GUILayout of this node.
        /// </summary>
        public virtual void Draw()
        {
            if (!activated) { return; }
            Color GUIBackground = GUI.backgroundColor;
            GUI.backgroundColor = Style.Color;
            if (Asset != null)
            {
                Rect.height = Toggle ? CurrentExpanedHeight : Style.CollapsedHeight;
                GUILayout.BeginArea(Rect, Skin.box);

                Toggle = GUILayout.Toggle(Toggle, new GUIContent(Asset.Name, Asset.Description), Skin.toggle, GUILayout.Height(20));
                if (Toggle)
                {
                    // Displaying and Selecting Statement
                    if (GUILayout.Button(Asset.name, Skin.toggle))
                    {
                        EditorGUIUtility.ShowObjectPicker<Statement>(Asset, false, "", 0);
                    }
                    if (EditorGUIUtility.GetObjectPickerObject() != null)
                    {
                        Asset = (Statement)EditorGUIUtility.GetObjectPickerObject();
                    }

                    // Handle type specific data
                    switch (Style.Type)
                    {
                        case StatementType.unknown:
                            Debug.Log(Id + "Type not set!");
                            break;
                        case StatementType.Gate:
                            break;
                        case StatementType.Inequality:
                            break;
                        case StatementType.Generator:
                            if (Asset.GetType() == typeof(FlatValueStatement))
                            {
                                GUILayout.Label("FlatValue: ");
                                Container.FloatValue = EditorGUILayout.FloatField(Container.FloatValue);
                            }
                            break;
                        case StatementType.Mutator:

                            break;
                        case StatementType.Evaluation:
                            break;
                    }

                }
                GUILayout.EndArea();
            }
            // input
            if (Port0 != null)
            {
                Port0.rect.y = Rect.y + (Rect.height * 0.5f) - Port0.rect.height * 0.5f;
                Port0.rect.y = Rect.y + Rect.height / 2f;
                Port0.rect.x = Rect.x - Port0.rect.width + 16f;
                Port0.Draw();
            }
            //output
            if (Port1 != null)
            {
                Port1.rect.y = Rect.y + (Rect.height * 0.5f) - Port1.rect.height * 0.5f;
                Port1.rect.y = Rect.y + Rect.height / 2f;
                Port1.rect.x = Rect.x + Rect.width - 16f;
                Port1.Draw();
            }


            GUI.backgroundColor = GUIBackground;
            // Update 

            // Collect input connections
            if (Port0 != null)
            {
                Inputs = Port0.GetConnectionIDs();

            }

            //sent
            OnUpdateRule(Id, Asset, Inputs, Container, Rect.position);
        }


        /// <summary>
        /// Dragging calculation of node.
        /// </summary>
        /// <param name="delta">Mouse delta position.</param>
        public void Drag(Vector2 delta)
        {
            Rect.position += delta;

        }

        /// <summary>
        /// Processes Unity events for this specific node.
        /// </summary>
        /// <param name="e">Current Unity Event.</param>
        /// <returns>Changed GUI data.</returns>
        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (Rect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            GUI.changed = true;
                            isSelected = true;

                        }
                        else
                        {
                            GUI.changed = true;
                            isSelected = false;

                        }
                    }
                    if (e.button == 1)
                    {
                        if (Rect.Contains(e.mousePosition) && OnRemoveNode != null)
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Remove", "Removes Node from rule"), false, () => OnRemoveNode(this));
                            menu.ShowAsContext();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 & isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }
    }

    /// <summary>
    /// Derrived node for <see cref="Action"/> assets.
    /// </summary>
    public class ActionShell : NodeShell
    {
        public Action<Action, Vector2> OnUpdateAction;
        public Action Action;

        public override void Draw()
        {
            if (!activated) { return; }
            Color GUIBackground = GUI.backgroundColor;
            GUI.backgroundColor = Style.Color;
            GUILayout.BeginArea(Rect, Skin.box);
            GUILayout.Label("Action: " + Action.Name);

            if (GUILayout.Button(Action.name, Skin.toggle))
            {
                EditorGUIUtility.ShowObjectPicker<Action>(Action, false, "", 0);
            }
            if (EditorGUIUtility.GetObjectPickerObject() != null)
            {
                Action = (Action)EditorGUIUtility.GetObjectPickerObject();
            }

            GUILayout.EndArea();
            GUI.backgroundColor = GUIBackground;
            if (Port0 != null)
            {
                Port0.rect.y = Rect.y + (Rect.height * 0.5f) - Port0.rect.height * 0.5f;
                Port0.rect.y = Rect.y + Rect.height / 2f;
                Port0.rect.x = Rect.x - Port0.rect.width + 16f;
                Port0.Draw();
            }

            // update
            OnUpdateAction(Action, Rect.position);
        }
    }

    /// <summary>
    /// Derrived node representing <see cref="Rule"/> meta data as node.
    /// </summary>
    public class RootShell : NodeShell
    {
        public Action<int, int, Vector2> OnUpdateRoot;
        public int MandatoryID;
        public int QualityID;
        public Action Action;
        public override void Draw()
        {
            if (!activated) { return; }
            Color GUIBackground = GUI.backgroundColor;
            GUI.backgroundColor = Style.Color;
            GUILayout.BeginArea(Rect, Skin.box);
            GUILayout.Label("Root");
            GUILayout.EndArea();
            GUI.backgroundColor = GUIBackground;
            //mandatory
            if (Port0 != null)
            {
                Port0.rect.y = Rect.y + (Rect.height * 0.5f) - Port0.rect.height * 0.5f;
                Port0.rect.y = Rect.y + Rect.height / 3f;
                Port0.rect.x = Rect.x - Port0.rect.width + 16f;
                Port0.Draw();
                GUI.Label(new Rect(Port0.rect.xMax, Port0.rect.y, 100, 15), "Mandatory");
            }
            // quality
            if (Port1 != null)
            {
                Port1.rect.y = Rect.y + (Rect.height * 0.5f) - Port1.rect.height * 0.5f;
                Port1.rect.y = Rect.y + (Rect.height / 3f) * 2;
                Port1.rect.x = Rect.x - Port1.rect.width + 16f;
                Port1.Draw();
                GUI.Label(new Rect(Port1.rect.xMax, Port1.rect.y, 100, 15), "Quality");
            }
            //action
            if (Port2 != null)
            {
                Port2.rect.y = Rect.y + (Rect.height * 0.66f) - Port2.rect.height * 0.5f;
                Port2.rect.y = Rect.y + Rect.height / 2f;
                Port2.rect.x = Rect.x + Rect.width - 16f;
                Port2.Draw();
                GUI.Label(new Rect(Port2.rect.xMax - 55, Port2.rect.y, 50, 15), "Action");
            }

            // update
            if (Port0 != null)
            {
                if (Port0.Connections.Count > 0)
                {
                    MandatoryID = Port0.Connections[0].outputPort.MyNode.Id;
                }
                else { MandatoryID = -1; }
            }
            if (Port1 != null)
            {
                if (Port1.Connections.Count > 0)
                {
                    QualityID = Port1.Connections[0].outputPort.MyNode.Id;
                }
                else { QualityID = -1; }
            }
            if (Port2 != null)
            {
                if (Port2.Connections.Count > 0)
                {
                    Action = (Port2.Connections[0].outputPort.MyNode as ActionShell).Action;
                }
            }

            // sent
            OnUpdateRoot(MandatoryID, QualityID, Rect.position);
        }

        public void DrawThreads()
        {
            foreach (Thread mandatory in Port0.Connections)
            {
                mandatory.Draw();
            }
            foreach (Thread quality in Port1.Connections)
            {
                quality.Draw();
            }
            foreach (Thread action in Port2.Connections)
            {
                action.Draw();
            }
        }

    }
}