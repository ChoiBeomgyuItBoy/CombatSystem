using System;
using CombatSystem.Core;
using UnityEngine;

namespace CombatSystem.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Knockback Effect")]
    public class KnockbackEffect : EffectStrategy
    {
        [SerializeField] Vector2 knockback;

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach(var target in data.GetTargets())
            {
                ForceReceiver forceReceiver = target.GetComponent<ForceReceiver>();

                if(forceReceiver != null)
                {
                    forceReceiver.AddKnockback(data.GetUser(), knockback);
                }
            }

            finished();
        }
    }
}