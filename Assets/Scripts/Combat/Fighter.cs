using UnityEngine;
using Cinemachine;
using RainbowAssets.Utils;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;
using RPG.Control;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] WeaponData defaultWeapon;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform leftHand;
        [SerializeField] Health target;
        [SerializeField] CinemachineTargetGroup targetGroup;
        [SerializeField] float targetingRange = 20;
        [SerializeField] [Range(0,1)] float targetingSpeedFraction = 0.3f;
        [Header("AI Data")]
        [SerializeField] Vector3[] targetingPoints;
        [SerializeField] float targetingMoveDistance = 5;
        [SerializeField] float targetingDuration = 5;
        [SerializeField] float suspicionDuration = 3;
        [SerializeField] [Range(0,1)] float chaseSpeedFraction = 1;
        AnimationPlayer animationPlayer;
        InputReader inputReader;
        ForceReceiver forceReceiver;
        Mover mover;
        GameObject player;
        WeaponData currentWeapon;
        Vector3 currentTargetingPoint;
        int currentAttackIndex = 0;
        float timeSinceLastSawTarget = Mathf.Infinity;
        float timeSinceStartedTargeting = Mathf.Infinity;
        const float targetGroupRadius = 2;
        const float targetGroupWeight = 1;

        public Transform GetHand(bool isLeftHanded)
        {
            return isLeftHanded ? leftHand : rightHand;
        }

        public Health GetTarget()
        {
            return target;
        }

        public void EquipWeapon(WeaponData weaponData)
        {
            currentWeapon = weaponData.Clone();
            currentWeapon.Spawn(gameObject, rightHand, leftHand, animationPlayer);
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
            EquipWeapon(defaultWeapon);
        }

        void Update()
        {
            timeSinceLastSawTarget += Time.deltaTime;
            timeSinceStartedTargeting += Time.deltaTime;
        }

        WeaponAttack GetCurrentAttack()
        {
            return currentWeapon.GetAttack(currentAttackIndex);
        }

        float GetCurrentAttackTime()
        {
            return animationPlayer.GetAnimationTime("Attack");
        }

        bool CurrentAttackIsLast()
        {
            return currentAttackIndex == currentWeapon.GetComboLength() - 1;
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
            currentWeapon.Hit(currentAttackIndex, target);
            forceReceiver.AddForce(transform.forward * GetCurrentAttack().GetAttackForce());
        }

        bool SelectTarget()
        {
            Health closestTarget;

            if(CompareTag("Player"))
            {
                closestTarget = GetClosestCombatTarget();
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

        Health GetClosestCombatTarget()
        {
            var hits = Physics.SphereCastAll(transform.position, targetingRange, Vector3.up, 0);
            float closestTargetDistance = Mathf.Infinity;
            Health closestTarget = null;

            foreach(var hit in hits)
            {
                CombatTarget combatTarget = hit.transform.GetComponent<CombatTarget>();

                if(combatTarget != null)
                {
                    Health targetHealth = combatTarget.GetComponent<Health>();

                    if(targetHealth.IsDead())
                    {
                        continue;
                    }

                    Renderer renderer = combatTarget.GetComponentInChildren<Renderer>();

                    if(!renderer.isVisible)
                    {
                        continue;
                    }

                    Vector2 targetPosition = Camera.main.WorldToViewportPoint(combatTarget.transform.position);
                    Vector2 centerToTarget = targetPosition - new Vector2(0.5f, 0.5f);
                    
                    if(centerToTarget.sqrMagnitude > closestTargetDistance)
                    {
                        continue;
                    }

                    closestTargetDistance = centerToTarget.sqrMagnitude;
                    closestTarget = targetHealth;
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
                direction += transform.position;
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
            return mover.AtDestination(currentTargetingPoint);
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
                    return TargetInRange(currentWeapon.GetRange());

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