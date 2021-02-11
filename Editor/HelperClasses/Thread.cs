using UnityEditor;
using UnityEngine;

/// <summary>
/// Represents connection between two <see cref="NodeShell"/> by containing <see cref="Port"/> refrences.
/// </summary>
public class Thread
{
    public Port inputPort = null;
    public Port outputPort = null;
    Color myColor = Color.white;

    public Thread(Port _inPort, Port _outPort)
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
