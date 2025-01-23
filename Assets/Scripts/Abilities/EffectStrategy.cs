using System;
using UnityEngine;

namespace RPG.Abilites
{
    public abstract class EffectStrategy : ScriptableObject
    {
        public abstract void StartEffect(AbilityData data, Action finished);

        public virtual EffectStrategy Clone()
        {
            return Instantiate(this);
        }
    }
}