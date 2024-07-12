using System;
using CombatSystem.Attributes;
using CombatSystem.Combat;
using UnityEngine;

namespace CombatSystem.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Spawn Projectile Effect")]
    public class SpawnProjectileEffect : EffectStrategy
    {
        [SerializeField] Projectile projectile;
        [SerializeField] float damage = 30;
        [SerializeField] Vector2 knockback;
        [SerializeField] bool isLeftHanded = false;
        [SerializeField] bool useTargetPoint = true;

        public override void StartEffect(AbilityData data, Action finished)
        {
            Fighter fighter = data.GetUser().GetComponent<Fighter>();
            Vector3 spawnPosition = fighter.GetHand(isLeftHanded).position;

            if(useTargetPoint)
            {
                LaunchProjectileForTargetPoint(data, spawnPosition);
            }
            else
            {
                LaunchProjectileForTargets(data, spawnPosition);
            }   

            finished();
        }

        void LaunchProjectileForTargetPoint(AbilityData data, Vector3 spawnPosition)
        {
            Projectile projectileInstance = Instantiate(projectile, spawnPosition, Quaternion.identity);
            projectileInstance.SetData(data.GetUser(), spawnPosition, damage, knockback, data.GetTargetPoint());
        }

        void LaunchProjectileForTargets(AbilityData data, Vector3 spawnPosition)
        {
            foreach(var target in data.GetTargets())
            {
                Health health = target.GetComponent<Health>();

                if(health != null)
                {
                    Projectile projectileInstance = Instantiate(projectile, spawnPosition, Quaternion.identity);
                    projectileInstance.SetData(data.GetUser(), spawnPosition, damage, knockback, health);
                }
            }
        }
    }
}