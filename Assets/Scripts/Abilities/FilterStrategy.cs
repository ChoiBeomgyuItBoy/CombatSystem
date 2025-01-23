using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilites
{
    public abstract class FilterStrategy : ScriptableObject
    {
        public abstract IEnumerable<GameObject> Filter(IEnumerable<GameObject> targetsToFilter);

        public virtual FilterStrategy Clone()
        {
            return Instantiate(this);
        }
    }
}
