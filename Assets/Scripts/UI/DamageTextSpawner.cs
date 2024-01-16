using UnityEngine;

namespace CombatSystem.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {   
        [SerializeField] DamageText damageTextPrefab;
        public void Spawn(float damage)
        {
            var damageTextInstance = Instantiate(damageTextPrefab, transform);
            damageTextInstance.SetValue(damage);
        }
    }
}
