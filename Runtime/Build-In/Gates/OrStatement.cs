using UnityEngine;

namespace AdelicSystem.RuleAI
{
    /// <summary>
    /// Build-In <see cref="Statement"/>. Type = OR-Gate boolian logic.
    /// </summary>

    [CreateAssetMenu(menuName = "RuleSystem/Build-In/Gate/Or")]
    public class OrStatement : Statement
    {
        public override float Evaluate(RuleController controller, Rule rule, Decision decision)
        {
            foreach (int input in decision.inputID)
            {
                if (rule.GetDecisionByIdentifier(input).Make(controller, rule) == 1)
                {
                    return 1f;
                }
            }
            return 0f;
        }
    }
}
