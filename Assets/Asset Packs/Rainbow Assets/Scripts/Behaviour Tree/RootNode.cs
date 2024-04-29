using UnityEngine;

namespace RainbowAssets.BehaviourTree
{
    public class RootNode : DecoratorNode
    {
        protected override void OnEnter() { }

        protected override Status OnTick()
        {
            return GetChild().Tick();
        }

        protected override void OnExit() { }
    }
}