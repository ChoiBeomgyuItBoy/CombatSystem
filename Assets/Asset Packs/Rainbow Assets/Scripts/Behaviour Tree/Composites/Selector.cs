using System.Linq;
using UnityEngine;

namespace RainbowAssets.BehaviourTree.Composites
{
    public class Selector : CompositeNode
    {
        [SerializeField] SelectionType selectionType;
        int currentChildIndex = 0;

        protected override void OnEnter()
        {
            currentChildIndex = 0;

            SetSelection();
        }

        protected override Status OnTick()
        {
            Status currentStatus = GetChild(currentChildIndex).Tick();

            switch (currentStatus)
            {
                case Status.Running:
                    return Status.Running;

                case Status.Success:
                    return Status.Success;

                case Status.Failure:
                    currentChildIndex++;
                    break;
            }

            if (currentChildIndex >= GetChildren().Count())
            {
                return Status.Failure;
            }

            return Status.Running;
        }

        protected override void OnExit() { }

        enum SelectionType
        {
            FirstToBeSuccessful,
            ByPriority,
            Random
        }

        void SetSelection()
        {
            switch (selectionType)
            {
                case SelectionType.FirstToBeSuccessful:
                    break;

                case SelectionType.ByPriority:
                    SortChildrenByPriority();
                    break;

                case SelectionType.Random:
                    ShuffleChildren();
                    break;
            }
        }
    }
}