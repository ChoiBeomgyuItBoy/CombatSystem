using UnityEngine;

namespace CombatSystem.Combat
{
    [System.Serializable]
    public class WeaponAttack 
    {
        [SerializeField] string animationName;
        [SerializeField] [Range(0, 100)] float attackForce;
        [SerializeField] [Range(0, 0.99f)] float forceTime;
        [SerializeField] [Range(0, 100)] float knockback;
        [SerializeField] [Range(0, 0.99f)] float endTime;
        [SerializeField] [Range(0, 1)] float bonusPercentage;

        public string GetAnimationName()
        {
            return animationName;
        }

        public float GetAttackForce()
        {
            return attackForce;
        }

        public float GetForceTime()
        {
            return forceTime;
        }

        public float GetKnockback()
        {
            return knockback;
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