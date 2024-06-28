using System;
using UnityEngine;

namespace CombatSystem.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Spawn Prefab Effect")]
    public class SpawnPrefabEffect : EffectStrategy
    {
        [SerializeField] GameObject effect;

        public override void StartEffect(AbilityData data, Action finished)
        {
            Instantiate(effect, data.GetTargetPoint(), Quaternion.identity);

            finished();
        }
    }
}
