using UnityEngine;
using UnityEngine.AI;
using CombatSystem.Core;
using RainbowAssets.Utils;

namespace CombatSystem.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] float maxSpeed = 6;
        NavMeshAgent agent;
        AnimationPlayer animationPlayer;

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.enabled = true;
            agent.isStopped = false;

            if(CompareTag("Player"))
            {
                destination += transform.position;
            }

            agent.destination = destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
        }

        public bool AtDestination(Vector3 destination, float tolerance)
        {
            return Vector3.Distance(transform.position, destination) < tolerance;
        }

        public Vector3 GetLocalVelocity()
        {
            return transform.InverseTransformDirection(agent.velocity);
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
            }
        }
    }
}
