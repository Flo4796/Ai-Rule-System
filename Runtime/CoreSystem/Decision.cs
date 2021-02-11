using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Decision
{
    public Vector2 GridPosition;
    public int identifier;
    public int[] inputID;
    public Statement Operator; 

    // Decision specific values
    public float FlatValue;

    public Decision() 
    {
        inputID = new int[0];
    }


    public float Make(RuleController controller, Rule rule)
    {
        return Operator.Evaluate(controller, rule, this);
    }
}
