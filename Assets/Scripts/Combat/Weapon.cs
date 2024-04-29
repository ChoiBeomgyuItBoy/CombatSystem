using CombatSystem.Attributes;
using CombatSystem.Core;
using UnityEngine;

namespace CombatSystem.Combat
{
    public class Weapon : MonoBehaviour 
    { 
        [SerializeField] Transform hitboxCenter;
        [SerializeField] float hitboxRadius = 0.5f;

        public void Hit(GameObject user, WeaponData weaponData, WeaponAttack attack)
        {
            var hits = Physics.SphereCastAll(hitboxCenter.position, hitboxRadius, Vector3.up, 0);

            foreach(var hit in hits)
            {
                Health health = hit.transform.GetComponent<Health>();

                if(health != null && health != user.GetComponent<Health>())
                {
                    health.TakeDamage(CalculateDamage(weaponData, attack));
                }

                ForceReceiver forceReceiver = hit.transform.GetComponent<ForceReceiver>();

                if(forceReceiver != null && forceReceiver != user.GetComponent<ForceReceiver>())
                {
                    forceReceiver.AddForce(CalculateKnockback(user.transform, forceReceiver.transform, attack));
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

        Vector3 CalculateKnockback(Transform user, Transform target, WeaponAttack attack)
        {
            Vector3 direction = (target.position - user.position).normalized;

            direction.y += attack.GetKnockback().y;
            direction.x *= attack.GetKnockback().x;
            direction.z *= attack.GetKnockback().x;

            return direction;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitboxCenter.position, hitboxRadius);
        }
    }
}
