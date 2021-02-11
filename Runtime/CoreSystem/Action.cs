using UnityEngine;

public abstract class Action : ScriptableObject
{
    public string Name;
    [TextArea] public string Description;
    public ActionType Type;
    public abstract void OnEnterAction(RuleController controller);
    public abstract void Execute(RuleController controller);
    public abstract void OnExitAction(RuleController controller);
    public abstract bool CanInterupt();
    public abstract bool CanDoBoth(Rule[] others);
    public abstract bool IsComplete(RuleController controller);
}
