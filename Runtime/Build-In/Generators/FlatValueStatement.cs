using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RuleSystemCore/FactoryStatements/Modifiers/FlatValue")]
public class FlatValueStatement : Statement
{
    public override float Evaluate(RuleController controller, Rule rule, Decision decision)
    {
        return decision.FlatValue;
    }
}
