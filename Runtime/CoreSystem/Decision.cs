using UnityEngine;

/// <summary>
/// Represents Iteration-specific wraper for a <see cref="Statement"/>.
/// </summary>
[System.Serializable]
public class Decision
{
    public Vector2 GridPosition;
    /// <summary>An identifier unique relative to the <see cref="Rule"/>. </summary>
    public int identifier;
    /// <summary>Collection of identifiers from dependent <see cref="Decision"/> within rule. </summary>
    public int[] inputID;
    public Statement Operator; 

    // Decision specific values
    public float FlatValue;

    public Decision() 
    {
        inputID = new int[0];
    }

    /// <summary>
    /// Triggers evaluation function within <see cref="Statement"/>.
    /// </summary>
    /// <param name="controller"><see cref="RuleController"/> triggering evaluation. </param>
    /// <param name="rule"><see cref="Rule"/> refrence to rule containing this decisison. </param>
    /// <returns></returns>
    public float Make(RuleController controller, Rule rule)
    {
        return Operator.Evaluate(controller, rule, this);
    }
}
