using System;
using System.Collections;
using UnityEngine;

namespace CombatSystem.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Delay Composite Effect")]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] EffectStrategy[] delayedEffects;
        [SerializeField] float delay = 1;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayEffects(data, finished));
        }

        IEnumerator DelayEffects(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(delay);

            foreach(var effect in delayedEffects)
            {
                effect.StartEffect(data, finished);
            }

            finished();
        }
    }
}