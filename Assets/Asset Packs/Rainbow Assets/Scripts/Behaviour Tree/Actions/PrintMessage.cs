using UnityEngine;

namespace RainbowAssets.BehaviourTree
{
    public class PrintMessage : ActionNode
    {
        [SerializeField] string message;

        protected override void OnEnter()
        {
            Debug.Log(message);
        }

        protected override Status OnTick()
        {
            return Status.Success;
        }

        protected override void OnExit() { }
    }
}