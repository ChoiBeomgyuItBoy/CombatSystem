using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Delay Composite Effect")]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] EffectStrategy[] delayedEffects;
        [SerializeField] float delay = 1;
        [SerializeField] bool abortIfCancelled = false;

        public override EffectStrategy Clone()
        {
            DelayCompositeEffect clone = Instantiate(this);

            for(int i = 0; i < delayedEffects.Length; i++)
            {
                clone.delayedEffects[i] = delayedEffects[i].Clone();
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
            return effect == delayedEffects[delayedEffects.Length - 1];
        }
    }
}