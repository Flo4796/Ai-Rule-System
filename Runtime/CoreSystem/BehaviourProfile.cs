/// <summary>
/// Represents a dataset for an entity that uses a <see cref="RuleController"/>. 
/// Contains arrays of <see cref="Rule"/>.
/// </summary>


namespace AdelicSystem.RuleAI
{
    [System.Serializable]
    public class BehaviourProfile
    {
        public Rule[] Set0;
        public Rule[] Set1;
        public Rule[] Set2;
        public Rule[] Set3;
        public Rule[] Set4;
        public Rule[] Set5;

#if UNITY_EDITOR
        public EditorFilters MyFilters;
#endif
    }
}


