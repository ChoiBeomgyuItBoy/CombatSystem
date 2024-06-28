using CombatSystem.Attributes;
using CombatSystem.Core;
using UnityEngine;

namespace CombatSystem.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 3;
        [SerializeField] float maxLifeTime = 10;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect;
        GameObject instigator;
        float damage;
        Vector2 knockback;
        Health target;
        Vector3 targetPoint;

        public void SetData(GameObject instigator, float damage, Vector2 knockback, Health target)
        {
            SetData(instigator, damage, knockback, target, default);
        }

        public void SetData(GameObject instigator, float damage, Vector2 knockback, Vector3 targetPoint)
        {
            SetData(instigator, damage, knockback, null, targetPoint);
        }

        void Start()
        {
            transform.LookAt(GetAimDirection());
        }

        void Update()
        {
            if(isHoming)
            {
                transform.LookAt(GetAimDirection());
            }

            transform.Translate(speed * Time.deltaTime * Vector3.forward);
        }   

        void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();

            if(health != null && health != instigator.GetComponent<Health>())
            {
                if(health.IsDead())
                {
                    return;
                }

                if(health.IsInvulnerable())
                {
                    return;
                }

                ForceReceiver forceReceiver = other.GetComponent<ForceReceiver>();

                if(forceReceiver != null && forceReceiver != instigator.GetComponent<ForceReceiver>())
                {
                    forceReceiver.AddKnockback(instigator, knockback);
                }

                health.TakeDamage(damage);

                DestroySequence();
            }
        }

        void SetData(GameObject instigator, float damage, Vector2 knockback, Health target=null, Vector3 targetPoint=default)
        {
            this.instigator = instigator;
            this.damage = damage;
            this.knockback = knockback;
            this.target = target;
            this.targetPoint = targetPoint;

            Destroy(gameObject, maxLifeTime);
        }

        void DestroySequence()
        {
            speed = 0;

            if(hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation);
            }
            
            Destroy(gameObject);
        }

        Vector3 GetAimDirection()
        {
            if(target != null)
            {
                return target.transform.position + Vector3.up * GetTargetCenter();
            }

            return targetPoint;
        }

        float GetTargetCenter()
        {
            return target.GetComponent<CharacterController>().height / 2;
        }
    }
}