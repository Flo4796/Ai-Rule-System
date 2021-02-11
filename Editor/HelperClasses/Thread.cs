using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Thread
{
    public Port2 inputPort = null;
    public Port2 outputPort = null;
    Color myColor = Color.white;

    public Thread(Port2 _inPort, Port2 _outPort)
    {
        inputPort = _inPort;
        outputPort = _outPort;
        myColor = _outPort.Color;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            inputPort.rect.center,
            outputPort.rect.center,
            inputPort.rect.center + Vector2.left * 50f,
            outputPort.rect.center - Vector2.left * 50f,
            myColor,
            null,
            5f
        );

        //if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        //{
        //    if (OnClickRemoveConnection != null)
        //    {
        //        OnClickRemoveConnection(this);
        //    }
        //}
    }
}
