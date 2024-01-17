using Cinemachine;
using CombatSystem.Attributes;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Combat
{
    public class Targeter : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] float targetRange = 20;
        [SerializeField] CinemachineTargetGroup targetGroup;
        const float targetGroupRadius = 2;
        const float targetGroupWeight = 1;
        Health currentTarget;

        bool SelectTarget()
        {
            var hits = Physics.SphereCastAll(transform.position, targetRange, Vector3.up, 0);

            foreach(var hit in hits)
            {
                Health target = hit.transform.GetComponent<Health>();

                if(target != null)
                {
                    SetTarget(target);
                    return true;
                }
            }

            return false;
        }

        void SetTarget(Health target)
        {
            currentTarget = target;
            targetGroup.AddMember(target.transform, targetGroupWeight, targetGroupRadius);
        }

        void CancelTarget()
        {
            if(currentTarget != null)
            {
                targetGroup.RemoveMember(currentTarget.transform);
                currentTarget = null;
            }
        }

        void FaceTarget()
        {
            if(currentTarget != null)
            {
                Vector3 lookPosition = currentTarget.transform.position - transform.position;
                lookPosition.y = 0;
                transform.rotation = Quaternion.LookRotation(lookPosition);
            }
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
                case "Select Target":
                    return SelectTarget();
            }

            return null;
        }
    }
}