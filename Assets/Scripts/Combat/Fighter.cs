using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Combat
{
    public class Fighter : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] WeaponData weaponData;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform leftHand;
        Weapon weapon;
        Animator animator;
        int currentAttackIndex = 0;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            weapon = weaponData.Spawn(rightHand, leftHand);
        }

        void Hit()
        {
            weapon.Hit(gameObject, weaponData.GetBaseDamage());
        }

        WeaponAttack GetCurrentAttack()
        {
            return weaponData.GetAttack(currentAttackIndex);
        }

        bool CurrentAttackIsLast()
        {
            return currentAttackIndex == weaponData.GetComboLength() - 1;
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

        float GetCurrentAttackTime()
        {
            var currentInfo = animator.GetCurrentAnimatorStateInfo(0);

            if(currentInfo.IsTag("Attack") && !animator.IsInTransition(0))
            {
                return currentInfo.normalizedTime;
            }

            return 0;
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            if(actionID == "Attack")
            {
                animator.CrossFade(GetCurrentAttack().GetAnimationName(), 0.1f);
            }

            if(actionID == "Cycle Combo")
            {
                currentAttackIndex++;
            }

            if(actionID == "Reset Combo")
            {
                currentAttackIndex = 0;
            }
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            if(predicate == "Attack Finished")
            {
                return GetCurrentAttackTime() >= 1;
            }

            if(predicate == "Can Do Combo")
            {
                return CanDoCombo();
            }

            return null;
        }
    }
}