using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Abilites
{
    [CreateAssetMenu(menuName = "Abilities/New Ability")]
    public class Ability : ScriptableObject
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] List<FilterStrategy> filterStrategies = new();
        [SerializeField] List<EffectStrategy> effectStrategies = new();
        [SerializeField] float cooldownTime = 5;
        public event Action abilityFinished;
        AbilityData currentData;
        float remainingCooldown = 0;

        public Ability Clone()
        {
            Ability clone = Instantiate(this);

            clone.targetingStrategy = targetingStrategy.Clone();
            clone.filterStrategies.Clear();
            clone.effectStrategies.Clear();

            foreach(var filter in filterStrategies)
            {
                clone.filterStrategies.Add(filter.Clone());
            }

            foreach(var effect in effectStrategies)
            {
                clone.effectStrategies.Add(effect.Clone());
            }

            return clone;
        }

        public void Cancel()
        {
            currentData.Cancel();
            currentData.StartCoroutine(CooldownRoutine());
        }

        public bool Use(GameObject user)
        {
            if(remainingCooldown > 0)
            {
                return false;
            }

            currentData = new(user);

            targetingStrategy.StartTargeting(currentData, TargetAquired);

            return true;
        }

        void TargetAquired()
        {
            if(currentData.IsCancelled())
            {
                return;
            }

            List<GameObject> filteredTargets = new();

            foreach(var filter in filterStrategies)
            {
                filteredTargets.AddRange(filter.Filter(currentData.GetTargets()));
            }

            currentData.SetTargets(filteredTargets);

            foreach(var effect in effectStrategies)
            {
                effect.StartEffect(currentData, () => EffectFinished(effect));
            }   
        }   

        void EffectFinished(EffectStrategy effect)
        {
            if(IsLastEffect(effect))
            {
                abilityFinished?.Invoke();
                Cancel();
            }
        }

        bool IsLastEffect(EffectStrategy effect)
        {
            return effect == effectStrategies[effectStrategies.Count - 1];
        }

        IEnumerator CooldownRoutine()
        {
            remainingCooldown = cooldownTime;

            float startTime = Time.time;

            while(remainingCooldown > 0)
            {
                remainingCooldown = cooldownTime - (Time.time - startTime);

                yield return null;
            }
        }
    }
}