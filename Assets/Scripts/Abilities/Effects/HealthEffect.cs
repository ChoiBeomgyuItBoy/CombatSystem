using System;
using CombatSystem.Attributes;
using CombatSystem.Core;
using UnityEngine;

namespace CombatSystem.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Health Effect")]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] float healthChange;
        [SerializeField] Vector2 knockback;

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach(var target in data.GetTargets())
            {
                Health health = target.GetComponent<Health>();

                if(health != null)
                {
                    if(healthChange < 0)
                    {
                        health.TakeDamage(-healthChange);
                    }
                    else
                    {
                        health.Heal(healthChange);
                    }
                }

                ForceReceiver forceReceiver = target.GetComponent<ForceReceiver>();

                if(forceReceiver != null)
                {
                    forceReceiver.AddKnockback(data.GetTargetPoint(), knockback);
                }
            }

            finished();
        }
    }
}