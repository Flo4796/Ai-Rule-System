using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RuleSystemCore/FactoryStatements/Operators/Add")]
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
