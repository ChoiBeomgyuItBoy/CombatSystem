using RainbowAssets.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CombatSystem.Control
{
    public class InputReader : MonoBehaviour, IPredicateEvaluator
    {
        Controls controls;

        public bool IsPressed(string actionName, bool thisFrame)
        {
            InputAction action = GetInputAction(actionName);

            return thisFrame? action.WasPerformedThisFrame() : action.IsPressed();
        }

        public Vector2 GetInputValue()
        {
            return GetInputAction("Locomotion").ReadValue<Vector2>();
        }

        void Awake()
        {
            controls = new Controls();
            controls.Player.Enable();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        InputAction GetInputAction(string actionName)
        {
            InputAction action = controls.FindAction(actionName);

            if(action == null)
            {
                Debug.LogError($"Input Action with name {actionName} not found");
                return null;
            }

            return action;
        }

        bool? IPredicateEvaluator.Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "Input Action":
                    return IsPressed(parameters[0], false);
                
                case "Input Action This Frame":
                    return IsPressed(parameters[0], true);
            }

            return null;
        }
    }
}