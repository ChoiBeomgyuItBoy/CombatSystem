using System;
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

        public override void StartEffect(AbilityData data, Action finished)
        {
            Fighter fighter = data.GetUser().GetComponent<Fighter>();

            Vector3 spawnPosition = fighter.GetHand(isLeftHanded).position;

            Projectile projectileInstance = Instantiate(projectile, spawnPosition, Quaternion.identity);

            projectileInstance.SetData(data.GetUser(), damage, knockback, data.GetTargetPoint());

            finished();
        }
    }
}