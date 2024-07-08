using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Abilites
{
    public class AbilityData
    {
        GameObject user;
        IEnumerable<GameObject> targets;
        Vector3 targetPoint;
        bool cancelled = false;

        public AbilityData(GameObject user)
        {
            this.user = user;
        }

        public GameObject GetUser()
        {
            return user;
        }

        public IEnumerable<GameObject> GetTargets()
        {
            return targets;
        }

        public Vector3 GetTargetPoint()
        {
            return targetPoint;
        }

        public bool IsCancelled()
        {
            return cancelled;
        }

        public void SetTargets(IEnumerable<GameObject> targets)
        {
            this.targets = targets;
        }

        public void SetTargetPoint(Vector3 targetPoint)
        {
            this.targetPoint = targetPoint;
        }

        public void Cancel()
        {
            cancelled = true;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            MonoBehaviour monoBehaviour = user.GetComponent<MonoBehaviour>();
            monoBehaviour.StartCoroutine(coroutine);
        }
    }
}