using UnityEngine;

namespace RainbowAssets.BehaviourTree
{
    public class Wait : ActionNode
    {
        [SerializeField] float secondsToWait = 3;
        float localTime = 0;

        protected override void OnEnter()
        {
            localTime = 0;
        }

        protected override Status OnTick()
        {
            localTime += Time.deltaTime;

            if(localTime >= secondsToWait)
            {
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnExit() { }
    }
}