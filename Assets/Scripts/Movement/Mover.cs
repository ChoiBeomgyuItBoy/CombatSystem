using UnityEngine;
using UnityEngine.AI;
using CombatSystem.Core;
using RainbowAssets.Utils;

namespace CombatSystem.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] float maxSpeed = 6;
        [SerializeField] float maxDistance = 10;
        [SerializeField] bool updateRotation = true;
        NavMeshAgent agent;
        AnimationPlayer animationPlayer;
        Vector3 initialPosition;

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.enabled = true;
            agent.isStopped = false;

            if(CompareTag("Player"))
            {
                destination += transform.position;
            }

            if(InTreshold(destination))
            {
                agent.destination = destination;
                agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            }
        }

        public bool AtDestination(Vector3 destination, float tolerance)
        {
            return Vector3.Distance(transform.position, destination) < tolerance;
        }

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animationPlayer = GetComponent<AnimationPlayer>();

            initialPosition = transform.position;
            agent.updateRotation = updateRotation;
        }

        void Update()
        {
            UpdateParameters();
        }

        void UpdateParameters()
        {
            if(animationPlayer != null)
            {
                animationPlayer.UpdateParameter("movementSpeed", GetMovementMagnitude());
                animationPlayer.UpdateParameter("forwardSpeed", GetForwardVelocity());
                animationPlayer.UpdateParameter("rightSpeed", GetRightVelocity());
            }
        }

        float GetMovementMagnitude()
        {
            return GetLocalVelocity().magnitude;
        }

        float GetForwardVelocity()
        {
            return GetLocalVelocity().z;
        }

        float GetRightVelocity()
        {
            return GetLocalVelocity().x;
        }

        Vector3 GetLocalVelocity()
        {
            return transform.InverseTransformDirection(agent.velocity);
        }

        bool InTreshold(Vector3 destination)
        {
            if(maxDistance == Mathf.Infinity)
            {
                return true;
            }

            return Vector3.Distance(initialPosition, destination) <= maxDistance;
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Cancel Movement":
                    if(agent.isActiveAndEnabled)
                    {
                        agent.isStopped = true;
                    }

                    break;

                case "Move No Destination":
                    MoveTo(Vector3.zero, 0);
                    break;
            }
        }
    }
}
