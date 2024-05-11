using CombatSystem.Core;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Attributes
{
    public class Health : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] float maxHealth = 100;
        [SerializeField] LazyEvent<float> onDamageTaken;
        public LazyEvent onDie;
        float currentHealth = 0;
        bool isInvulnerable = false;

        public void TakeDamage(float damage)
        {
            if(!IsDead() && !isInvulnerable)
            {
                currentHealth = Mathf.Max(0, currentHealth - damage);
                StartCoroutine(onDamageTaken?.Invoke(damage));     

                if(IsDead())
                {
                    StartCoroutine(onDie?.Invoke());
                }   
            }
        }

        public float GetFraction()
        {
            return currentHealth / maxHealth;
        }

        public bool IsDead()
        {
            return currentHealth == 0;
        }

        void Awake()
        {
            currentHealth = maxHealth;
        }

        void SetInvulnerable(bool isInvulnerable)
        {
            this.isInvulnerable = isInvulnerable;
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Set Invulnerability":
                    SetInvulnerable(true);
                    break;

                case "Cancel Invulnerability":
                    SetInvulnerable(false);
                    break;
            }
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "On Damage Taken":
                    return onDamageTaken.WasInvoked();

                case "On Die":
                    return onDie.WasInvoked();

                case "Is Dead":
                    return IsDead();
            }

            return null;
        }
    }
}