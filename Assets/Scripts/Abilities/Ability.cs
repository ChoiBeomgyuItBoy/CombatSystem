using System;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Abilites
{
    [CreateAssetMenu(menuName = "Abilities/New Ability")]
    public class Ability : ScriptableObject
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        public event Action finished;

        public void Use(GameObject user)
        {
            targetingStrategy.StartTargeting(user, (IEnumerable<GameObject> targets) => TargetAquired(user, targets));
        }

        void TargetAquired(GameObject user, IEnumerable<GameObject> targets)
        {
            foreach(var filter in filterStrategies)
            {
                targets = filter.Filter(targets);
            }

            foreach(var effect in effectStrategies)
            {
                effect.StartEffect(user, targets, EffectFinished);
            }   
        }   

        void EffectFinished()
        {
            finished.Invoke();
        }
    }
}