using System;
using UnityEngine;

namespace CombatSystem.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Spawn Prefab Effect")]
    public class SpawnPrefabEffect : EffectStrategy
    {
        [SerializeField] GameObject effect;
        [SerializeField] SpawnChoice spawnChoice;

        enum SpawnChoice
        {
            TargetPoint,
            UserTransform,
            UserPosition,
            PerTargetTransform,
            PerTargetPosition
        }

        public override void StartEffect(AbilityData data, Action finished)
        {
            switch (spawnChoice)
            {
                case SpawnChoice.TargetPoint:
                    Instantiate(effect, data.GetTargetPoint(), Quaternion.identity);
                    break;

                case SpawnChoice.UserTransform:
                    Instantiate(effect, data.GetUser().transform);
                    break;

                case SpawnChoice.UserPosition:
                    Instantiate(effect, data.GetUser().transform.position, Quaternion.identity);
                    break;

                case SpawnChoice.PerTargetTransform:
                    foreach(var target in data.GetTargets())
                    {
                        Instantiate(effect, target.transform);
                    }
                    break;

                case SpawnChoice.PerTargetPosition:
                    foreach(var target in data.GetTargets())
                    {
                        Instantiate(effect, target.transform.position, Quaternion.identity);
                    }
                    break;
            }

            finished();
        }
    }
}
