using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RuleSystemCore/FactoryStatements/Operators/Or")]
public class OrStatement : Statement
{
    public override float Evaluate(RuleController controller, Rule rule, Decision decision)
    {
        foreach (int input in decision.inputID)
        {
            if(rule.GetDecisionByIdentifier(input).Make(controller, rule) == 1)
            {
                return 1f;
            }
        }return 0f;
    }
}
