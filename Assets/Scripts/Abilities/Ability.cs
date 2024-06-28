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
        [SerializeField] float cooldownTime = 5;
        public event Action abilityFinished;

        public float GetCooldownTime()
        {
            return cooldownTime;
        }

        public void Use(GameObject user)
        {
            AbilityData data = new(user);

            targetingStrategy.StartTargeting(data, () => TargetAquired(data));
        }

        void TargetAquired(AbilityData data)
        {
            List<GameObject> filteredTargets = new();

            foreach(var filter in filterStrategies)
            {
                filteredTargets.AddRange(filter.Filter(data.GetTargets()));
            }

            data.SetTargets(filteredTargets);

            foreach(var effect in effectStrategies)
            {
                effect.StartEffect(data, () => EffectFinished(effect));
            }   
        }   

        void EffectFinished(EffectStrategy effect)
        {
            if(IsLastEffect(effect))
            {
                abilityFinished.Invoke();
            }
        }

        bool IsLastEffect(EffectStrategy effect)
        {
            return effect == effectStrategies[effectStrategies.Length - 1];
        }
    }
}