using UnityEngine;


[CreateAssetMenu(menuName = "RuleSystemCore/FactoryStatements/Operators/Multiply")]
public class MultiplyStatement : Statement
{
    public override float Evaluate(RuleController controller, Rule rule, Decision decision)
    {
        float total = 1f;
        foreach (int input in decision.inputID)
        {
            total *= rule.GetDecisionByIdentifier(input).Make(controller, rule);
        }
        return total;
    }
}
