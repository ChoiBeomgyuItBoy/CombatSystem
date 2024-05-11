using CombatSystem.Control;
using RainbowAssets.Utils;
using UnityEngine;

namespace CombatSystem.Movement
{
    public class FreeLooker : MonoBehaviour, IAction
    {
        [SerializeField] [Range(0,1)] float freeLookSpeedFraction = 0.6f;
        Mover mover;
        InputReader inputReader;        

        void Awake()
        {
            mover = GetComponent<Mover>();
            inputReader = GetComponent<InputReader>();
        }

        Vector3 GetFreeLookDirection()
        {
            Vector2 inputValue = inputReader.GetInputValue();
            
            Transform mainCamera = Camera.main.transform;

            Vector3 right = (inputValue.x * mainCamera.right).normalized;
            right.y = 0;

            Vector3 forward = (inputValue.y * mainCamera.forward).normalized;
            forward.y = 0;

            return right + forward;
        }
        
        void IAction.DoAction(string actionID, string[] parameters)
        {
            switch(actionID)
            {
                case "Free Look Movement":
                    mover.MoveTo(GetFreeLookDirection(), freeLookSpeedFraction);  
                    break;
            }

        }
    }
}