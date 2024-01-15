using UnityEngine;

namespace CombatSystem.Combat
{
    [System.Serializable]
    public class WeaponAttack 
    {
        [SerializeField] string animationName;
        [SerializeField] [Range(0, 0.99f)] float endTime;
        [SerializeField] [Range(0, 1)] float bonusPercentage;

        public string GetAnimationName()
        {
            return animationName;
        }

        public float GetEndTime()
        {
            return endTime;
        }

        public float GetBonusPercentage()
        {
            return bonusPercentage;
        }
    }
}