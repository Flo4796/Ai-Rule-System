using AdelicSystems.RuleAI;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents HUB of Rule System. Processes Rule update and Handles change-requests.
/// </summary>

namespace AdelicSystem.RuleAI
{
    public class RuleController : MonoBehaviour
    {
        // Information Properties
        public float HighesActiveQuality { get { return ActiveQuality; } }
        public string CurrentActiveRule { get { return ActiveRule; } }

        private float ActiveQuality = -1f;
        private string ActiveRule = "No Active Rule";
        Dictionary<SequenceAction, int> SequenceTracker = new Dictionary<SequenceAction, int>();

        public BehaviourProfile Profile;
        List<Rule> potentialNextRule = new List<Rule>();
        List<Rule> activeRulePool = new List<Rule>();

        protected void UpdateRuleSystem()
        {
            // Check activeRule list
            if (activeRulePool.Count > 0)
            {
                for (int i = activeRulePool.Count - 1; i >= 0; i--)
                {
                    // Remove if an ActiveRule is complete
                    if (activeRulePool[i].MyAction.IsComplete(this))
                    {
                        activeRulePool[i].MyAction.OnExitAction(this);
                        activeRulePool.RemoveAt(i);
                    }
                }
            }

            // Evaluate scheduled Rules
            if (potentialNextRule.Count > 0)
            {
                RequalifyActivePool();
                Rule highestPotential = GetHighestPotentialRule();

                if (highestPotential.MyAction.CanDoBoth(activeRulePool.ToArray()))
                {
                    highestPotential.MyAction.OnEnterAction(this);
                    activeRulePool.Add(highestPotential);
                    potentialNextRule.Clear();
                }
                else if (highestPotential.MyAction.CanInterupt() && highestPotential.Quality > ActiveQuality)
                {
                    InteruptActivePool();
                    highestPotential.MyAction.OnEnterAction(this);
                    activeRulePool.Add(highestPotential);
                    potentialNextRule.Clear();
                }
                else if (activeRulePool.Count == 0)
                {
                    highestPotential.MyAction.OnEnterAction(this);
                    activeRulePool.Add(highestPotential);
                    potentialNextRule.Clear();
                }
            }

            // Update Active Rules
            ActiveRule = "No Active Rule";
            foreach (Rule activeRule in activeRulePool)
            {
                activeRule.MyAction.Execute(this);
                ActiveRule = activeRule.RuleName;
            }
        }
        /// <summary>
        /// External-Handle. Allows rule posing by external systems.
        /// </summary>
        /// <param name="rule">Rule to pose.</param>
        public void PoseRule(Rule rule)
        {
            if (!potentialNextRule.Contains(rule))
            {
                potentialNextRule.Add(rule);
            }
        }

        /// <summary>
        /// Checks whether param rule is already active in pool.
        /// </summary>
        /// <param name="rule">Rule to check</param>
        /// <returns></returns>
        public bool IsRuleActive(Rule rule)
        {
            if (rule != null)
            {
                foreach (Rule Active in activeRulePool)
                {
                    if (Active.MyAction == rule.MyAction)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Subscribes a Starting Sequence composite to Rule Controller. Allowing Sequence position tracking.
        /// </summary>
        /// <param name="sequence"></param>
        public void SubscribeSequence(SequenceAction sequence)
        {
            if (!SequenceTracker.ContainsKey(sequence))
                SequenceTracker.Add(sequence, 0);
        }

        /// <summary>
        /// Unsubscribe a Finished Sequence composite from this RuleController.
        /// </summary>
        /// <param name="sequence"></param>
        public void UnsubScribeSequence(SequenceAction sequence)
        {
            if (SequenceTracker.ContainsKey(sequence))
                SequenceTracker.Remove(sequence);
        }

        /// <summary>
        /// Returns Index from Sequence
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public int GetSequenceIndex(SequenceAction sequence)
        {
            if (SequenceTracker.ContainsKey(sequence))
                return SequenceTracker[sequence];
            return -1;
        }

        /// <summary>
        /// Increments refrecenced sequence index by +1.
        /// </summary>
        /// <param name="sequence"></param>
        public void IncrementSequence(SequenceAction sequence)
        {
            if (SequenceTracker.ContainsKey(sequence))
                SequenceTracker[sequence] += 1;
        }


        /// <summary>
        /// Updates Quality of Active Rules and set highest quality as threshold.
        /// </summary>
        private void RequalifyActivePool()
        {
            ActiveQuality = 0f;
            foreach (Rule activeRule in activeRulePool)
            {
                float Quality = activeRule.MakeQualityDecision(this);
                if(Quality > ActiveQuality)
                {
                    ActiveQuality = Quality;
                }
            }
        }

        /// <summary>
        /// Evaluates Quality decision of potential <see cref="Rule"/>.
        /// </summary>
        /// <returns>Highest quality rule</returns>
        private Rule GetHighestPotentialRule()
        {
            Rule highestQtRule = null;

            foreach (Rule potentialRule in potentialNextRule)
            {
                potentialRule.MakeQualityDecision(this);
                if (highestQtRule == null || highestQtRule.Quality < potentialRule.Quality)
                {
                    highestQtRule = potentialRule;
                }
            }
            potentialNextRule.Remove(highestQtRule);
            return highestQtRule;
        }
        /// <summary>
        /// Interupts all Active actions.
        /// </summary>
        private void InteruptActivePool()
        {
            foreach (Rule activeRule in activeRulePool)
            {
                activeRule.MyAction.OnExitAction(this);
            }
            activeRulePool.Clear();
        }
    }
}
