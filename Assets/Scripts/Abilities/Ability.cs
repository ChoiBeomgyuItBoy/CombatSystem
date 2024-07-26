using System;
using System.Collections;
using System.Collections.Generic;
using RainbowAssets.BehaviourTree;
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
        AbilityData currentData;
        float remainingCooldown = 0;
        int finishedEffectCount = 0;

        public Ability Clone()
        {
            Ability clone = Instantiate(this);

            clone.targetingStrategy = targetingStrategy.Clone();

            for(int i = 0; i < filterStrategies.Length; i++)
            {
                clone.filterStrategies[i] = filterStrategies[i].Clone();
            }

            for(int i = 0; i < effectStrategies.Length; i++)
            {
                clone.effectStrategies[i] = effectStrategies[i].Clone();
            }

            return clone;
        }

        public void Cancel()
        {
            currentData.Cancel();
            currentData.StartCoroutine(CooldownRoutine());
            finishedEffectCount = 0;
        }

        public bool CanBeUsed()
        {
            return remainingCooldown <= 0;
        }

        public void Use(GameObject user)
        {
            if(CanBeUsed())
            {
                currentData = new(user);
                targetingStrategy.StartTargeting(currentData, TargetAquired);
            }
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
                effect.StartEffect(currentData, EffectFinished);
            }   
        }   

        void EffectFinished()
        {
            finishedEffectCount++;

            if(finishedEffectCount >= effectStrategies.Length)
            {
                abilityFinished?.Invoke();
                Cancel();
            }
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