using UnityEngine;

/// <summary>
/// Build-In <see cref="Statement"/>. Type represents a math Addition of the inputs.
/// </summary>
[CreateAssetMenu(menuName = "RuleSystem/Build-In/Mutator/Add")]
public class AddStatement : Statement
{
    public override float Evaluate(RuleController controller, Rule rule, Decision decision)
    {
        float total = 0f;

        foreach (int input in decision.inputID)
        {
            total += rule.GetDecisionByIdentifier(input).Make(controller, rule);
        }
        return total;
    }
}
