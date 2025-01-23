using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class HealthText : MonoBehaviour
    {
        [SerializeField] TMP_Text healthText;
        [SerializeField] bool isHealing = false;

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void SetValue(float amount)
        {
            if(isHealing)
            {
                healthText.text = $"+{amount}";
            }
            else
            {
                healthText.text = $"-{amount}";
            }
        }
    }   
}
