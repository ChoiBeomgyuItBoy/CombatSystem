using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Attributes;
using CombatSystem.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace CombatSystem.Abilites.Targeting
{
    [CreateAssetMenu(menuName = "Abilities/Targeting/Delayed Follow Targeting")]
    public class DelayedFollowTargeting : TargetingStrategy
    {
        [SerializeField] TargetFollower follower;
        [SerializeField] float areaAffectRadius = 5;
        [SerializeField] float followTime = 5;

        public override void StartTargeting(AbilityData data, Action finished)
        {
            data.StartCoroutine(TargetingRoutine(data, finished));
        }

        IEnumerator TargetingRoutine(AbilityData data, Action finished)
        {
            GameObject user = data.GetUser();
            Health target = user.GetComponent<Fighter>().GetTarget();
            TargetFollower followerInstance = Instantiate(follower, target.transform.position, Quaternion.identity);

            followerInstance.GetComponent<NavMeshAgent>().Warp(target.transform.position);

            followerInstance.transform.localScale = new Vector3(areaAffectRadius, 1, areaAffectRadius);
            followerInstance.SetTarget(target);

            float currentTime = 0;

            while(currentTime < followTime)
            {
                if(data.IsCancelled())
                {
                    break;
                }

                currentTime += Time.deltaTime;

                yield return null;
            }
            
            data.SetTargets(GetTargetsInRadius(followerInstance.transform.position));
            data.SetTargetPoint(followerInstance.transform.position);
            
            Destroy(followerInstance.gameObject);
            finished();
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