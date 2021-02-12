#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace AdelicSystem.RuleAI
{
    public enum TypeFilter
    {
        Unknown,
        Unit
    }

    public enum EditorFilter
    {
        Melee,
        Range
    }

    [System.Serializable]
    public class EditorFilters
    {
        public TypeFilter Type = TypeFilter.Unknown;
        public List<Filter> Filters = new List<Filter>();
        public string Search = string.Empty;
    }

    [System.Serializable]
    public class Filter
    {
        public string Name;
        public Color Color;
    }
}
#endif
