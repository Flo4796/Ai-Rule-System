using UnityEngine;

public abstract class Statement: ScriptableObject 
{
    public string Name;
    [TextArea] public string Description;
    public StatementType Type;
    public abstract float Evaluate (RuleController controller, Rule rule, Decision decision);

}
