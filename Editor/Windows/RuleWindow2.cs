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
    Port selectedInPort = null;
    Port selectedOutPort = null;
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
                                                capturedActions, OnRequestActionNode, SubjectRule,
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
    /// <summary>
    /// Load function that unpacks <see cref="Rule"/> from corresponding <see cref="SerializedProperty"/>.
    /// </summary>
    private void LoadRule()
    {
        string setName = SubjectRule.propertyPath.Remove(SubjectRule.propertyPath.IndexOf(".Array")).Replace("Profile.", "");
        int ruleIndex = int.Parse(SubjectRule.propertyPath.Substring(SubjectRule.propertyPath.IndexOf("[") + 1).Replace("]", ""));

        deserializedRule = RuleSystemUtil.DeserializeRule(SubjectRule,setName,ruleIndex);
        rootNode = RuleCreator.CreateRoot(deserializedRule.RootGridPosition, OnNodePortContact, OnUpdateRootRequest,deserializedRule.MandatoryId, deserializedRule.QualityId, NodeSkin);
        if (deserializedRule.MyAction != null)
        {
            actionNode = RuleCreator.CreateActionNode(deserializedRule.ActionGridPosition, OnNodePortContact, OnUpdateActionRequest, OnRemoveNode, deserializedRule.MyAction, NodeSkin);
        }
        foreach (Decision decision in deserializedRule.MyDecisions)
        {

            ruleDecisions.Add(decision);
            NodeShell rawNode = RuleCreator.CreateNewDecisionNode(decision.identifier, decision.Operator,OnUpdateRuleRequest, OnNodePortContact, OnRemoveNode ,NodeSkin);
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
    /// <summary>
    /// Creates established threads from deserialized Rule.
    /// </summary>
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
        if(actionNode!= null)
        {
            Thread actionThread = new Thread(rootNode.Port2, actionNode.Port0);
            rootNode.Port2.Connections.Add(actionThread);
            actionNode.activated = true;
        }

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
    /// <summary>
    /// Save function that repacks Rule into <see cref=" SerializedProperty"/> and updates Obj.
    /// </summary>
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
    /// <summary>
    /// Iteration over <see cref="NodeShell"/> in window. Triggers Draw - functionality.
    /// </summary>
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
    /// <summary>
    /// Iteration over <see cref="Thread"/> in window. Triggers Draw - functionality.
    /// </summary>
    private void DrawThreads()
    {
        rootNode.DrawThreads();
        foreach (Thread thread in Yarn)
        {
            thread.Draw();
        }
    }
    /// <summary>
    /// Draws grid.
    /// </summary>
    /// <param name="gridSpacing">Spacing between two grid lines. </param>
    /// <param name="gridOpacity">Opacity of grid lines. </param>
    /// <param name="gridColor">Color of grid background. </param>
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

    #region Node Helpers
    /// <summary>
    /// Returns <see cref="Decision"/> by iterating ruleDecisions.
    /// </summary>
    /// <param name="id">Decision specific identifier.</param>
    /// <returns></returns>
    private Decision GetDecisionById(int id)
    {
        foreach (Decision decision in ruleDecisions)
        {
            if(decision.identifier == id) { return decision; }
        }
        return null;
    }
    /// <summary>
    /// Returns corresponding <see cref="NodeShell"/> of a decision by id.
    /// </summary>
    /// <param name="id">Decision specific identifier. </param>
    /// <returns></returns>
    private NodeShell GetNodeById(int id)
    {
        foreach (NodeShell node in ruleNodes)
        {
            if(node.Id == id) { return node; }
        }
        return null;
    }
    /// <summary>
    /// Returns <see cref="int"/> number that is unique relative to the current <see cref="Rule"/>. 
    /// Used to identify <see cref="Decision"/>.
    /// </summary>
    /// <returns></returns>
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
    private bool IsConnected(Port input, Port output)
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
    /// <summary>
    /// Process handle for <see cref="Thread"/> that is currently being made.
    /// </summary>
    /// <param name="e">Current Unity Event. </param>
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
    /// <summary>
    /// Event handle for nodes that are being created.
    /// </summary>
    /// <param name="e">Current Unity Event. </param>
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
    /// <summary>
    /// Input event processing for <see cref="RuleWindow2"/>.
    /// </summary>
    /// <param name="e">Current Unity Event. </param>
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
    /// <summary>
    /// Iteration through <see cref="NodeShell"/>, executing ProcessEvent - function.
    /// </summary>
    /// <param name="e">Current Unity Event. </param>
    private void ProcessNodeEvents(Event e)
    {
        rootNode.ProcessEvents(e);
        if (actionNode != null) { actionNode.ProcessEvents(e); }
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
    /// <summary>
    /// Iterates through <see cref="NodeShell"/> and executes ProcessPortEvent -function.
    /// </summary>
    /// <param name="e"></param>
    private void ProcessThreadEvents(Event e)
    {
        rootNode.ProcessPortEvents(e);

        foreach (NodeShell node in ruleNodes)
        {
            node.ProcessPortEvents(e);
        }

    }
    /// <summary>
    /// Handles Object drag for Nodes.
    /// </summary>
    /// <param name="delta">Mouse delta compared to previous frame. </param>
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
    /// <summary>
    /// Action-event handles <see cref="NodeShell"/> instantiation. Used specificly for <see cref="Decision"/> nodes.
    /// </summary>
    /// <param name="statement"><see cref="Statement"/> requesting node. </param>
    private void OnRequestDecisionNode(Statement statement)
    {
        createdDecision = new Decision();
        createdDecision.identifier = GenerateDecisionId();
        createdDecision.Operator = statement;
        createdNode = RuleCreator.CreateNewDecisionNode(createdDecision.identifier, statement, OnUpdateRuleRequest, OnNodePortContact, OnRemoveNode, NodeSkin);
        createdNode.Rect.position = mousePos;
        
    }
    /// <summary>
    /// Action-event handles <see cref="ActionShell"/> instantiation.
    /// </summary>
    /// <param name="action"><see cref="Action"/> that needs a node. </param>
    private void OnRequestActionNode(Action action)
    {
        createdActionNode = RuleCreator.CreateActionNode(deserializedRule.ActionGridPosition, OnNodePortContact, OnUpdateActionRequest, OnRemoveNode, action, NodeSkin);
        createdActionNode.Rect.position = mousePos;

    }
    /// <summary>
    /// Action-event handles <see cref="NodeShell"/> update request. Triggers when node content changes.
    /// </summary>
    /// <param name="id"><see cref="Decision"/> unique identifier. </param>
    /// <param name="asset">Currently set <see cref="Statement"/> for decision.</param>
    /// <param name="inputs">Identifiers of evaluation-dependent decisions. </param>
    /// <param name="data">Decision specifc data structure. (i.e. flatvalue). </param>
    /// <param name="position">Grid position of node. </param>
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
    /// <summary>
    /// Action-event handles <see cref="RootShell"/> update request. Triggers when root node content changes.
    /// </summary>
    /// <param name="mandatoryID">Identifier of mandatory decision parameter.</param>
    /// <param name="QualityID">Identifier of Quality decision parameter.</param>
    /// <param name="position">Grid position of root node.</param>
    private void OnUpdateRootRequest(int mandatoryID, int QualityID, Vector2 position)
    {
        deserializedRule.MandatoryId = mandatoryID;
        deserializedRule.QualityId = QualityID;
        deserializedRule.RootGridPosition = position;
    }
    /// <summary>
    /// Action-event handles <see cref="ActionShell"/> update request. Triggers when an action nodes content changes.
    /// </summary>
    /// <param name="action">Currently updating <see cref="Action"/>.</param>
    /// <param name="position">Gridposition of <see cref="ActionShell"/>.</param>
    private void OnUpdateActionRequest(Action action, Vector2 position)
    {
        deserializedRule.ActionGridPosition = position;
        deserializedRule.MyAction = action;
    }
    /// <summary>
    /// Action-event handles <see cref="Port"/> clicks by user. 
    /// Connects two nodes together by <see cref="Thread"/> when specific conditions are met.
    /// </summary>
    /// <param name="port">Currently triggered <see cref="Port"/>.</param>
    private void OnNodePortContact(Port port)
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
    /// <summary>
    /// Action-event handles <see cref="NodeShell"/> remove from  rule requests.
    /// </summary>
    /// <param name="node">Node that requests to be removed.</param>
    private void OnRemoveNode(NodeShell node)
    {
        if (ruleNodes.Remove(node))
        {
            var decision = GetDecisionById(node.Id);
            ruleDecisions.Remove(decision);
        }
        else if (actionNode == node)
        {
            actionNode = null;
            ruleAction = null;
        }
    }

    private void OnRemoveThread(Thread thread)
    {
        if (rootNode.Port0.Connections.Remove(thread)) return;
        if (rootNode.Port1.Connections.Remove(thread)) return;
        if (rootNode.Port2.Connections.Remove(thread)) return;
        if (Yarn.Contains(thread)) return;
    }

    #endregion
}
