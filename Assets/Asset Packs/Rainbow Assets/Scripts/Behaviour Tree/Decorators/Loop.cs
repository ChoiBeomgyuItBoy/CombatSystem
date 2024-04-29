namespace RainbowAssets.BehaviourTree
{
    public class Loop : DecoratorNode
    {
        protected override void OnEnter() { }

        protected override Status OnTick()
        {
            GetChild().Tick();

            return Status.Running;
        }

        protected override void OnExit() { }
    }
}