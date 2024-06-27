using CombatSystem.Control;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Abilites
{
    public class AbilityStore : MonoBehaviour, IPredicateEvaluator
    {
        [SerializeField] Ability[] abilities;
        string[] abilityInputs;
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
            abilities[index].Use(gameObject);
        }

        void FillInputs()
        {
            abilityInputs = new string[abilities.Length];

            for(int i = 0; i < abilities.Length; i++)
            {
                abilityInputs[i] = $"Ability {i + 1}";
            }
        }

        bool InputPressed(int index)
        {
            return inputReader.GetInputAction(abilityInputs[index]).WasPressedThisFrame();
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "Ability Selected":
                    
                    for(int i = 0; i < abilities.Length; i++)
                    {
                        if(InputPressed(i))
                        {
                            UseAbility(i);

                            return true;
                        }
                    }

                    return false;
            }

            return null;
        }
    }
}