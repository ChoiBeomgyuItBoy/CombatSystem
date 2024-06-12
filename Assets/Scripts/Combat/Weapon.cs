using CombatSystem.Attributes;
using CombatSystem.Core;
using UnityEngine;

namespace CombatSystem.Combat
{
    public class Weapon : MonoBehaviour 
    { 
        [SerializeField] Transform hitboxCenter;
        [SerializeField] float hitboxRadius = 0.5f;

        public void Hit(GameObject user, float damage, Vector2 knockback)
        {
            var hits = Physics.SphereCastAll(hitboxCenter.position, hitboxRadius, Vector3.up, 0);

            foreach(var hit in hits)
            {
                Health health = hit.transform.GetComponent<Health>();

                if(health != null && health != user.GetComponent<Health>())
                {
                    health.TakeDamage(damage);
                }

                ForceReceiver forceReceiver = hit.transform.GetComponent<ForceReceiver>();

                if(forceReceiver != null && forceReceiver != user.GetComponent<ForceReceiver>())
                {
                    forceReceiver.AddForce(GetKnockbackDirection(user.transform, forceReceiver.transform, knockback));
                }
            }
        }

        Vector3 GetKnockbackDirection(Transform user, Transform target, Vector2 knockback)
        {
            Vector3 direction = (target.position - user.position).normalized;

            direction.y += knockback.y;
            direction.x *= knockback.x;
            direction.z *= knockback.x;

            return direction;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitboxCenter.position, hitboxRadius);
        }
    }
}