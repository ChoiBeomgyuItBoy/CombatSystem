using UnityEngine;

namespace CombatSystem.Attributes
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100;
        [SerializeField] float currentHealth = 0;

        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
        }

        void Awake()
        {
            currentHealth = maxHealth;
        }
    }
}