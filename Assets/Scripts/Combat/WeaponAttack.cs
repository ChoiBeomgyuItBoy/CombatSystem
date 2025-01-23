using UnityEngine;

namespace RPG.Combat
{
    [System.Serializable]
    public class WeaponAttack 
    {
        [SerializeField] string animationName;
        [SerializeField] Vector2 knockback;
        [SerializeField] float attackForce;
        [SerializeField] [Range(0, 0.99f)] float endTime;
        [SerializeField] [Range(0, 1)] float bonusPercentage;

        public string GetAnimationName()
        {
            return animationName;
        }

        public Vector2 GetKnockback()
        {
            return knockback;
        }

        public float GetAttackForce()
        {
            return attackForce;
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