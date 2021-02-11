using UnityEngine;

///<summary>
///Represents the connection between <see cref="Decision"/> and <see cref="Action"/>.
///Instances of this class are evaluated by the <see cref="RuleController"/> to change the behaviour of an Entity. 
/// </summary>
[System.Serializable]
public class Rule
{
    public string RuleName;
    public string Description;
    public Vector2 RootGridPosition;
    public Vector2 ActionGridPosition;
    /// <summary>Represents a seperate <see cref="Decision"/>-tree for evaluating how well this rule would work for executing <see cref="RuleController"/>.</summary>
    public int QualityId = -1;
    /// <summary> Represents a <see cref="Decision"/>-tree of <see cref="Statement"/> that have to be true in order for the rule to work. </summary>
    public int MandatoryId = -1;
    public Decision[] MyDecisions;
    public Action MyAction;
    public float Quality;


    /// <summary>
    /// Evaluates <see cref="Decision"/> that represent Mandatory conditions for an <see cref="Action"/> to work.
    /// </summary>
    /// <param name="controller"> Reference to the <see cref="RuleController"/> that evaluates this rule.</param>
    /// <returns>true when action is executable.</returns>
    public bool CanRuleBeExecuted(RuleController controller)
    {
        Decision mandatory = GetDecisionByIdentifier(MandatoryId);
        return (mandatory.Make(controller, this) > 0) ? true : false;
    }

    /// <summary>
    /// Evaluates <see cref="Decision"/> that represent the Quality of this rule in relation to the entities current situation.
    /// </summary>
    /// <param name="controller"></param>
    /// <returns>value representing Quality. Higher means more likely to be the right decision.</returns>
    public float MakeQualityDecision(RuleController controller)
    {
        Decision quality = GetDecisionByIdentifier(QualityId);
        Quality = quality.Make(controller, this);
        return Quality;
    }

    /// <summary>
    /// Function that translates an decision-identifier into a <see cref="Decision"/>.
    /// </summary>
    /// <param name="id">identifier of decision</param>
    /// <returns>decision of identifer</returns>
    public Decision GetDecisionByIdentifier(int id)
    {
        foreach (Decision decision in MyDecisions)
        {
            if (decision.identifier == id)
                return decision;
        }return null;
    }
}
