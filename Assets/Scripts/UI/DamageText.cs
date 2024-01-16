using TMPro;
using UnityEngine;

namespace CombatSystem.UI
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] TMP_Text damageText;

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void SetValue(float damage)
        {
            damageText.text = $"{damage}";
        }
    }   
}
