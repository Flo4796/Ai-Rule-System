using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RuleWindow2 : EditorWindow
{
    [SerializeField] GUISkin AdelicSkin;
    [SerializeField] GUISkin NodeSkin;
    bool isInitialized = false;
    SessionData myData;
    public SerializedProperty SubjectRule;

    #region Fields
    Dictionary<StatementType, List<Statement>> capturedStatements = new Dictionary<StatementType, List<Statement>>();
    Dictionary<ActionType, List<Action>> capturedActions = new Dictionary<ActionType, List<Action>>();
    List<Decision> ruleDecisions = new List<Decision>();
    List<NodeShell> ruleNodes = new List<NodeShell>();
    List<Thread> Yarn = new List<Thread>();
    Action ruleAction = null;
    Port2 selectedInPort = null;
    Port2 selectedOutPort = null;
    RootShell rootNode = null;
    NodeShell createdNode = null;
    ActionShell actionNode = null;
    ActionShell createdActionNode = null;
    Decision createdDecision = null;
    Rule deserializedRule = null;
    Vector2 offset;
    Vector2 drag;
    Vector2 mousePos;
    #endregion



    public void Initialize()
    {
        // catching refrences
        myData = new SessionData();
        // Collect Statements
        capturedStatements = RuleSystemUtil.CollectDecisionsByType();
        // Collect Actions
        capturedActions = RuleSystemUtil.CollectActionsByType();
        // Load Rule
        LoadRule();
        LoadThreads();
        isInitialized = true;
    }

    private void OnGUI()
    {
        if (!isInitialized) { return; }
        mousePos = Event.current.mousePosition;
        // Update nodes
        SubjectRule.serializedObject.Update();
        // Draw Grid
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        // Drawn Nodes
        DrawNodes();
        // Draw Connections
        DrawThreads();

        // Draw ContextMenu
        RuleLayoutUtil.DrawContentLibraryLayout(capturedStatements, OnRequestDecisionNode, 
                                                capturedActions, OnRequestActionNode,
                                                WindowUtils.ContentMenuLayout,ref myData, AdelicSkin);

        // process events
        ProcessCreatedNode(Event.current);
        ProcessLiveThread(Event.current);
        ProcessNodeEvents(Event.current);
        ProcessEvent(Event.current);


       

        if (GUI.changed)
        {
            // Update Rule
            SaveRule();
        }
    }

    #region Serialization
    private void LoadRule()
    {
        string setName = SubjectRule.propertyPath.Remove(SubjectRule.propertyPath.IndexOf(".Array")).Replace("Profile.", "");
        int ruleIndex = int.Parse(SubjectRule.propertyPath.Substring(SubjectRule.propertyPath.IndexOf("[") + 1).Replace("]", ""));

        deserializedRule = RuleSystemUtil.DeserializeRule(SubjectRule,setName,ruleIndex);
        rootNode = RuleCreator.CreateRoot(deserializedRule.RootGridPosition, OnNodePortContact, OnUpdateRootRequest,deserializedRule.MandatoryId, deserializedRule.QualityId, NodeSkin);
        if (deserializedRule.MyAction != null)
        {
            actionNode = RuleCreator.CreateActionNode(deserializedRule.ActionGridPosition, OnNodePortContact, OnUpdateActionRequest, deserializedRule.MyAction, NodeSkin);
            actionNode.activated = true;
        }
        foreach (Decision decision in deserializedRule.MyDecisions)
        {

            ruleDecisions.Add(decision);
            NodeShell rawNode = RuleCreator.CreateNewDecisionNode(decision.identifier, decision.Operator,OnUpdateRuleRequest, OnNodePortContact, NodeSkin);
            rawNode.Container = new NodeShell.Data
            {
                FloatValue = decision.FlatValue
            };
            rawNode.Inputs = decision.inputID;
            rawNode.Rect.position = decision.GridPosition;
            rawNode.activated = true;
            ruleNodes.Add(rawNode);
        }
    }
    private void LoadThreads()
    {
        if (rootNode.MandatoryID != -1)
        {
            NodeShell mandatory = GetNodeById(rootNode.MandatoryID);
            Thread rootThread = new Thread(rootNode.Port0, mandatory.Port1);
            rootNode.Port0.Connections.Add(rootThread);
        }
        if(rootNode.QualityID != -1)
        {
            NodeShell Quality = GetNodeById(rootNode.QualityID);
            Thread qualityThread = new Thread(rootNode.Port1, Quality.Port1);
            rootNode.Port1.Connections.Add(qualityThread);
        }
        rootNode.activated = true;
        foreach (NodeShell node in ruleNodes)
        {
            if(node.Inputs.Length > 0)
            {
                foreach (int inputId in node.Inputs)
                {
                    NodeShell output = GetNodeById(inputId);
                    Thread thread = new Thread(node.Port0, output.Port1);
                    node.Port0.Connections.Add(thread);
                    Yarn.Add(thread);
                }
            }
        }
    }

    private void SaveRule()
    {
        string setName = SubjectRule.propertyPath.Remove(SubjectRule.propertyPath.IndexOf(".Array")).Replace("Profile.", "");
        int ruleIndex = int.Parse(SubjectRule.propertyPath.Substring(SubjectRule.propertyPath.IndexOf("[") + 1).Replace("]", ""));
        deserializedRule.MandatoryId = rootNode.MandatoryID;
        deserializedRule.QualityId = rootNode.QualityID;
        deserializedRule.RootGridPosition = rootNode.Rect.position;
        deserializedRule.MyDecisions = ruleDecisions.ToArray();
        deserializedRule.MyAction = ruleAction;
        if(actionNode!= null)deserializedRule.ActionGridPosition = actionNode.Rect.position;
        RuleSystemUtil.SerializeRule(deserializedRule, SubjectRule, setName, ruleIndex);
        SubjectRule.serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Draw
    private void DrawNodes()
    {
        rootNode.Draw();
        if (actionNode != null)
            actionNode.Draw();
        foreach (NodeShell node in ruleNodes)
        {
            node.Draw();
        }
    }
    private void DrawThreads()
    {
        rootNode.DrawThreads();

        foreach (Thread thread in Yarn)
        {
            thread.Draw();
        }
    }
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
    #endregion

    #region Decision Helpers
    private Decision GetDecisionById(int id)
    {
        foreach (Decision decision in ruleDecisions)
        {
            if(decision.identifier == id) { return decision; }
        }
        return null;
    }
    private NodeShell GetNodeById(int id)
    {
        foreach (NodeShell node in ruleNodes)
        {
            if(node.Id == id) { return node; }
        }
        return null;
    }
    private int GenerateDecisionId()
    {
        int id = 0;
        bool isUnique = true;

        do
        {
            isUnique = true;
            foreach (Decision decision in ruleDecisions)
            {
                if (id == decision.identifier)
                {
                    id++;
                    isUnique = false;
                    break;
                }
            }
        } while (!isUnique);
        return id;
    }
    #endregion

    #region Port/Thread Helpers
    private bool IsConnected(Port2 input, Port2 output)
    {
        foreach (Thread thread in Yarn)
        {
            if(thread.inputPort == input && thread.outputPort == output)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region I/O Functions
    private void ProcessLiveThread(Event e)
    {
        if (selectedInPort != null & selectedOutPort == null)
        {
            Handles.DrawBezier(
                selectedInPort.rect.center,
                e.mousePosition,
                selectedInPort.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                5f
            );

            GUI.changed = true;
        }

        if (selectedOutPort != null & selectedInPort == null)
        {
            Handles.DrawBezier(
                selectedOutPort.rect.center,
                e.mousePosition,
                selectedOutPort.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                selectedOutPort.Color,
                null,
                5f
            );

            GUI.changed = true;
        }
    }
    private void ProcessCreatedNode(Event e)
    {
        if (createdNode != null)
        {
            createdNode.Rect.position = e.mousePosition;
            createdNode.Draw();
            switch (e.type)
            {
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        if (createdNode != null)
                        {
                            ruleDecisions.Add(createdDecision);
                            ruleNodes.Add(createdNode);
                            createdNode.activated = true;
                            createdNode = null;
                            createdDecision = null;
                        }

                    }
                    else if (e.button == 1)
                    {
                        createdNode = null;
                    }
                    break;
            }
        }
        if (createdActionNode != null)
        {
            createdActionNode.Rect.position = e.mousePosition;
            createdActionNode.Draw();

            switch (e.type)
            {
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        if (createdActionNode != null)
                        {
                            actionNode = createdActionNode;
                            ruleAction = actionNode.Action;
                            actionNode.activated = true;
                            createdActionNode = null;
                        }
                    }
                    else if (e.button == 1)
                    {
                        createdActionNode = null;
                    }
                    break;

            }
        }
    }
    private void ProcessEvent(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }
    private void ProcessNodeEvents(Event e)
    {
        rootNode.ProcessEvents(e);
        if(actionNode!= null) { actionNode.ProcessEvents(e); }
        if (ruleNodes != null)
        {
            for (int i = ruleNodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = ruleNodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }
    private void OnDrag(Vector2 delta)
    {
        drag = delta;
        rootNode.Drag(delta);
        if(actionNode!= null) { actionNode.Drag(delta); }
        if (ruleNodes != null)
        {
            for (int i = 0; i < ruleNodes.Count; i++)
            {
                ruleNodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    #endregion

    #region Event Handles
    private void OnRequestDecisionNode(Statement statement)
    {
        createdDecision = new Decision();
        createdDecision.identifier = GenerateDecisionId();
        createdDecision.Operator = statement;
        createdNode = RuleCreator.CreateNewDecisionNode(createdDecision.identifier, statement, OnUpdateRuleRequest, OnNodePortContact, NodeSkin);
        createdNode.Rect.position = mousePos;
        
    }
    private void OnRequestActionNode(Action action)
    {
        createdActionNode = RuleCreator.CreateActionNode(deserializedRule.ActionGridPosition, OnNodePortContact, OnUpdateActionRequest, action, NodeSkin);
        createdActionNode.Rect.position = mousePos;

    }
    private void OnUpdateRuleRequest(int id, Statement asset, int[] inputs, NodeShell.Data data, Vector2 position)
    {
        Decision decision = GetDecisionById(id);
        if(decision == null) { return; }
        if (inputs != null) 
        { decision.inputID = inputs; }
        decision.Operator = asset;
        decision.FlatValue = data.FloatValue;
        decision.GridPosition = position;
    }
    private void OnUpdateRootRequest(int mandatoryID, int QualityID, Vector2 position)
    {
        deserializedRule.MandatoryId = mandatoryID;
        deserializedRule.QualityId = QualityID;
        deserializedRule.RootGridPosition = position;
    }
    private void OnUpdateActionRequest(Action action, Vector2 position)
    {
        deserializedRule.ActionGridPosition = position;
        deserializedRule.MyAction = action;
    }
    private void OnNodePortContact(Port2 port)
    {
        if(port.type == PortType.Input)
        {
            selectedInPort = port;

            if (selectedOutPort != null)
            {
                if(selectedInPort.MyNode != selectedOutPort.MyNode)
                {
                    if(!IsConnected(selectedInPort, selectedOutPort))
                    {
                        if (selectedInPort.isDecision == selectedOutPort.isDecision)
                        {
                            Thread thread = new Thread(selectedInPort, selectedOutPort);
                            selectedInPort.Connections.Add(thread);
                            Yarn.Add(thread);
                        }
                    }    
                }
                selectedInPort = null;
                selectedOutPort = null;
            }
        }
        else if(port.type == PortType.Output)
        {
            selectedOutPort = port;

            if(selectedInPort != null)
            {
                if(selectedInPort.MyNode != selectedOutPort.MyNode)
                {
                    if(!IsConnected(selectedInPort, selectedOutPort))
                    {
                        if (selectedInPort.isDecision == selectedOutPort.isDecision)
                        {
                            Thread thread = new Thread(selectedInPort, selectedOutPort);
                            selectedInPort.Connections.Add(thread);
                            Yarn.Add(thread);
                        }

                    }
                }
                selectedInPort = null;
                selectedOutPort = null;
            }
        }

    }

    #endregion
}
