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
        Weapon weapon;
        Motioner motioner;
        int currentAttackIndex = 0;

        void Awake()
        {
            motioner = GetComponent<Motioner>();
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
            return motioner.GetNormalizedTime("Attack");
        }

        void Attack()
        {
            motioner.Play(GetCurrentAttack().GetAnimationName());
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
            weapon.Hit(gameObject, weaponData.GetBaseDamage());
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