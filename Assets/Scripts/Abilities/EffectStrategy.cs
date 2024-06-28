using System;
using UnityEngine;

namespace CombatSystem.Abilites
{
    public abstract class EffectStrategy : ScriptableObject
    {
        public abstract void StartEffect(AbilityData data, Action finished);
    }
}