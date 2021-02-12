using UnityEngine;

namespace AdelicSystem.RuleAI
{
    /// <summary>
    /// Build-In <see cref="Statement"/>. Type represents a math division of the inputs.
    /// </summary>
    [CreateAssetMenu(fileName = "new DivisionStatement", menuName = "RuleSystem/Build-In/Mutator/Division")]
    public class DivisionStatement : Statement
    {
        // Makes Quality Decision for an implicit Action.
        public override float Evaluate(RuleController controller, Rule rule, Decision decision)
        {
            if (decision.inputID.Length > 1)
            {
                float total = rule.GetDecisionByIdentifier(decision.inputID[0]).Make(controller, rule);

                for (int i = 1; i < decision.inputID.Length; i++)
                {
                    total /= rule.GetDecisionByIdentifier(decision.inputID[i]).Make(controller, rule);
                }
                return total;
            }
            else if (decision.inputID.Length == 1)
            {
                return rule.GetDecisionByIdentifier(decision.inputID[0]).Make(controller, rule);
            }
            return 0f;
        }
    }
}

