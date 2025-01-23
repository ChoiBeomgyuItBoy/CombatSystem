using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RainbowAssets.Utils;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] float maxSpeed = 6;
        [SerializeField] float pointTolerance = 1;
        [SerializeField] bool updateRotation = true;
        NavMeshAgent agent;
        AnimationPlayer animationPlayer;

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.destination = destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
        }

        public bool AtDestination(Vector3 destination)
        {
            return Vector3.Distance(transform.position, destination) < pointTolerance;
        }

        public void LookAt(Vector3 target)
        {
            Vector3 lookDirection = target - transform.position;
            lookDirection.y = 0;

            if(lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        public void Warp(Vector3 position)
        {
            agent.Warp(position);
        }

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animationPlayer = GetComponent<AnimationPlayer>();
        }

        void Start()
        {
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
