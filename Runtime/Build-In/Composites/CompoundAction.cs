using UnityEngine;
using AdelicSystem.RuleAI;
///<summary>
/// Build-In <see cref="Action"/> composite. Used to combine multiple single actions into a single compound.
//</summary>


namespace AdelicSystems.RuleAI
{
    [CreateAssetMenu(fileName = "new CompoundAction", menuName = "RuleSystem/Actions/Composites/Compound")]
    public class CompoundAction : Action
    {
        public Action[] ActionElements;
        // Acts out Action. Runs every updateloop.
        public override void Execute(RuleController controller)
        {
            // Executes all Action-Elements simultaniously.
            for (int i = 0; i < ActionElements.Length; i++)
            {
                ActionElements[i].Execute(controller);
            }
        }

        // Evaluates if this Action can be done together with active(other) actions.
        public override bool CanDoBoth(Rule[] others)
        {
            // This compound can be done together if ALL action-elements can be done together with the other
            for (int i = 0; i < ActionElements.Length; i++)
            {
                if (!ActionElements[i].CanDoBoth(others))
                    return false;
            }
            return true;
        }

        // Evaluates if this action can interupt the active actions.
        public override bool CanInterupt()
        {
            // Interution is possible if ONE of the Action-Elements is interuptable.
            for (int i = 0; i < ActionElements.Length; i++)
            {
                if (ActionElements[i].CanInterupt())
                    return true;
            }
            return false;
        }

        // Evaluates, once action is active, if the action is completed.
        public override bool IsComplete(RuleController controller)
        {
            // Compound is complete when ALL Action-Elements are Complete.
            for (int i = 0; i < ActionElements.Length; i++)
            {
                if (!ActionElements[i].IsComplete(controller))
                    return false;
            }
            return true;
        }

        // Runs once, when action is activated.
        public override void OnEnterAction(RuleController controller)
        {
            // Do Safety Check:
            if (ActionElements == null && ActionElements.Length == 0)
                throw new System.Exception("Error Compound broken!");

            // All Action-Element Entry Actions are Executed Simultaniously.
            for (int i = 0; i < ActionElements.Length; i++)
            {
                ActionElements[i].OnEnterAction(controller);
            }
        }

        // Runs once, when action is deactivated.
        public override void OnExitAction(RuleController controller)
        {
            // All Action-Element Exit Actions are Executed Simultaniously.
            for (int i = 0; i < ActionElements.Length; i++)
            {
                ActionElements[i].OnEnterAction(controller);
            }
        }
    }
}
