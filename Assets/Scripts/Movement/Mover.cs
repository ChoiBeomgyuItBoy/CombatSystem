using UnityEngine;
using UnityEngine.AI;
using CombatSystem.Core;
using RainbowAssets.Utils;

namespace CombatSystem.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] float maxSpeed = 6;
        [SerializeField] float destinationTolerance = 1;
        NavMeshAgent agent;
        AnimationPlayer animationPlayer;

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.enabled = true;
            agent.destination = transform.position + destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
        }

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animationPlayer = GetComponent<AnimationPlayer>();
        }

        void Update()
        {
            UpdateParameters();
        }

        void UpdateParameters()
        {
            animationPlayer.UpdateParameter("movementSpeed", GetMovementMagnitude());
            animationPlayer.UpdateParameter("forwardSpeed", GetForwardVelocity());
            animationPlayer.UpdateParameter("rightSpeed", GetRightVelocity());
        }

        float GetMovementMagnitude()
        {
            return transform.InverseTransformDirection(agent.velocity).magnitude;
        }

        float GetForwardVelocity()
        {
            return transform.InverseTransformDirection(agent.velocity).z;
        }

        float GetRightVelocity()
        {
            return transform.InverseTransformDirection(agent.velocity).x;
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Move No Destination":
                    MoveTo(Vector3.zero, 0);
                    break;
            }
        }
    }
}
