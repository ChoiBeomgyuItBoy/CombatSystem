using UnityEngine;
using CombatSystem.Core;
using RainbowAssets.Utils;

namespace CombatSystem.Combat
{
    public class Fighter : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] WeaponData weaponData;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform leftHand;
        AnimationPlayer animationPlayer;
        Weapon weapon;
        int currentAttackIndex = 0;

        void Awake()
        {
            animationPlayer = GetComponent<AnimationPlayer>();
        }

        void Start()
        {
            weapon = weaponData.Spawn(rightHand, leftHand);
        }

        WeaponAttack GetCurrentAttack()
        {
            return weaponData.GetAttack(currentAttackIndex);
        }

        bool CurrentAttackIsLast()
        {
            return currentAttackIndex == weaponData.GetComboLength() - 1;
        }

        float GetCurrentAttackTime()
        {
            return animationPlayer.GetAnimationTime("Attack");
        }

        void Attack()
        {
            animationPlayer.PlaySmooth(GetCurrentAttack().GetAnimationName());
        }

        void CycleCombo()
        {
            currentAttackIndex++;
        }

        void ResetCombo()
        {
            currentAttackIndex = 0;
        }

        bool CanDoCombo()
        {
            WeaponAttack currentAttack = GetCurrentAttack();

            if(CurrentAttackIsLast())
            {
                return false;
            }

            float successTime = currentAttack.GetEndTime();

            if(GetCurrentAttackTime() < successTime)
            {
                return false;
            }

            return true;
        }

        // Animation Event
        void Hit()
        {
            weapon.Hit(gameObject, weaponData, GetCurrentAttack());
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            if(actionID == "Attack")
            {
                Attack();
            }

            if(actionID == "Cycle Combo")
            {
                CycleCombo();
            }

            if(actionID == "Reset Combo")
            {
                ResetCombo();
            }
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            if(predicate == "Can Do Combo")
            {
                return CanDoCombo();
            }

            return null;
        }
    }
}