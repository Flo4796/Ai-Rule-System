using UnityEngine;
/// <summary>
/// Build-In <see cref="Statement"/>. Type representing an unchanged float value.
/// </summary>

[CreateAssetMenu(menuName = "RuleSystem/Build-In/Generator/FlatValue")]
public class FlatValueStatement : Statement
{
    public override float Evaluate(RuleController controller, Rule rule, Decision decision)
    {
        return decision.FlatValue;
    }
}
