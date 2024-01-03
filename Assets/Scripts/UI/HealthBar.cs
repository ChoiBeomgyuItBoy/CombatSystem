using CombatSystem.Attributes;
using UnityEngine;

namespace CombatSystem.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] RectTransform foreground;

        void Update()
        {
            foreground.localScale = new Vector3(health.GetFraction(), 1, 1);
        }
    }
}