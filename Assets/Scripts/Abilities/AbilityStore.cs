using System.Collections.Generic;
using CombatSystem.Control;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Abilites
{
    public class AbilityStore : MonoBehaviour, IPredicateEvaluator
    {
        [SerializeField] Ability[] abilities;
        Dictionary<Ability, float> cooldownTimers = new();
        Ability currentAbility;
        InputReader inputReader;
        string[] abilityInputs;

        void Awake()
        {
            inputReader = GetComponent<InputReader>();
        }

        void Start()
        {
            FillInputs();
        }

        void Update()
        {
            var keys = new List<Ability>(cooldownTimers.Keys);

            foreach(var ability in keys)
            {
                cooldownTimers[ability] -= Time.deltaTime;

                if(cooldownTimers[ability] < 0)
                {
                    cooldownTimers.Remove(ability);
                }
            }
        }

        bool UseAbility(int index)
        {
            Ability candidate = abilities[index];

            if(GetRemainingCooldown(candidate) <= 0)
            {
                currentAbility = candidate;
                currentAbility.abilityFinished += CancelAbility;
                currentAbility.Use(gameObject);
                return true;
            }

            return false;
        }

        void CancelAbility()
        {
            StartCooldown(currentAbility);
            currentAbility.abilityFinished -= CancelAbility;
            currentAbility = null;
        }

        void FillInputs()
        {
            abilityInputs = new string[abilities.Length];

            for(int i = 0; i < abilities.Length; i++)
            {
                abilityInputs[i] = $"Ability {i + 1}";
            }
        }

        void StartCooldown(Ability ability)
        {
            cooldownTimers[ability] = ability.GetCooldownTime();
        }

        float GetRemainingCooldown(Ability ability)
        {
            if(!cooldownTimers.ContainsKey(ability))
            {
                return 0;
            }
            
            return cooldownTimers[ability];
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "Ability Selected":
                    for(int i = 0; i < abilities.Length; i++)
                    {
                        if(inputReader.WasPressed(abilityInputs[i]))
                        {
                            return UseAbility(i);
                        }
                    }

                    return false;

                case "Ability Finished":
                    return currentAbility == null;
            }

            return null;
        }
    }
}