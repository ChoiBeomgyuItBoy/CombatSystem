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
            MonoBehaviour monoBehaviour = data.GetUser().GetComponent<MonoBehaviour>();

            monoBehaviour.StartCoroutine(TargetingRoutine(data, finished));
        }

        IEnumerator TargetingRoutine(AbilityData data, Action finished)
        {
            GameObject user = data.GetUser();
            GameObject pointInstance = Instantiate(pointPrefab, user.transform.position, Quaternion.identity);

            pointInstance.transform.localScale = new Vector3(areaAffectRadius * 2, 1, areaAffectRadius * 2);

            InputReader inputReader = user.GetComponent<InputReader>();

            yield return new WaitWhile(() => !inputReader.WasPressed(activationInput));
            
            Destroy(pointInstance.gameObject);
            data.SetTargets(GetTargetsInRadius(pointInstance.transform.position));
            data.SetTargetPoint(pointInstance.transform.position);

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