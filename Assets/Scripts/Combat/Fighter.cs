using UnityEngine;
using Cinemachine;
using RainbowAssets.Utils;
using CombatSystem.Core;
using CombatSystem.Attributes;
using CombatSystem.Movement;
using CombatSystem.Control;

namespace CombatSystem.Combat
{
    public class Fighter : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] WeaponData weaponData;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform leftHand;
        [SerializeField] CinemachineTargetGroup targetGroup;
        [SerializeField] float targetingRange = 20;
        [SerializeField] [Range(0,1)] float targetingSpeedFraction = 0.3f;
        [SerializeField] float targetingMoveDistance = 5;
        [SerializeField] float targetingPointTolerance = 2;
        [SerializeField] float targetingDuration = 5;
        [SerializeField] float suspicionDuration = 3;
        [SerializeField] [Range(0,1)] float chaseSpeedFraction = 1;
        AnimationPlayer animationPlayer;
        InputReader inputReader;
        ForceReceiver forceReceiver;
        Mover mover;
        GameObject player;
        Health target;
        Vector3 currentTargetingPoint;
        int currentAttackIndex = 0;
        float timeSinceLastSawTarget = Mathf.Infinity;
        float timeSinceStartedTargeting = Mathf.Infinity;
        bool attackForceApplied = false;
        const float targetGroupRadius = 2;
        const float targetGroupWeight = 1;
        readonly Vector3[] targetingPoints = new Vector3[]
        {
            Vector3.left,
            Vector3.back,
            Vector3.forward,
            Vector3.right
        };

        public Transform GetHand(bool isLeftHanded)
        {
            return isLeftHanded ? leftHand : rightHand;
        }

        void Awake()
        {
            animationPlayer = GetComponent<AnimationPlayer>();
            inputReader = GetComponent<InputReader>();
            forceReceiver = GetComponent<ForceReceiver>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
        }

        void Start()
        {
            EquipWeapon();
        }

        void Update()
        {
            timeSinceLastSawTarget += Time.deltaTime;
            timeSinceStartedTargeting += Time.deltaTime;
        }

        void EquipWeapon()
        {
            if(weaponData != null)
            {
                weaponData = weaponData.Clone();
                weaponData.Spawn(gameObject, rightHand, leftHand, animationPlayer);
            }
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
            forceReceiver.AddForce(forceMotion);
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
            weaponData.Hit(currentAttackIndex, target);
        }

        bool SelectTarget()
        {
            Health closestTarget;

            if(CompareTag("Player"))
            {
                closestTarget = GetClosestTargetOnScreen();
            }
            else
            {
                closestTarget = GetPlayerAsTarget();
            }

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
                Health currentTarget = hit.transform.GetComponent<Health>();

                if(currentTarget != null && currentTarget != GetComponent<Health>())
                {
                    if(currentTarget.IsDead())
                    {
                        continue;
                    }

                    Renderer renderer = currentTarget.GetComponentInChildren<Renderer>();

                    if(!renderer.isVisible)
                    {
                        continue;
                    }

                    Vector2 targetPosition = Camera.main.WorldToViewportPoint(currentTarget.transform.position);
                    Vector2 centerToTarget = targetPosition - new Vector2(0.5f, 0.5f);
                    
                    if(centerToTarget.sqrMagnitude > closestTargetDistance)
                    {
                        continue;
                    }

                    closestTargetDistance = centerToTarget.sqrMagnitude;
                    closestTarget = currentTarget;
                }
            }

            return closestTarget;
        }

        Health GetPlayerAsTarget()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if(distanceToPlayer < targetingRange)
            {
                Health playerHealth = player.GetComponent<Health>();

                if(playerHealth.IsDead())
                {
                    return null;
                }

                return playerHealth;
            }

            return null;
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
                mover.LookAt(target.transform.position);
            }
        }

        bool TargetInRange(float range)
        {
            if(target != null)
            {
                float targetDistance = (transform.position - target.transform.position).magnitude;

                bool inRange =  targetDistance <= range;

                if(inRange)
                {
                    timeSinceLastSawTarget = 0;
                }

                return inRange;
            }

            return false;
        }

        Vector3 GetTargetingDirection()
        {
            Vector3 direction = Vector3.zero;

            if(CompareTag("Player"))
            {
                Vector2 inputValue = inputReader.GetInputValue();
                Vector3 right = transform.right * inputValue.x;
                Vector3 forward = transform.forward * inputValue.y;

                direction += right;
                direction += forward;
            }
            else
            {
                direction = currentTargetingPoint;
            }

            return direction;
        }

        void StartTargeting()
        {
            timeSinceStartedTargeting = 0;
        }

        void TargetingMovement()
        {
            mover.MoveTo(GetTargetingDirection(), targetingSpeedFraction);
        }

        void ChooseRandomTargetingPoint()
        {
            int randomOption = new System.Random().Next(0, targetingPoints.Length - 1);
            currentTargetingPoint = transform.position + targetingPoints[randomOption] * targetingMoveDistance;
        }

        bool AtTargetingPoint()
        {
            return mover.AtDestination(currentTargetingPoint, targetingPointTolerance);
        }

        void ChaseMovement()
        {
            mover.MoveTo(target.transform.position, chaseSpeedFraction);
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

                case "Targeting Movement":
                    TargetingMovement();
                    break;

                case "Start Targeting":
                    StartTargeting();
                    break;

                case "Choose Random Targeting Point":
                    ChooseRandomTargetingPoint();
                    break;

                case "Chase Movement":
                    ChaseMovement();
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

                case "Target In Targeting Range":
                    return TargetInRange(targetingRange);
                
                case "Target In Attack Range":
                    return TargetInRange(weaponData.GetRange());

                case "At Targeting Point":
                    return AtTargetingPoint();

                case "Suspicion Time Finished":
                    return suspicionDuration < timeSinceLastSawTarget;

                case "Targeting Time Finished":
                    return targetingDuration < timeSinceStartedTargeting;
            } 

            return null;
        }
    }
}