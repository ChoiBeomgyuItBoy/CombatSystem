using System;
using UnityEngine;

namespace CombatSystem.Abilites
{
    public abstract class TargetingStrategy : ScriptableObject
    {
        public abstract void StartTargeting(AbilityData data, Action finished);

        public virtual TargetingStrategy Clone()
        {
            return Instantiate(this);
        }
    }
}
