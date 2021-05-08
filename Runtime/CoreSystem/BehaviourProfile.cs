

using System.Collections.Generic;
using UnityEditor;
/// <summary>
/// Represents a dataset for an entity that uses a <see cref="RuleController"/>. 
/// Contains arrays of <see cref="Rule"/>.
/// </summary>
namespace AdelicSystem.RuleAI
{
    public enum Set { Set0, Set1, Set2, Set3, Set4, Set5, NoSet};
    [System.Serializable]
    public class BehaviourProfile
    {
        public Rule[] Set0;
        public Rule[] Set1;
        public Rule[] Set2;
        public Rule[] Set3;
        public Rule[] Set4;
        public Rule[] Set5;


        // Services
        public bool AddToSet(Set set, Rule rule)
        {
            switch (set)
            {
                case Set.Set0:
                    if(!ArrayUtility.Contains(Set0, rule))
                    {
                        ArrayUtility.Add(ref Set0, rule);
                        return true;
                    }
                    break;
                case Set.Set1:
                    if (!ArrayUtility.Contains(Set1, rule))
                    {
                        ArrayUtility.Add(ref Set1, rule);
                        return true;
                    }
                    break;
                case Set.Set2:
                    if (!ArrayUtility.Contains(Set2, rule))
                    {
                        ArrayUtility.Add(ref Set2, rule);
                        return true;
                    }
                    break;
                case Set.Set3:
                    if (!ArrayUtility.Contains(Set3, rule))
                    {
                        ArrayUtility.Add(ref Set3, rule);
                        return true;
                    }
                    break;
                case Set.Set4:
                    if (!ArrayUtility.Contains(Set4, rule))
                    {
                        ArrayUtility.Add(ref Set4, rule);
                        return true;
                    }
                    break;
                case Set.Set5:
                    if (!ArrayUtility.Contains(Set5, rule))
                    {
                        ArrayUtility.Add(ref Set5, rule);
                        return true;
                    }
                    break;
            }return false;
        }

        public bool RemoveFromSet(Set set, Rule rule)
        {
            switch (set)
            {
                case Set.Set0:
                    if (ArrayUtility.Contains(Set0, rule))
                    {
                        ArrayUtility.Remove(ref Set0, rule);
                        return true;
                    }
                    break;
                case Set.Set1:
                    if (ArrayUtility.Contains(Set1, rule))
                    {
                        ArrayUtility.Remove(ref Set1, rule);
                        return true;
                    }
                    break;
                case Set.Set2:
                    if (ArrayUtility.Contains(Set2, rule))
                    {
                        ArrayUtility.Remove(ref Set2, rule);
                        return true;
                    }
                    break;
                case Set.Set3:
                    if (ArrayUtility.Contains(Set3, rule))
                    {
                        ArrayUtility.Remove(ref Set3, rule);
                        return true;
                    }
                    break;
                case Set.Set4:
                    if (ArrayUtility.Contains(Set4, rule))
                    {
                        ArrayUtility.Remove(ref Set4, rule);
                        return true;
                    }
                    break;
                case Set.Set5:
                    if (ArrayUtility.Contains(Set5, rule))
                    {
                        ArrayUtility.Remove(ref Set5, rule);
                        return true;
                    }
                    break;
            }
            return false;
        }

        public KeyValuePair<Set, Rule> FindRuleAndSet(string ruleName)
        {
            if(Set0.Length > 0)
            {
                foreach (Rule rule in Set0)
                {
                    if(rule.RuleName.ToUpper() == ruleName.ToUpper())
                    {
                        return new KeyValuePair<Set, Rule>(Set.Set0, rule);
                    }
                }
            }
            if (Set1.Length > 0)
            {
                foreach (Rule rule in Set1)
                {
                    if (rule.RuleName.ToUpper() == ruleName.ToUpper())
                    {
                        return new KeyValuePair<Set, Rule>(Set.Set1, rule);
                    }
                }
            }
            if (Set2.Length > 0)
            {
                foreach (Rule rule in Set2)
                {
                    if (rule.RuleName.ToUpper() == ruleName.ToUpper())
                    {
                        return new KeyValuePair<Set, Rule>(Set.Set2, rule);
                    }
                }
            }
            if (Set3.Length > 0)
            {
                foreach (Rule rule in Set3)
                {
                    if (rule.RuleName.ToUpper() == ruleName.ToUpper())
                    {
                        return new KeyValuePair<Set, Rule>(Set.Set3, rule);
                    }
                }
            }
            if (Set4.Length > 0)
            {
                foreach (Rule rule in Set4)
                {
                    if (rule.RuleName.ToUpper() == ruleName.ToUpper())
                    {
                        return new KeyValuePair<Set, Rule>(Set.Set4, rule);
                    }
                }
            }
            if (Set5.Length > 0)
            {
                foreach (Rule rule in Set5)
                {
                    if (rule.RuleName.ToUpper() == ruleName.ToUpper())
                    {
                        return new KeyValuePair<Set, Rule>(Set.Set5, rule);
                    }
                }
            }
            return new KeyValuePair<Set, Rule>(Set.NoSet, null);
        }

        public Set FindSetByRule(string ruleName)
        {
            KeyValuePair<Set, Rule> result = FindRuleAndSet(ruleName);
            return result.Key;
        }

        public Set FindSetByRule(Rule rule)
        {
            KeyValuePair<Set, Rule> result = FindRuleAndSet(rule.RuleName);
            return result.Key;
        }

#if UNITY_EDITOR
        public EditorFilters MyFilters;
#endif
    }
}


