
using UnityEngine;

namespace CombatSystem.Combat
{
    [System.Serializable]
    public class WeaponAttack 
    {
        [SerializeField] string animationName;
        [SerializeField] [Range(0, 0.99f)] float endTime;

        public string GetAnimationName()
        {
            return animationName;
        }

        public float GetEndTime()
        {
            return endTime;
        }
    }
}