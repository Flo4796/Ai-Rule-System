using UnityEngine;
/// <summary>
/// Represents editor window utilities. Like the size of rectangles/windows.
/// </summary>
public static class WindowUtils
{
    #region Library Window
    /// <summary>Minimum size of the Library window.</summary>
    public static Vector2 LibMinWindowSize = new Vector2(1000, 500);
    /// <summary>Size of the controller display layout area </summary>
    public static Rect ControlLayout { get { return new Rect(0, 0, 250, Screen.height); } }
    /// <summary> Size of the Area displaying a <see cref="BehaviourProfile"/></summary>
    public static Rect ProfileLayout { get { return new Rect(ControlLayout.width, 0, Screen.width - ControlLayout.width, Screen.height); } }
    /// <summary>Size of the area displaying a set of rules.</summary>
    public static Rect RuleSetLayout = new Rect(0, 0, 200, 300);
    /// <summary> Size of area displaying profile property and rule manipulation.</summary>
    public static Rect PropertyLayout { get { return new Rect(Screen.width - 250, 0, 250, Screen.height); } }

    /// <summary>Minimum size of the Rule Windows.</summary>
    public static Vector2 RuleMinWindowSize = new Vector2(1000, 400);

    /// <summary>Rectangle for displaying contextMenu panel.</summary>
    public static Rect  ContentMenuLayout { get { return new Rect(Screen.width - 250, 0, 250, Screen.height); } }

    public static string RulePresetFolder = "Presets\\Rules";

    public static string ProfilePresetFolder = "Presets\\Profiles";
    #endregion
}
