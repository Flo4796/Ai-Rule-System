using UnityEngine;
///<summary>
/// Max 2 inputs: compares whether input a is lesser than input b.
///</summary>
[CreateAssetMenu(fileName = "new LesserThan", menuName = "RuleSystem/Build-In/Inequalities/LesserThan")]
public class LesserThan : Statement
{
    // Makes Quality Decision for an implicit Action.
    public override float Evaluate (RuleController controller, Rule rule, Decision decision)
    {
        // Do decision logic here.
        if(decision.inputID.Length == 2)
        {
            float a = rule.GetDecisionByIdentifier(decision.inputID[0]).Make(controller, rule);
            float b = rule.GetDecisionByIdentifier(decision.inputID[1]).Make(controller, rule);
            return (a < b) ? 1 : 0;
        }return 0;
    }
}

