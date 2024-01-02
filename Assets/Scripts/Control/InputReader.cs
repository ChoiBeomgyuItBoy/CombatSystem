using RainbowAssets.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CombatSystem.Control
{
    public class InputReader : MonoBehaviour, IPredicateEvaluator
    {
        Controls controls;

        public InputAction GetInputAction(string actionName)
        {
            InputAction action = controls.FindAction(actionName);

            if(action == null)
            {
                Debug.LogError($"Input Action with name {actionName} not found");
                return null;
            }

            return action;
        }

        bool IsPressed(string actionName, bool thisFrame)
        {
            InputAction action = GetInputAction(actionName);

            if(thisFrame)
            {
                return action.WasPressedThisFrame();
            }
            else
            {
                return action.IsPressed();
            }
        }

        void Awake()
        {
            controls = new Controls();
            controls.Player.Enable();
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            if(predicate == "Input Action")
            {
                string actionName = parameters[0];
                return IsPressed(actionName, false);
            }

            return null;
        }
    }
}