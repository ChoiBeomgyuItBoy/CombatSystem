using UnityEngine;

namespace CombatSystem.Combat
{
    [CreateAssetMenu(menuName = "New Weapon")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] Weapon equippedWeaponPrefab;
        [SerializeField] float baseDamage = 20;
        [SerializeField] float range = 5;
        [SerializeField] bool isLeftHanded = false;
        [SerializeField] WeaponAttack[] combo;

        public float GetBaseDamage()
        {
            return baseDamage;
        }

        public float GetRange()
        {
            return range;
        }

        public WeaponAttack GetAttack(int index)
        {
            return combo[index];
        }

        public int GetComboLength()
        {
            return combo.Length;
        }

        public Weapon Spawn(Transform rightHand, Transform leftHand)
        {
            Weapon weaponInstance = null;

            if(equippedWeaponPrefab != null)
            {
                if(isLeftHanded)
                {
                    weaponInstance = Instantiate(equippedWeaponPrefab, leftHand);
                }
                else
                {
                    weaponInstance = Instantiate(equippedWeaponPrefab, rightHand);
                }
            } 

            return weaponInstance;
        }
    }
}