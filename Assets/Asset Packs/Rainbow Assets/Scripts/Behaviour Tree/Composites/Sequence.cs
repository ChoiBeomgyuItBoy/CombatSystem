using System.Linq;

namespace RainbowAssets.BehaviourTree.Composites
{
    public class Sequence : CompositeNode
    {
        int currentChildIndex = 0;

        protected override void OnEnter()
        {
            currentChildIndex = 0;
        }

        protected override Status OnTick()
        {
            Status currentStatus = GetChild(currentChildIndex).Tick();

            switch (currentStatus)
            {
                case Status.Running:
                    return Status.Running;

                case Status.Success:
                    currentChildIndex++;
                    break;

                case Status.Failure:
                    return Status.Failure;
            }

            if (currentChildIndex >= GetChildren().Count())
            {
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnExit() { }
    }
}