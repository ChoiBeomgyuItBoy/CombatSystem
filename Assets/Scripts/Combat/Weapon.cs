using CombatSystem.Attributes;
using UnityEngine;

namespace CombatSystem.Combat
{
    public class Weapon : MonoBehaviour 
    { 
        [SerializeField] Transform damageCenter;
        [SerializeField] float damageRadius = 0.5f;

        public void Hit(GameObject user, WeaponData weaponData, WeaponAttack attack)
        {
            var hits = Physics.SphereCastAll(damageCenter.position, damageRadius, Vector3.up, 0);

            foreach(var hit in hits)
            {
                Health health = hit.transform.GetComponent<Health>();

                if(health != null && health != user.GetComponent<Health>())
                {
                    health.TakeDamage(GetTotalDamage(weaponData, attack));
                }
            }
        }

        float GetTotalDamage(WeaponData weaponData, WeaponAttack attack)
        {
            float baseDamage = weaponData.GetBaseDamage();
            float bonusPercentage = attack.GetBonusPercentage();
            float bonus = baseDamage * bonusPercentage;
            return baseDamage + bonus;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(damageCenter.position, damageRadius);
        }
    }
}
