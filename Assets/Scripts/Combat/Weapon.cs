using CombatSystem.Attributes;
using UnityEngine;

namespace CombatSystem.Combat
{
    public class Weapon : MonoBehaviour 
    { 
        [SerializeField] Transform damageCenter;
        [SerializeField] float damageRadius = 0.5f;

        public void Hit(GameObject user, float damage)
        {
            var hits = Physics.SphereCastAll(damageCenter.position, damageRadius, Vector3.up, 0);

            foreach(var hit in hits)
            {
                Health health = hit.transform.GetComponent<Health>();

                if(health != null && health != user.GetComponent<Health>())
                {
                    health.TakeDamage(damage);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(damageCenter.position, damageRadius);
        }
    }
}
