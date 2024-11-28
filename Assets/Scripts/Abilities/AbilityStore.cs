using CombatSystem.Control;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Abilites
{
    public class AbilityStore : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] bool randomSelection = false;
        [SerializeField] Ability[] abilities;        
        InputReader inputReader;
        Ability currentAbility;
        string[] abilityInputs;
        int currentAbilityIndex = 0;

        [System.Serializable]
        class AbilityCondition
        {
            public Ability ability;
            public Condition useCondition;
        }

        void Awake()
        {
            inputReader = GetComponent<InputReader>();

            for(int i = 0; i < abilities.Length; i++)
            {
                abilities[i]= abilities[i].Clone();
            }

            if(inputReader != null)
            {
                FillInputs();
            }
        }

        bool UseAbility(int index)
        {
            if(abilities.Length == 0)
            {
                return false;
            }

            Ability candidate = abilities[index];

            if(candidate.CanBeUsed())
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
            if(currentAbility != null)
            {
                currentAbility.Cancel();
                currentAbility.abilityFinished -= CancelAbility;
                currentAbility = null;
            }
        }

        void FillInputs()
        {
            abilityInputs = new string[abilities.Length];

            for(int i = 0; i < abilities.Length; i++)
            {
                abilityInputs[i] = $"Ability {i + 1}";
            }
        }

        bool AbilitySelected()
        {
            for(int i = 0; i < abilities.Length; i++)
            {
                if(inputReader.WasPressed(abilityInputs[i]))
                {
                    return UseAbility(i);
                }
            }

            return false;
        }

        void SelectAbility()
        {
            if(randomSelection)
            {
                UseAbility(Random.Range(0, abilities.Length));
            }
            else
            {
                UseAbility(currentAbilityIndex);
            }
        }

        void CycleAbility()
        {
            if(currentAbilityIndex == abilities.Length - 1)
            {
                currentAbilityIndex = 0;
                return;
            }

            currentAbilityIndex++;
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Select Ability":
                    SelectAbility();
                    return;

                case "Cancel Ability":
                    CancelAbility();
                    break;

                case "Cycle Ability":
                    CycleAbility();
                    break;
            }
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "Ability Selected":
                    return AbilitySelected();

                case "Ability Finished":
                    return currentAbility == null;
            }

            return null;
        }
    }
}