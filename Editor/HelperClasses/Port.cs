using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PortType { Input, Output }
/// <summary>
/// Representing a connection point of a <see cref="NodeShell"/>.
/// </summary>
public class Port
{
    public string Name;
    public Color Color = Color.white;
    public bool isDecision;
    public Rect rect;
    public PortType type;
    public List<Thread> Connections = new List<Thread>();
    public NodeShell MyNode;
    public Action<Port> OnClickPort;
    public Action<Thread> OnRemoveThread;
    public GUISkin nodeSkin;
    const string ThreadFormat = "From: {0} To: {1}";

    public void Draw()
    {


        var prev = GUI.backgroundColor;
        GUI.backgroundColor = Color;

        if (GUI.Button(rect,"", nodeSkin.button))
        {
            if (OnClickPort != null)
            {
                OnClickPort(this);
            }
        }
        GUI.backgroundColor = prev;
    }


    public void ProcessEvents(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDown:
                if(e.button == 1)
                {
                    if(rect.Contains(e.mousePosition) && Connections.Count > 0)
                    {
                        GenericMenu menu = new GenericMenu();
                        foreach (Thread thread in Connections)
                        { 
                            menu.AddItem(new GUIContent(string.Format(ThreadFormat, Name, thread.outputPort.Name), "Removes thread from port"), false, () => OnRemoveThread(thread));
                        }
                        
                        menu.ShowAsContext();
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Collects <see cref="Decision"/> identifiers from connected <see cref="Thread"/>.
    /// </summary>
    /// <returns>represents dependencies of this decision. </returns>
    public int[] GetConnectionIDs()
    {
        int[] identifiers = new int[0];
        foreach (Thread thread in Connections)
        {
            ArrayUtility.Add<int>(ref identifiers, thread.outputPort.MyNode.Id);
        }
        return identifiers;
    }

}
