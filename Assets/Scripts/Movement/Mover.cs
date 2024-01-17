using UnityEngine;
using UnityEngine.AI;
using CombatSystem.Control;
using CombatSystem.Core;
using RainbowAssets.Utils;

namespace CombatSystem.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] float maxSpeed = 6;
        NavMeshAgent agent;
        AnimationPlayer animationPlayer;
        InputReader inputReader;
        Transform mainCamera;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animationPlayer = GetComponent<AnimationPlayer>();
            inputReader = GetComponent<InputReader>();
            mainCamera = Camera.main.transform;
        }

        void Update()
        {
            animationPlayer.UpdateParameter("movementSpeed", GetMovementSpeed());
            animationPlayer.UpdateParameter("forwardSpeed", GetForwardSpeed());
            animationPlayer.UpdateParameter("rightSpeed", GetRightSpeed());
        }

        float GetMovementSpeed()
        {
            return transform.InverseTransformDirection(agent.velocity).magnitude;
        }

        float GetForwardSpeed()
        {
            return transform.InverseTransformDirection(agent.velocity).z;
        }

        float GetRightSpeed()
        {
            return transform.InverseTransformDirection(agent.velocity).x;
        }

        void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.destination = transform.position + destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
        }

        Vector3 GetFreeLookDirection()
        {
            Vector2 inputValue = GetInputValue();

            Vector3 right = (inputValue.x * mainCamera.right).normalized;
            right.y = 0;

            Vector3 forward = (inputValue.y * mainCamera.forward).normalized;
            forward.y = 0;

            return right + forward;
        }

        Vector3 GetTargetingDirection()
        {
            Vector2 inputValue = GetInputValue();

            Vector3 direction = new();
            Vector3 right = transform.right * inputValue.x;
            Vector3 forward = transform.forward * inputValue.y;

            direction += right;
            direction += forward;

            return direction;
        }

        Vector2 GetInputValue()
        {
            return inputReader.GetInputAction("Locomotion").ReadValue<Vector2>();
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Free Look Movement":
                    MoveTo(GetFreeLookDirection(), float.Parse(parameters[0]));  
                    break;

                case "Targeting Movement":
                    MoveTo(GetTargetingDirection(), float.Parse(parameters[0]));
                    break;
            }
        }
    }
}
