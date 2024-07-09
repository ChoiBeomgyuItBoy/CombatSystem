using System;
using UnityEngine;

namespace CombatSystem.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Spawn Prefab Effect")]
    public class SpawnPrefabEffect : EffectStrategy
    {
        [SerializeField] GameObject effect;
        [SerializeField] bool spawnAtTargetPoint = true;

        public override void StartEffect(AbilityData data, Action finished)
        {
            if(spawnAtTargetPoint)
            {
                Instantiate(effect, data.GetTargetPoint(), Quaternion.identity);
            }
            else
            {
                Instantiate(effect, data.GetUser().transform);
            }

            finished();
        }
    }
}
