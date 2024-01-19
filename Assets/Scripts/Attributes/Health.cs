using CombatSystem.Core;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Attributes
{
    public class Health : MonoBehaviour, IPredicateEvaluator
    {
        [SerializeField] float maxHealth = 100;
        [SerializeField] LazyEvent<float> onDamageTaken;
        public LazyEvent onDie;
        float currentHealth = 0;

        public void TakeDamage(float damage)
        {
            if(!IsDead())
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

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "On Damage Taken":
                    return onDamageTaken.WasInvoked();

                case "On Die":
                    return onDie.WasInvoked();
            }

            return null;
        }
    }
}