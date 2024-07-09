using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Delay Composite Effect")]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] List<EffectStrategy> delayedEffects = new();
        [SerializeField] float delay = 1;
        [SerializeField] bool abortIfCancelled = false;

        public override EffectStrategy Clone()
        {
            DelayCompositeEffect clone = Instantiate(this);

            clone.delayedEffects.Clear();

            foreach(var effect in delayedEffects)
            {
                clone.delayedEffects.Add(effect.Clone());
            }

            return clone;
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayEffects(data, finished));
        }

        IEnumerator DelayEffects(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(delay);

            if(data.IsCancelled() && abortIfCancelled)
            {
                yield break;
            }

            foreach(var effect in delayedEffects)
            {
                effect.StartEffect(data, () => EffectFinished(effect, finished));
            }
        }

        void EffectFinished(EffectStrategy effect, Action finished)
        {
            if(IsLastEffect(effect))
            {
                finished();
            }
        }

        bool IsLastEffect(EffectStrategy effect)
        {
            return effect == delayedEffects[delayedEffects.Count - 1];
        }
    }
}