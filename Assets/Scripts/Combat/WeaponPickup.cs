using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] WeaponData weaponData;
        
        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                if(other.TryGetComponent(out Fighter fighter))
                {
                    fighter.EquipWeapon(weaponData);
                }
            }
        }
    }
}