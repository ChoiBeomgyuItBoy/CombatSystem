using System.Collections.Generic;
using CombatSystem.Control;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Abilites
{
    public class AbilityStore : MonoBehaviour, IAction, IPredicateEvaluator
    {
        [SerializeField] List<Ability> abilities = new();
        InputReader inputReader;
        Ability currentAbility;
        List<string> abilityInputs = new();

        void Awake()
        {
            inputReader = GetComponent<InputReader>();

            List<Ability> abilitiesCopy = new(abilities);

            abilities.Clear();

            foreach(var ability in abilitiesCopy)
            {
                abilities.Add(ability.Clone());
            }

            if(inputReader != null)
            {
                FillInputs();
            }
        }

        bool UseAbility(int index)
        {
            Ability candidate = abilities[index];

            if(candidate.Use(gameObject))
            {
                currentAbility = candidate;
                currentAbility.abilityFinished += CancelAbility;
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
            for(int i = 0; i < abilities.Count; i++)
            {
                abilityInputs.Add($"Ability {i + 1}");
            }
        }

        bool AbilitySelected()
        {
            for(int i = 0; i < abilities.Count; i++)
            {
                if(inputReader.WasPressed(abilityInputs[i]))
                {
                    return UseAbility(i);
                }
            }

            return false;
        }

        void SelectRandomAbility()
        {
            UseAbility(Random.Range(0, abilities.Count));
        }

        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Select Random Ability":
                    SelectRandomAbility();
                    return;

                case "Cancel Ability":
                    CancelAbility();
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