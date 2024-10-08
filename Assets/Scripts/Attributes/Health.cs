using CombatSystem.Core;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Attributes
{
    public class Health : MonoBehaviour, IPredicateEvaluator
    {
        [SerializeField] float maxHealth = 100;
        [SerializeField] LazyEvent<float> onDamageTaken;
        [SerializeField] LazyEvent<float> onHeal;
        public LazyEvent onDie;
        float currentHealth = 0;
        [SerializeField] int invulnerabilityCounter = 0;

        public bool IsInvulnerable()
        {
            return invulnerabilityCounter > 0;
        }

        public void SetInvulnerability()
        {
            invulnerabilityCounter++;
        }

        public void CancelInvulnerability()
        {
            invulnerabilityCounter = Mathf.Max(0, invulnerabilityCounter - 1);
        }

        public bool IsDead()
        {
            return currentHealth == 0;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public float GetCurrentHealth()
        {
            return currentHealth;
        }

        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if(!IsDead() && !IsInvulnerable())
            {
                currentHealth = Mathf.Max(0, currentHealth - damage);
                StartCoroutine(onDamageTaken?.Invoke(damage));     

                if(IsDead())
                {
                    StartCoroutine(onDie?.Invoke());
                }   
            }
        }

        public void Heal(float amount)
        {
            if(!IsDead() && currentHealth < GetMaxHealth())
            {
                currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
                StartCoroutine(onHeal?.Invoke(amount));
            }
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

                case "Is Dead":
                    return IsDead();

                case "Has Health Percentage":
                    float percentage = float.Parse(parameters[1]);

                    if(parameters[0] == ">")
                    {
                        return GetHealthPercentage() > percentage;
                    }

                    if(parameters[0] == "<")
                    {
                        return GetHealthPercentage() < percentage;
                    }

                    return false;
            }

            return null;
        }
    }
}