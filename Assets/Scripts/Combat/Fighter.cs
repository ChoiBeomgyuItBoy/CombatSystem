using UnityEngine;
using Cinemachine;
using RainbowAssets.Utils;
using CombatSystem.Core;
using CombatSystem.Attributes;
using CombatSystem.Movement;

namespace CombatSystem.Combat
{
    public class Fighter : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] WeaponData weaponData;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform leftHand;
        [SerializeField] CinemachineTargetGroup targetGroup;
        [SerializeField] float targetingRange = 20;
        AnimationPlayer animationPlayer;
        Mover mover;
        Health target;
        Weapon weapon;
        int currentAttackIndex = 0;
        bool attackForceApplied = false;
        const float targetGroupRadius = 2;
        const float targetGroupWeight = 1;

        void Awake()
        {
            animationPlayer = GetComponent<AnimationPlayer>();
            mover = GetComponent<Mover>();
        }

        void Start()
        {
            weapon = weaponData.Spawn(rightHand, leftHand);
        }

        WeaponAttack GetCurrentAttack()
        {
            return weaponData.GetAttack(currentAttackIndex);
        }

        float GetCurrentAttackTime()
        {
            return animationPlayer.GetAnimationTime("Attack");
        }

        bool CurrentAttackIsLast()
        {
            return currentAttackIndex == weaponData.GetComboLength() - 1;
        }

        bool CurrentAttackFinished()
        {
            float endTime = GetCurrentAttack().GetEndTime();

            if(GetCurrentAttackTime() < endTime)
            {
                return false;
            }
            
            return true;
        }

        void Attack()
        {
            attackForceApplied = false;
            animationPlayer.PlaySmooth(GetCurrentAttack().GetAnimationName());
        }

        void ApplyAttackForce()
        {
            float forceTime = GetCurrentAttack().GetForceTime();

            if(GetCurrentAttackTime() < forceTime)
            {
                return;
            }

            if(attackForceApplied)
            {
                return;
            }

            Vector3 forceMotion = transform.forward * GetCurrentAttack().GetAttackForce();
            mover.AddForce(forceMotion);
            attackForceApplied = true;
        }

        void CycleCombo()
        {
            currentAttackIndex++;
        }

        void ResetCombo()
        {
            currentAttackIndex = 0;
        }

        // Animation Event
        void Hit()
        {
            weapon.Hit(gameObject, weaponData, GetCurrentAttack());
        }

        bool SelectTarget()
        {
            Health closestTarget = GetClosestTargetOnScreen();

            if(closestTarget != null)
            {
                SetTarget(closestTarget);
                return true;
            }

            return false;
        }

        Health GetClosestTargetOnScreen()
        {
            var hits = Physics.SphereCastAll(transform.position, targetingRange, Vector3.up, 0);
            float closestTargetDistance = Mathf.Infinity;
            Health closestTarget = null;

            foreach(var hit in hits)
            {
                Health target = hit.transform.GetComponent<Health>();

                if(target != null && target != GetComponent<Health>())
                {
                    if(target.IsDead())
                    {
                        continue;
                    }

                    Renderer renderer = target.GetComponentInChildren<Renderer>();

                    if(!renderer.isVisible)
                    {
                        continue;
                    }

                    Vector2 targetPosition = Camera.main.WorldToViewportPoint(target.transform.position);
                    Vector2 centerToTarget = targetPosition - new Vector2(0.5f, 0.5f);
                    
                    if(centerToTarget.sqrMagnitude > closestTargetDistance)
                    {
                        continue;
                    }

                    closestTargetDistance = centerToTarget.sqrMagnitude;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        void SetTarget(Health target)
        {
            this.target = target;

            target.onDie.AddListener(CancelTarget);

            if(targetGroup != null)
            {
                targetGroup.AddMember(target.transform, targetGroupWeight, targetGroupRadius);
            }
        }

        void CancelTarget()
        {
            if(target != null)
            {
                if(targetGroup != null)
                {
                    targetGroup.RemoveMember(target.transform);
                }

                target.onDie.RemoveListener(CancelTarget);

                target = null;
            }
        }

        void FaceTarget()
        {
            if(target != null)
            {
                Vector3 lookDirection = target.transform.position - transform.position;
                lookDirection.y = 0;
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        bool TargetInRange()
        {
            if(target != null)
            {
                float targetDistance = (transform.position - target.transform.position).magnitude;
                return targetDistance <= targetingRange;
            }

            return false;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, targetingRange);
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Attack":
                    Attack();
                    break;

                case "Apply Attack Force":
                    ApplyAttackForce();
                    break;
                
                case "Cycle Combo":
                    CycleCombo();
                    break;

                case "Reset Combo":
                    ResetCombo();
                    break;
                    
                case "Face Target":
                    FaceTarget();
                    break;

                case "Cancel Target":
                    CancelTarget();
                    break;
            }
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "Current Attack Finished":
                    return CurrentAttackFinished();

                case "Current Attack Is Last":
                    return CurrentAttackIsLast();

                case "Select Target":
                    return SelectTarget();

                case "Target In Range":
                    return TargetInRange();
            }

            return null;
        }
    }
}