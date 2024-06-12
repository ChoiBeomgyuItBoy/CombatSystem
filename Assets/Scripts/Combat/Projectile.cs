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
                health.TakeDamage(damage);
            }

            ForceReceiver forceReceiver = other.GetComponent<ForceReceiver>();

            if(forceReceiver != null && forceReceiver != instigator.GetComponent<ForceReceiver>())
            {
                forceReceiver.AddForce(GetKnockbackDirection(forceReceiver.transform));
            }

            Destroy(gameObject);
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

        Vector3 GetKnockbackDirection(Transform target)
        {
            Vector3 direction = (target.position - instigator.transform.position).normalized;

            direction.y += knockback.y;
            direction.x *= knockback.x;
            direction.z *= knockback.x;

            return direction;
        }
    }
}