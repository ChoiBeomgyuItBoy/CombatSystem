using CombatSystem.Control;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Movement
{
    public class Dodger : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] float dodgeCooldownTime = 2;
        [SerializeField] [Range(0,1)] float dodgeDuration = 2;
        [SerializeField] [Range(0,1)] float dodgeSpeedFraction = 0.6f;
        InputReader inputReader;
        Mover mover;
        Vector2 lastFrameDirection;
        float timeSinceLastDodge = Mathf.Infinity;
        float remainingDodgeTime = 0;

        void Awake()
        {
            inputReader = GetComponent<InputReader>();
            mover = GetComponent<Mover>();
        }

        void Update()
        {
            timeSinceLastDodge += Time.deltaTime;
        }

        void StartDodge(Vector3 direction)
        {
            remainingDodgeTime = dodgeDuration;
            lastFrameDirection = direction;
        }

        void StartDirectionalDodge()
        {
            StartDodge(inputReader.GetInputValue());
        }

        void StartFixedDodge()
        {
            StartDodge(Vector2.up);
        }

        void DodgeMovement()
        {
            mover.MoveTo(GetDodgeDirection(), dodgeSpeedFraction);
        }

        Vector3 GetDodgeDirection()
        {
            Vector3 direction = new();

            Vector3 right = transform.right * lastFrameDirection.x * dodgeDuration;
            Vector3 forward = transform.forward * lastFrameDirection.y * dodgeDuration;

            remainingDodgeTime -= Time.deltaTime;

            if(remainingDodgeTime > 0)
            {
                direction += right;
                direction += forward;
                direction += transform.position;
            }
            else
            {
                timeSinceLastDodge = 0;
            }

            return direction;
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Start Directional Dodge":
                    StartDirectionalDodge();
                    break;

                case "Start Fixed Dodge":
                    StartFixedDodge();
                     break;

                case "Dodge Movement":
                    DodgeMovement();
                    break;
            }
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "Can Dodge":
                    return dodgeCooldownTime < timeSinceLastDodge;

                case "Dodge Finished":
                    return remainingDodgeTime <= 0;
            }

            return null;
        }
    }
}
