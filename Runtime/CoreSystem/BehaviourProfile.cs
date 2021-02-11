/// <summary>
/// Represents a dataset for an entity that uses a <see cref="RuleController"/>. 
/// Contains arrays of <see cref="Rule"/>.
/// </summary>
[System.Serializable]
public class BehaviourProfile
{
    public Rule[] Set0;
    public Rule[] Set1;
    public Rule[] Set2;

#if UNITY_EDITOR
    public EditorFilters MyFilters;
#endif
}
