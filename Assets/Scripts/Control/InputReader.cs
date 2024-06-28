using System.Threading.Tasks;
using RainbowAssets.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CombatSystem.Control
{
    public class InputReader : MonoBehaviour, IPredicateEvaluator
    {
        Controls controls;

        public bool WasPressed(string actionName)
        {
            return GetInputAction(actionName).WasPressedThisFrame();
        }

        public bool IsPressed(string actionName)
        {
            return GetInputAction(actionName).IsPressed();
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
                case "Was Pressed":
                    return WasPressed(parameters[0]);

                case "Is Pressed":
                    return IsPressed(parameters[0]);
            }

            return null;
        }
    }
}