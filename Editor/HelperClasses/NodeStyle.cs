using UnityEngine;

/// <summary>
/// Base class representing Node GUILayout settings.
/// </summary>
public class NodeStyle
{
    public string Name;
    public Rect Rect = new Rect(0,0,150,25);
    public Color Color;
    //create ports
    public StatementType Type;
    public float CollapsedHeight = 25;
    public float MinExpandedHeight = 100;
}
/// <summary>
/// Derrived class representing Action node GUILayout settings.
/// </summary>
public class ActionStyle: NodeStyle
{
    public ActionStyle(string name)
    {
        Name = name;
        Color = Color.white;
        Rect.size = new Vector2(135, 75);
    }
}
/// <summary>
/// Derrived class representing Root node GUILayout settings.
/// </summary>
public class RootNodeStyle:NodeStyle
{
    public RootNodeStyle(string name)
    {
        Name = name;
        Rect = new Rect(0, 0, 175, 150);
        Color = Color.gray;
        Type = StatementType.unknown;
    }
}
/// <summary>
/// Derrived class representing Generator node GUILayout settings.
/// </summary>
public class GenNodeStyle : NodeStyle
{
    public GenNodeStyle(string name)
    {
        Name = name;
        Color = new Color(0.172549f, 0.7882354f, 0.5647059f);
        Type = StatementType.Generator;
    }
}
/// <summary>
/// Derrived class representing Boolian node GUILayout settings.
/// </summary>
public class BoolNodeStyle : NodeStyle
{
    public BoolNodeStyle(string name, StatementType type)
    {
        Name = name;
        Color = new Color(0.9882354f, 0.3764706f, 0.2588235f);
        Type = type;
    }
}
/// <summary>
/// Derrived class representing Mutator node GUILayout settings.
/// </summary>
public class MutNodeStyle : NodeStyle
{
    public MutNodeStyle(string name)
    {
        Name = name;
        Color = new Color(0.172549f, 0.509804f, 0.7882354f); 
        Type = StatementType.Mutator;
    }
}
/// <summary>
/// Derrived class representing Custom node GUILayout settings.
/// </summary>
public class EvNodeStyle:NodeStyle
{
    public EvNodeStyle(string name)
    {
        Name = name;
        Color = new Color(0.9333334f, 0.9019608f, 0.3411765f);
        Type = StatementType.Evaluation;
    }
}