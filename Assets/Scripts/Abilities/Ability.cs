using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Abilites
{
    [CreateAssetMenu(menuName = "Abilities/New Ability")]
    public class Ability : ScriptableObject
    {
        [SerializeField] TargetingStrategy targetingStrategy;

        public void Use(GameObject user)
        {
            targetingStrategy.StartTargeting(user, TargetAquired);
        }

        void TargetAquired(IEnumerable<GameObject> targets)
        {
            foreach(var target in targets)
            {
                Debug.Log(target);
            }
        }   
    }
}