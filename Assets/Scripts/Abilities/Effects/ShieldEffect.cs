using System;
using System.Collections;
using CombatSystem.Attributes;
using UnityEngine;

namespace CombatSystem.Abilites.Effects
{
    [CreateAssetMenu(menuName = "Abilities/Effects/Shield Effect")]
    public class ShieldEffect : EffectStrategy
    {
        [SerializeField] Health shieldPrefab;
        [SerializeField] GameObject destroyEffect;
        [SerializeField] float maxLifeTime = 20;

        public override void StartEffect(AbilityData data, Action finished)
        {
            Health user = data.GetUser().GetComponent<Health>();
            Health shieldInstance = Instantiate(shieldPrefab, user.transform);

            shieldInstance.tag = user.tag;
            
            user.SetInvulnerable(true);
            data.StartCoroutine(DestroyRoutine(user, shieldInstance));

            finished();
        }

        IEnumerator DestroyRoutine(Health user, Health shieldInstance)
        {
            float elapsedTime = 0;

            while(elapsedTime < maxLifeTime)
            {       
                if(shieldInstance.IsDead())
                {
                    if(destroyEffect != null)
                    {
                        Instantiate(destroyEffect, user.transform.position, Quaternion.identity);
                    }

                    break;
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }   

            user.SetInvulnerable(false);
            Destroy(shieldInstance.gameObject);
        }
    }
}