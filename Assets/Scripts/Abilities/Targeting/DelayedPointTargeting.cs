using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Attributes;
using CombatSystem.Combat;
using CombatSystem.Control;
using CombatSystem.Core;
using CombatSystem.Movement;
using UnityEngine;

namespace CombatSystem.Abilites.Targeting
{
    [CreateAssetMenu(menuName = "Abilities/Targeting/Delayed Point Targeting")]
    public class DelayedPointTargeting : TargetingStrategy
    {
        [SerializeField] string targetingAnimation = "";
        [SerializeField] Mover movingPoint;
        [SerializeField] float areaAffectRadius = 5;
        [SerializeField] float maxLifeTime = 5;
        [SerializeField] float maxDistance = 20;
        [SerializeField] [Range(0,1)] float pointSpeedFraction = 1;
        const string activationInput = "Attack";

        public override void StartTargeting(AbilityData data, Action finished)
        {
            AnimationPlayer animationPlayer = data.GetUser().GetComponent<AnimationPlayer>();
            animationPlayer.PlaySmooth(targetingAnimation);
            data.StartCoroutine(TargetingRoutine(data, finished));
        }

        IEnumerator TargetingRoutine(AbilityData data, Action finished)
        {
            GameObject user = data.GetUser();
            InputReader inputReader = user.GetComponent<InputReader>();
            Health currentTarget = user.GetComponent<Fighter>().GetTarget();
            Mover pointInstance = Instantiate(movingPoint, user.transform.position, Quaternion.identity);

            pointInstance.Warp(user.transform.position);
            pointInstance.transform.localScale = new Vector3(areaAffectRadius, 1, areaAffectRadius);

            float elapsedTime = 0;

            while(!data.IsCancelled() && elapsedTime < maxLifeTime)
            {
                if(user.CompareTag("Player"))
                {
                    Vector3 pointPosition = pointInstance.transform.position;
                    Vector3 movingDirection = GetMovingDirection(pointPosition, inputReader);

                    if(InDistanceThreshold(movingDirection, user))
                    {
                        pointInstance.MoveTo(movingDirection, pointSpeedFraction);
                    }

                    if(inputReader.WasPressed(activationInput))
                    {
                        break;
                    }
                }
                else
                {
                    pointInstance.MoveTo(currentTarget.transform.position, pointSpeedFraction);
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            data.SetTargets(GetTargetsInRadius(pointInstance.transform.position));
            data.SetTargetPoint(pointInstance.transform.position);
            Destroy(pointInstance.gameObject);
            finished();
        }

        bool InDistanceThreshold(Vector3 pointPosition, GameObject user)
        {
            return Vector3.Distance(user.transform.position, pointPosition) <= maxDistance;
        }

        Vector3 GetMovingDirection(Vector3 pointPosition, InputReader inputReader)
        {
            Vector2 inputValue = inputReader.GetInputValue();
            Transform mainCamera = Camera.main.transform;

            Vector3 right = (inputValue.x * mainCamera.right).normalized;
            right.y = 0;

            Vector3 forward = (inputValue.y * mainCamera.forward).normalized;
            forward.y = 0;

            return right + forward + pointPosition;
        }

        IEnumerable<GameObject> GetTargetsInRadius(Vector3 pointPosition)
        {
            RaycastHit[] hits = Physics.SphereCastAll(pointPosition, areaAffectRadius, Vector3.up, 0);

            foreach(var hit in hits)
            {
                yield return hit.collider.gameObject;
            }
        }
    }
}