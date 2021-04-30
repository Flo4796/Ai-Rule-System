using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents HUB of Rule System. Processes Rule update and Handles change-requests.
/// </summary>

namespace AdelicSystem.RuleAI
{
    public class RuleController : MonoBehaviour
    {
        public BehaviourProfile Profile;
        List<Rule> potentialNextRule = new List<Rule>();
        List<Rule> activeRulePool = new List<Rule>();

        private void Update()
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
                else if (highestPotential.MyAction.CanInterupt())
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
            foreach (Rule activeRule in activeRulePool)
            {
                activeRule.MyAction.Execute(this);
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
            return activeRulePool.Contains(rule);
        }

        /// <summary>
        /// Updates Quality of Active Rules and set highest quality as threshold.
        /// </summary>
        private void RequalifyActivePool()
        {

            foreach (Rule activeRule in activeRulePool)
            {
                activeRule.MakeQualityDecision(this);
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
