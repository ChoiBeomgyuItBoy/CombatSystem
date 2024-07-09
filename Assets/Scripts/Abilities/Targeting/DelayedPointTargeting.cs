using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Control;
using UnityEngine;

namespace CombatSystem.Abilites.Targeting
{
    [CreateAssetMenu(menuName = "Abilities/Targeting/Delayed Point Targeting")]
    public class DelayedPointTargeting : TargetingStrategy
    {
        [SerializeField] GameObject pointPrefab;
        [SerializeField] float areaAffectRadius = 5;
        const string activationInput = "Attack";

        public override void StartTargeting(AbilityData data, Action finished)
        {
            data.StartCoroutine(TargetingRoutine(data, finished));
        }

        IEnumerator TargetingRoutine(AbilityData data, Action finished)
        {
            GameObject user = data.GetUser();
            GameObject pointInstance = Instantiate(pointPrefab, user.transform.position, Quaternion.identity);

            pointInstance.transform.localScale = new Vector3(areaAffectRadius, 1, areaAffectRadius);

            InputReader inputReader = user.GetComponent<InputReader>();

            while(!data.IsCancelled())
            {
                if(inputReader.WasPressed(activationInput))
                {
                    data.SetTargets(GetTargetsInRadius(pointInstance.transform.position));
                    data.SetTargetPoint(pointInstance.transform.position);
                    break;
                }

                yield return null;
            }

            Destroy(pointInstance.gameObject);
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