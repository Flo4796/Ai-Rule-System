using UnityEngine;



namespace AdelicSystem.RuleAI
{
    /// <summary>
    /// Represents base class representing something a Rule-based-entity can do.
    /// </summary>
    public abstract class Action : ScriptableObject
    {
        public string Name;
        [TextArea] public string Description;
        public ActionType Type;
        /// <summary>
        /// Run once entry Action. Used pre-execute loop.
        /// </summary>
        /// <param name="controller"><see cref="GameObject"/> calling this action. </param>
        public abstract void OnEnterAction(RuleController controller);
        /// <summary>
        /// Main functionality loop. Run while action is active.
        /// </summary>
        /// <param name="controller"><see cref="GameObject"/> calling this action. </param>
        public abstract void Execute(RuleController controller);
        /// <summary>
        /// Run once exit Action. Used post-execute loop.
        /// </summary>
        /// <param name="controller"><see cref="GameObject"/> calling this action. </param>
        public abstract void OnExitAction(RuleController controller);
        /// <summary>
        /// Evaluation if this action can interupt active-actions.
        /// </summary>
        /// <returns></returns>
        public abstract bool CanInterupt();
        /// <summary>
        /// Evaluates whether this action can co-execute with other actions.
        /// </summary>
        /// <param name="others"></param>
        /// <returns></returns>
        public abstract bool CanDoBoth(Rule[] others);
        /// <summary>
        /// Evaluation whether this action has completed for this <see cref="RuleController"/>.
        /// </summary>
        /// <param name="controller"><see cref="GameObject"/> calling this action. </param>
        /// <returns></returns>
        public abstract bool IsComplete(RuleController controller);
    }
}
