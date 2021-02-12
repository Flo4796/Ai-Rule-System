using UnityEngine;

namespace AdelicSystem.RuleAI
{
    /// <summary>
    /// Build-In <see cref="Statement"/>. Type represents a math subtraction of the inputs.
    /// </summary>
    [CreateAssetMenu(menuName = "RuleSystem/Build-In/Mutator/Subtract")]
    public class SubtractStatement : Statement
    {
        public override float Evaluate(RuleController controller, Rule rule, Decision decision)
        {
            float total = 0f;

            foreach (int input in decision.inputID)
            {
                if (total == 0)
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
}