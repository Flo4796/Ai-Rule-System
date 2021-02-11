using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

/// <summary>
/// Asset collecter utility for RuleSystem editors.
/// </summary>
public static class RuleSystemUtil
{
    /// <summary>
    /// Captures all objects with a <see cref="RuleController"/> and serializeses them.
    /// </summary>
    /// <returns>Serialized captured controller array</returns>
    public static SerializedObject[] CollectRuleSytemObjects()
    {
        List<SerializedObject> collectedRules = new List<SerializedObject>();

        foreach (RuleController controller in Resources.FindObjectsOfTypeAll<RuleController>())
        {
            collectedRules.Add(new SerializedObject(controller));
        }
        return collectedRules.ToArray();
    }

    /// <summary>
    /// Captures all <see cref="Statement"/> (core of <see cref="Decision"/> logic). Arranges them by type of statments.
    /// </summary>
    /// <returns><see cref=" Dictionary{TKey, TValue}"/> sorted statements.</returns>
    public static Dictionary<StatementType, List<Statement>> CollectDecisionsByType()
    {
        Dictionary<StatementType, List<Statement>> collectedDecisions = new Dictionary<StatementType, List<Statement>>();

        List<Statement> rawCollectedDecisions = new List<Statement>(Resources.FindObjectsOfTypeAll<Statement>());
        if(rawCollectedDecisions == null || rawCollectedDecisions.Count == 0) { throw new Exception("Error no Statements collected!"); }

        foreach (Statement statement in rawCollectedDecisions)
        {
            if(collectedDecisions.ContainsKey(statement.Type))
            {
                collectedDecisions[statement.Type].Add(statement);
            }
            else
            {
                collectedDecisions.Add(statement.Type, new List<Statement> { statement });
            }
        }
        return collectedDecisions;
    }

    /// <summary>
    /// Captures all <see cref="Action"/> assets within project Resources folder. Sorted by set type.
    /// </summary>
    /// <returns><see cref=" Dictionary{TKey, TValue}"/> sorted Actions.</returns>
    public static Dictionary<ActionType, List<Action>> CollectActionsByType()
    {
        Dictionary<ActionType, List<Action>> collectedActions = new Dictionary<ActionType, List<Action>>();

        List<Action> capturedActions = new List<Action>(Resources.FindObjectsOfTypeAll<Action>());

        foreach (Action action in capturedActions)
        {
            if(!collectedActions.ContainsKey(action.Type))
            {
                collectedActions.Add(action.Type, new List<Action> { action });
            }
            else
            {
                collectedActions[action.Type].Add(action);
            }
        }
        return collectedActions;
    }

    /// <summary>
    /// Unwraps <see cref="Rule"/> from its Serialized counterpart.
    /// </summary>
    /// <param name="ruleProperty">Serialized version of rule.</param>
    /// <param name="parentRuleSet">String name representing set containing this Rule.</param>
    /// <param name="ruleIndex">Zero-based index of rule in ruleset array. </param>
    /// <returns></returns>
    public static Rule DeserializeRule(SerializedProperty ruleProperty, string parentRuleSet, int ruleIndex)
    {
        FieldInfo profileInfo = ruleProperty.serializedObject.targetObject.GetType().GetField("Profile");
        var profile = profileInfo.GetValue(ruleProperty.serializedObject.targetObject);
        FieldInfo setInfo = profile.GetType().GetField(parentRuleSet);
        Rule[] set = (Rule[])setInfo.GetValue(profile);
        return set[ruleIndex];
    }

    /// <summary>
    /// Updates Serialized property counterpart of <see cref="Rule"/>.
    /// </summary>
    /// <param name="rule">Updated <see cref="Rule"/> version. </param>
    /// <param name="ruleProperty">Rule serialized counterpart.</param>
    /// <param name="parentRuleSet">String name representing set containing this Rule.</param>
    /// <param name="ruleIndex">Zero-based index of rule in ruleset array. </param>
    public static void SerializeRule(Rule rule, SerializedProperty ruleProperty, string parentRuleSet, int ruleIndex)
    {
        FieldInfo profileInfo = ruleProperty.serializedObject.targetObject.GetType().GetField("Profile");
        var profile = profileInfo.GetValue(ruleProperty.serializedObject.targetObject);
        FieldInfo setInfo = profile.GetType().GetField(parentRuleSet);
        Rule[] set = (Rule[])setInfo.GetValue(profile);
        set[ruleIndex] = rule;

    }

}
