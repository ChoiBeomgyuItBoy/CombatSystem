using CombatSystem.Control;
using RainbowAssets.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CombatSystem.Abilites
{
    public class AbilityStore : MonoBehaviour, IPredicateEvaluator
    {
        [SerializeField] Ability[] abilities;
        InputReader inputReader;

        void Awake()
        {
            inputReader = GetComponent<InputReader>();
        }

        void UseAbility(int index)
        {
            abilities[index].Use(gameObject);
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "Ability Selected":
                    
                    var abilityAction = inputReader.GetInputAction("Ability");

                    if(abilityAction != null && abilityAction.activeControl != null)
                    {
                        int index = abilityAction.GetBindingIndexForControl(abilityAction.activeControl);

                        UseAbility(index);

                        return true;
                    }

                    return false;
            }

            return null;
        }
    }
}