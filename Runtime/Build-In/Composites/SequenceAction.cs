using UnityEngine;
using AdelicSystem.RuleAI;
///<summary>
///  Build-In <see cref="Action"/> composite. Used to sequence multiple action-elements.
//</summary>


namespace AdelicSystems.RuleAI
{
    [CreateAssetMenu(fileName = "new SequenceAction", menuName = "RuleSystem/Actions/Composites/Sequence")]
    public class SequenceAction : Action
    {
        public Action[] ActionElements;
        // Acts out Action. Runs every updateloop.
        public override void Execute(RuleController controller)
        {
            // Execute Current Index
            int index = controller.GetSequenceIndex(this);
            ActionElements[index].Execute(controller);

            // Increment Index if Action-Element is complete
            if (ActionElements[index].IsComplete(controller))
                controller.IncrementSequence(this);

        }

        // Evaluates if this Action can be done together with active(other) actions.
        public override bool CanDoBoth(Rule[] others)
        {
            // Sequence can be done both if ALL Action-Elements can do both
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
            // A sequence interups if the first element can interupt.
            return ActionElements[0].CanInterupt();
        }

        // Evaluates, once action is active, if the action is completed.
        public override bool IsComplete(RuleController controller)
        {
            // Completes Sequence if The sequence index is bigger than the amount of elements in sequence
            return controller.GetSequenceIndex(this) >= ActionElements.Length;
        }

        // Runs once, when action is activated.
        public override void OnEnterAction(RuleController controller)
        {
            //Subscribe this Sequence to controller
            controller.SubscribeSequence(this);
        }

        // Runs once, when action is deactivated.
        public override void OnExitAction(RuleController controller)
        {
            // Unsubscribe this sequence from controller
            controller.UnsubScribeSequence(this);
        }
    }
}
