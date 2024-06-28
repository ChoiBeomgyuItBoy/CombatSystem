using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Abilites
{
    public class AbilityData
    {
        GameObject user;
        IEnumerable<GameObject> targets;
        Vector3 targetPoint;

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

        public void SetTargets(IEnumerable<GameObject> targets)
        {
            this.targets = targets;
        }

        public void SetTargetPoint(Vector3 targetPoint)
        {
            this.targetPoint = targetPoint;
        }
    }
}