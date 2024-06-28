using CombatSystem.Control;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Abilites
{
    public class AbilityStore : MonoBehaviour, IPredicateEvaluator
    {
        [SerializeField] Ability[] abilities;
        string[] abilityInputs;
        Ability currentAbility;
        InputReader inputReader;

        void Awake()
        {
            inputReader = GetComponent<InputReader>();
        }

        void Start()
        {
            FillInputs();
        }

        void UseAbility(int index)
        {
            currentAbility = abilities[index];
            currentAbility.abilityFinished += CancelAbility;
            currentAbility.Use(gameObject);
        }

        void CancelAbility()
        {
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

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "Ability Selected":
                    for(int i = 0; i < abilities.Length; i++)
                    {
                        if(inputReader.WasPressed(abilityInputs[i]))
                        {
                            UseAbility(i);

                            return true;
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