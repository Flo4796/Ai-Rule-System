using UnityEngine;
///<summary>
/// Set here Description of decision Functionality
///</summary>
[CreateAssetMenu(fileName = "new Equal", menuName = "RuleSystem/Build-In/Inequalities/Equal")]
public class Equal : Statement
{
    // Makes Quality Decision for an implicit Action.
    public override float Evaluate (RuleController controller, Rule rule, Decision decision)
    {
        // Do decision logic here.
        if(decision.inputID.Length > 1)
        {
            float value = rule.GetDecisionByIdentifier(decision.inputID[0]).Make(controller, rule);
            for (int i = 1; i < decision.inputID.Length; i++)
            {
                if(rule.GetDecisionByIdentifier(decision.inputID[i]).Make(controller,rule) != value)
                {
                    return 0;
                }
            }return 1;
        }return 0;
    }
}

