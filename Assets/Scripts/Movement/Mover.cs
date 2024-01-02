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

        void IAction.DoAction(string actionID, string[] parameters)
        {
            if(actionID == "Movement Action")
            {
                if(parameters[0] == "Free Look")
                {
                    bool parsed = float.TryParse(parameters[1], out float speed);

                    if(parsed)
                    {
                        MoveTo(GetFreeLookDirection(), speed);
                    }
                }
            }

            if(actionID == "Start Locomotion")
            {
                animator.CrossFadeInFixedTime("Locomotion", 0.1f);
            }
        }
    }
}
