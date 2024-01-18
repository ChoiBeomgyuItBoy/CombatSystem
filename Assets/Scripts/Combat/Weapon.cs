using CombatSystem.Attributes;
using CombatSystem.Movement;
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
                    health.TakeDamage(CalculateDamage(weaponData, attack));
                }

                Mover mover = hit.transform.GetComponent<Mover>();

                if(mover != null && mover != user.GetComponent<Mover>())
                {
                    mover.AddForce(CalculateKnockback(user, health.gameObject, attack));
                }
            }
        }

        float CalculateDamage(WeaponData weaponData, WeaponAttack attack)
        {
            float baseDamage = weaponData.GetBaseDamage();
            float bonusPercentage = attack.GetBonusPercentage();
            float bonus = baseDamage * bonusPercentage;
            return Mathf.Round(baseDamage + bonus);
        }

        Vector3 CalculateKnockback(GameObject user, GameObject target, WeaponAttack attack)
        {
            Vector3 userPosition = user.transform.position;
            Vector3 targetPosition = target.transform.position;
            Vector3 direction = (targetPosition - userPosition).normalized;
            return direction * attack.GetKnockback();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(damageCenter.position, damageRadius);
        }
    }
}
