using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PortType { Input, Output }
public class Port2
{
    public Color Color = Color.white;
    public bool isDecision;
    public Rect rect;
    public PortType type;
    public List<Thread> Connections = new List<Thread>();
    public NodeShell MyNode;
    public Action<Port2> OnClickPort;
    public GUISkin nodeSkin;


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
