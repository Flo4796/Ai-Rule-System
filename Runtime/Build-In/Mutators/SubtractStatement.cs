using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RuleSystemCore/FactoryStatements/Operators/Subtract")]
public class SubtractStatement : Statement
{
    public override float Evaluate(RuleController controller, Rule rule, Decision decision)
    {
        float total = 0f;

        foreach (int input in decision.inputID)
        {
            if(total == 0)
            {
                total = rule.GetDecisionByIdentifier(input).Make(controller, rule);
            }
            else
            {
                total -= rule.GetDecisionByIdentifier(input).Make(controller, rule);
            }
        }
        return total;
    }
}
