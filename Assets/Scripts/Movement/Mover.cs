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
        [SerializeField] float forceDrag = 0.3f;
        NavMeshAgent agent;
        CharacterController controller;
        AnimationPlayer animationPlayer;
        InputReader inputReader;
        Transform mainCamera;
        Vector3 impact;
        Vector3 damp;
        float verticalVelocity = 0;

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.enabled = true;
            agent.destination = transform.position + destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
        }

        public void Move(Vector3 motion)
        {
            agent.enabled = false;
            controller.Move((motion + GetTotalForce()) * Time.deltaTime);
        }

        public void AddForce(Vector3 force)
        {
            impact += force;
        }

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            controller = GetComponent<CharacterController>();
            animationPlayer = GetComponent<AnimationPlayer>();
            inputReader = GetComponent<InputReader>();
            mainCamera = Camera.main.transform;
        }

        void Update()
        {
            UpdateParameters();
            CalculateForces();
        }

        void UpdateParameters()
        {
            animationPlayer.UpdateParameter("movementSpeed", GetMovementSpeed());
            animationPlayer.UpdateParameter("forwardSpeed", GetForwardSpeed());
            animationPlayer.UpdateParameter("rightSpeed", GetRightSpeed());
        }

        void CalculateForces()
        {
            if(controller.isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = Physics.gravity.y * Time.deltaTime;
            }
            else
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }

            impact = Vector3.SmoothDamp(impact, Vector3.zero, ref damp, forceDrag);
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

        Vector3 GetTotalForce()
        {
            return impact + Vector3.up * verticalVelocity;
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

                case "Move No Motion":
                    Move(Vector3.zero);
                    break;
            }
        }
    }
}
