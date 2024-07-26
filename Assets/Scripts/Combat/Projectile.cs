using CombatSystem.Attributes;
using CombatSystem.Core;
using UnityEngine;

namespace CombatSystem.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 10;
        [SerializeField] float maxHeight = 5;
        [SerializeField] float maxLifeTime = 10;
        [SerializeField] bool isParabolic = true;
        [SerializeField] GameObject hitEffect;
        [SerializeField] LineRenderer pathLine;
        [SerializeField] float lineStep = 0.1f;
        GameObject instigator;
        float damage;
        Vector3 launchPoint;
        Vector2 knockback;
        Health target;
        Vector3 targetPoint;
        Vector3 direction;
        Vector3 targetInitialPosition;
        float timeSinceLaunched;

        public void SetData(GameObject instigator, Vector3 launchPoint, float damage, Vector2 knockback, Health target)
        {
            SetData(instigator, launchPoint, damage, knockback, target, default);
        }

        public void SetData(GameObject instigator, Vector3 launchPoint, float damage, Vector2 knockback, Vector3 targetPoint)
        {
            SetData(instigator, launchPoint, damage, knockback, null, targetPoint);
        }

        public void DrawPath()
        {
            pathLine.positionCount = GetPathPoints().Length;
            pathLine.SetPositions(GetPathPoints());
        }

        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            timeSinceLaunched += Time.deltaTime;
            FollowPath();
            DrawPath();
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(instigator.tag))
            {
                return;
            }

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
                    forceReceiver.AddKnockback(instigator.transform.position, knockback);
                }

                health.TakeDamage(damage);

                DestroySequence();
            }
        }

        void DestroySequence()
        {
            if(hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation);
            }
            
            Destroy(gameObject);
        }

        void SetData(GameObject instigator, Vector3 launchPoint, float damage, Vector2 knockback, Health target=null, Vector3 targetPoint=default)
        {
            this.instigator = instigator;
            this.launchPoint = launchPoint;
            this.damage = damage;
            this.knockback = knockback;
            this.target = target;
            this.targetPoint = targetPoint;

            CalculateParameters();
            
            Destroy(gameObject, maxLifeTime);
        }

        void CalculateParameters()
        {
            if(target != null)
            {
                targetInitialPosition = target.transform.position;
            }

            direction = GetAimLocation() - launchPoint;
            targetPoint = new(direction.magnitude, direction.y, 0);
        }

        void FollowPath()
        {
            Vector3 nextPosition = CalculatePath(timeSinceLaunched);
            Vector3 directionToNextPosition = nextPosition - transform.position;

            if(directionToNextPosition != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionToNextPosition);
            }

            transform.position = nextPosition;
        }

        Vector3 CalculatePath(float time)
        {
            return isParabolic ? GetParabolicPath(time) : GetLinealPath(time);
        }

        Vector3 GetLinealPath(float time)
        {
            return launchPoint + direction.normalized * (speed * time);
        }

        Vector3 GetParabolicPath(float time)
        {
            float gravity = -Physics.gravity.y;

            float x = GetInitialVelocity() * time * Mathf.Cos(GetAngleToTarget());
            float y = GetInitialVelocity() * time * Mathf.Sin(GetAngleToTarget()) - 0.5f * gravity * Mathf.Pow(time, 2);

            return launchPoint + direction.normalized * x + Vector3.up * y;
        }

        Vector3[] GetPathPoints()
        {
            int pointCount = (int)(GetTimeToTarget() / lineStep);

            Vector3[] pathPoints = new Vector3[pointCount];

            for(int i = 0; i < pointCount - 1; i++)
            {
                pathPoints[i] = CalculatePath(i * lineStep);
            }

            pathPoints[pointCount - 1] = CalculatePath(GetTimeToTarget());

            return pathPoints;
        }

        float GetTimeToTarget()
        {
            return isParabolic ? GetParabolicTime() : GetLinealTime();
        }

        float GetParabolicTime()
        {
            float gravity = -Physics.gravity.y;
            float y = targetPoint.y;

            float a = Mathf.Sqrt(2 * gravity * maxHeight);
            float b = Mathf.Sqrt(2 * gravity * (maxHeight - y));

            float timePlus = (-a - b) / -gravity;
            float timeMinus = (-a + b) / -gravity;

            return timePlus > timeMinus ? timePlus : timeMinus;
        }

        float GetLinealTime()
        {
            return Vector3.Distance(launchPoint, GetAimLocation()) / speed;
        }

        float GetAngleToTarget()
        {
            float gravity = -Physics.gravity.y;
            float x = targetPoint.x;
            float a = Mathf.Sqrt(2 * gravity * maxHeight);
            return Mathf.Atan(a * GetTimeToTarget() / x);
        }

        float GetInitialVelocity()
        {
            float gravity = -Physics.gravity.y;
            float a = Mathf.Sqrt(2 * gravity * maxHeight);
            return a / Mathf.Sin(GetAngleToTarget());
        }

        Vector3 GetAimLocation()
        {
            if(target != null)
            {
                return targetInitialPosition + Vector3.up * GetTargetCenter();
            }

            return targetPoint;
        }

        float GetTargetCenter()
        {
            return target.GetComponent<CharacterController>().height / 2;
        }
    }
}