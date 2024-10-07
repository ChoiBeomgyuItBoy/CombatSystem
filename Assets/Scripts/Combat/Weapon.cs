using CombatSystem.Attributes;
using CombatSystem.Core;
using UnityEngine;

namespace CombatSystem.Combat
{
    public class Weapon : MonoBehaviour 
    { 
        [SerializeField] Transform hitboxCenter;
        [SerializeField] float hitboxRadius = 0.5f;
        [SerializeField] GameObject attackEffect;
        [SerializeField] GameObject hitEffect;

        public void Hit(GameObject user, float damage, Vector2 knockback)
        {
            if(attackEffect != null)
            {
                Instantiate(attackEffect, transform.position, transform.rotation);
            }

            var hits = Physics.SphereCastAll(hitboxCenter.position, hitboxRadius, Vector3.up, 0);

            foreach(var hit in hits)
            {
                if(hit.transform.CompareTag(user.tag))
                {
                    continue;
                }

                Health health = hit.transform.GetComponent<Health>();

                if(health != null && health != user.GetComponent<Health>())
                {
                    if(health.IsDead())
                    {
                        continue;
                    }

                    if(health.IsInvulnerable())
                    {
                        continue;
                    }

                    ForceReceiver forceReceiver = hit.transform.GetComponent<ForceReceiver>();

                    if(forceReceiver != null && forceReceiver != user.GetComponent<ForceReceiver>())
                    {
                        forceReceiver.AddKnockback(user.transform.position, knockback);
                    }

                    health.TakeDamage(damage);

                    if(hitEffect != null)
                    {
                        Instantiate(hitEffect, transform.position, transform.rotation);
                    }
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitboxCenter.position, hitboxRadius);
        }
    }
}