using System;
using UnityEngine;

namespace CombatSystem.Abilites
{
    [CreateAssetMenu(menuName = "Abilities/New Ability")]
    public class Ability : ScriptableObject
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        public event Action abilityFinished;

        public void Use(GameObject user)
        {
            AbilityData data = new(user);

            targetingStrategy.StartTargeting(data, () => TargetAquired(data));
        }

        void TargetAquired(AbilityData data)
        {
            foreach(var filter in filterStrategies)
            {
                data.SetTargets(filter.Filter(data.GetTargets()));
            }

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