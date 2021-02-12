using UnityEngine;

namespace AdelicSystem.RuleAI
{
    /// <summary>
    /// Represents base class of something a rule-based-entity can evaluate.
    /// </summary>
    public abstract class Statement : ScriptableObject
    {
        public string Name;
        [TextArea] public string Description;
        public StatementType Type;
        public abstract float Evaluate(RuleController controller, Rule rule, Decision decision);

    }
}
