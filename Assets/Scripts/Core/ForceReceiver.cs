using RainbowAssets.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace CombatSystem.Core
{
    public class ForceReceiver : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] float forceDrag = 0.3f;
        CharacterController controller;
        Vector3 impact;
        Vector3 damp;
        NavMeshAgent agent;
        float verticalVelocity = 0;

        public void Move()
        {
            if(agent != null)
            {
                agent.enabled = false;
            }

            controller.Move(GetTotalForce() * Time.deltaTime);
        }

        public void AddForce(Vector3 force)
        {
            impact += force;
        }

        public void AddKnockback(GameObject target, Vector2 knockback)
        {
            Vector3 direction = (transform.position - target.transform.position).normalized;

            direction.y += knockback.y;
            direction.x *= knockback.x;
            direction.z *= knockback.x;

            AddForce(direction);
        }

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            CalculateForces();
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

        Vector3 GetTotalForce()
        {
            return impact + Vector3.up * verticalVelocity;
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "Is Grounded":
                    return controller.isGrounded;

                case "Has Impact Magnitude":
                    float threshold = float.Parse(parameters[0]);
                    return impact.magnitude >= threshold;
            }
                
            return null;
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Apply Motion Zero":
                    Move();
                    break;
            }
        }
    }
}
