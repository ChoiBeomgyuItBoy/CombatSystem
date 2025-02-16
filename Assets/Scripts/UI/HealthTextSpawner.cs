using UnityEngine;

namespace RPG.UI
{
    public class HealthTextSpawner : MonoBehaviour
    {   
        [SerializeField] HealthText healthTextPrefab;

        public void Spawn(float amount)
        {
            var healthTextInstance = Instantiate(healthTextPrefab, transform);
            healthTextInstance.SetValue(amount);
        }
    }
}
