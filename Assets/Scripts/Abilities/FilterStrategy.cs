using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Abilites
{
    public abstract class FilterStrategy : ScriptableObject
    {
        public abstract IEnumerable<GameObject> Filter(IEnumerable<GameObject> targetsToFilter);
    }
}
