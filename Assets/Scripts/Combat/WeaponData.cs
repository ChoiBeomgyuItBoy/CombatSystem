using CombatSystem.Attributes;
using CombatSystem.Core;
using UnityEngine;

namespace CombatSystem.Combat
{
    [CreateAssetMenu(menuName = "New Weapon")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] Weapon equippedWeapon;
        [SerializeField] Projectile projectile;
        [SerializeField] AnimatorOverrideController animatorOverride;
        [SerializeField] float baseDamage = 20;
        [SerializeField] float range = 5;
        [SerializeField] bool isLeftHanded = false;
        [SerializeField] WeaponAttack[] combo;
        GameObject user;
        Transform rightHand;
        Transform leftHand;
        Weapon weaponInstance;
        const string spawnID = "Weapon";
        const string destroyID = "Destroying";

        public WeaponData Clone()
        {
            return Instantiate(this);
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

        public void Spawn(GameObject user, Transform rightHand, Transform leftHand, AnimationPlayer animationPlayer)
        {
            this.user = user;
            this.rightHand = rightHand;
            this.leftHand = leftHand;

            DestroyOldWeapon();

            if(equippedWeapon != null)
            {
                weaponInstance = Instantiate(equippedWeapon, GetHand());
                weaponInstance.gameObject.name = spawnID;
            }

            animationPlayer.SetAnimatorOverride(animatorOverride);
        }
 
        public void Hit(int comboIndex, Health target)
        {
            float damage = GetTotalDamage(comboIndex);
            Vector3 knockback = GetAttack(comboIndex).GetKnockback();

            if(projectile != null)
            {
                LaunchProjectile(target, damage, knockback);
            }
            else
            {
                weaponInstance.Hit(user, damage, knockback);
            }
        }

        Projectile LaunchProjectile(Health target, float damage, Vector3 knockback)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHand().position, Quaternion.identity);

            if(target != null)
            {
                projectileInstance.SetData(user, GetHand().position, damage, knockback, target);
            }
            else
            {
                projectileInstance.SetData(user, GetHand().position, damage, knockback, GetProjectilePoint());
            }

            return projectileInstance;
        }

        Vector3 GetProjectilePoint()
        {
            return user.transform.position + user.transform.forward * range;
        }

        float GetTotalDamage(int comboIndex)
        {
            float bonusPercentage = GetAttack(comboIndex).GetBonusPercentage();
            float bonusDamage = baseDamage * bonusPercentage;
            return Mathf.Round(baseDamage + bonusDamage);
        }

        Transform GetHand()
        {
            return isLeftHanded ? leftHand : rightHand;
        }

        void DestroyOldWeapon()
        {
            Transform oldWeapon = rightHand.Find(spawnID);

            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(spawnID);
            }

            if(oldWeapon != null)
            {
                oldWeapon.name = destroyID;
                Destroy(oldWeapon.gameObject);
            }
        }
    }
}