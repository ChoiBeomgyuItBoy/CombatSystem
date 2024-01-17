using CombatSystem.Core;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Attributes
{
    public class Health : MonoBehaviour, IPredicateEvaluator
    {
        [SerializeField] float maxHealth = 100;
        [SerializeField] LazyEvent<float> onDamageTaken;
        float currentHealth = 0;

        public void TakeDamage(float damage)
        {
            if(!IsDead())
            {
                currentHealth = Mathf.Max(0, currentHealth - damage);
                StartCoroutine(onDamageTaken?.Invoke(damage));        
            }
        }

        public float GetFraction()
        {
            return currentHealth / maxHealth;
        }

        bool IsDead()
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
                case "Damage Taken":
                    return onDamageTaken.WasInvoked();
            }

            return null;
        }
    }
}