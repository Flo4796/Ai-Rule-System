using UnityEngine;

[CreateAssetMenu(menuName = "RuleSystemCore/FactoryStatements/Operators/And")]
public class AndStatement : Statement
{
    public override float Evaluate(RuleController controller, Rule rule, Decision decision)
    {
        foreach (int input in decision.inputID)
        {
            if(rule.GetDecisionByIdentifier(input).Make(controller,rule) == 0)
            {
                return 0;
            }
        }return 1;
    }
}
