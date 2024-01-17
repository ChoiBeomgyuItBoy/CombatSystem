using UnityEngine;
using Cinemachine;
using RainbowAssets.Utils;
using CombatSystem.Core;
using CombatSystem.Attributes;

namespace CombatSystem.Combat
{
    public class Fighter : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] WeaponData weaponData;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform leftHand;
        [SerializeField] float targetRange = 20;
        [SerializeField] CinemachineTargetGroup targetGroup;
        AnimationPlayer animationPlayer;
        Health target;
        Weapon weapon;
        int currentAttackIndex = 0;
        const float targetGroupRadius = 2;
        const float targetGroupWeight = 1;

        void Awake()
        {
            animationPlayer = GetComponent<AnimationPlayer>();
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
            float successTime = GetCurrentAttack().GetEndTime();

            if(GetCurrentAttackTime() < successTime)
            {
                return false;
            }
            
            return true;
        }

        void Attack()
        {
            animationPlayer.PlaySmooth(GetCurrentAttack().GetAnimationName());
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
            var hits = Physics.SphereCastAll(transform.position, targetRange, Vector3.up, 0);

            foreach(var hit in hits)
            {
                Health target = hit.transform.GetComponent<Health>();

                if(target != null && target != GetComponent<Health>())
                {
                    SetTarget(target);
                    return true;
                }
            }

            return false;
        }

        void SetTarget(Health target)
        {
            this.target = target;

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
                return targetDistance <= targetRange;
            }

            return false;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, targetRange);
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Attack":
                    Attack();
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