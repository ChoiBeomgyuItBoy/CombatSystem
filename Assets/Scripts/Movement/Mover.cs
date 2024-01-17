using CombatSystem.Control;
using RainbowAssets.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace CombatSystem.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] float maxSpeed = 6;
        NavMeshAgent agent;
        Animator animator;
        InputReader inputReader;
        Transform mainCamera;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            inputReader = GetComponent<InputReader>();
            mainCamera = Camera.main.transform;
        }

        void Update()
        {
            animator.SetFloat("movementSpeed", GetLocalVelocity());
        }

        float GetLocalVelocity()
        {
            return transform.InverseTransformDirection(agent.velocity).magnitude;
        }

        void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.destination = transform.position + destination;
           agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
        }

        Vector3 GetFreeLookDirection()
        {
            Vector2 inputValue = inputReader.GetInputAction("Locomotion").ReadValue<Vector2>();

            Vector3 right = (inputValue.x * mainCamera.right).normalized;
            right.y = 0;

            Vector3 forward = (inputValue.y * mainCamera.forward).normalized;
            forward.y = 0;

            return right + forward;
        }

        Vector3 GetTargetingDirection()
        {
            Vector3 direction = new();
            Vector2 inputValue = inputReader.GetInputAction("Locomotion").ReadValue<Vector2>();
            Vector3 right = transform.right * inputValue.x;
            Vector3 forward = transform.forward * inputValue.y;

            direction += right;
            direction += forward;

            return direction;
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
