using UnityEngine;

namespace CombatSystem.Combat
{
    [CreateAssetMenu(menuName = "New Weapon")]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] GameObject equippedWeaponPrefab;
        [SerializeField] bool isLeftHanded = false;
        [SerializeField] WeaponAttack[] combo;

        public WeaponAttack GetAttack(int index)
        {
            return combo[index];
        }

        public int GetComboLength()
        {
            return combo.Length;
        }

        public void Spawn(Transform rightHand, Transform leftHand)
        {
            if(equippedWeaponPrefab != null)
            {
                if(isLeftHanded)
                {
                    Instantiate(equippedWeaponPrefab, leftHand);
                }
                else
                {
                    Instantiate(equippedWeaponPrefab, rightHand);
                }
            } 
        }
    }
}