using RainbowAssets.Utils;
using UnityEngine;

namespace RainbowAssets.BehaviourTree
{
    public class CustomAction : ActionNode
    {
        [SerializeField] Status defaultStatus = Status.Running;
        [SerializeField] ActionData[] onEnterActions;
        [SerializeField] ActionData[] onTickActions;
        [SerializeField] ActionData[] onExitActions;
        [SerializeField] StatusCondition[] statusConditions;

        protected override void OnEnter()
        {
            DoActions(onEnterActions);
        }

        protected override Status OnTick()
        {
            DoActions(onTickActions);

            return CheckStatus();
        }

        protected override void OnExit()
        {
            DoActions(onExitActions);
        }

        [System.Serializable]
        class ActionData
        {
            public string actionID;
            public string[] parameters;
        }

        [System.Serializable]
        class StatusCondition
        {
            public Status status;
            public Condition condition;
        }

        Status CheckStatus()
        {
            foreach (var statusCondition in statusConditions)
            {
                bool success = statusCondition.condition.Check(controller.GetComponents<IPredicateEvaluator>());

                if (success)
                {
                    return statusCondition.status;
                }
            }

            return defaultStatus;
        }

        void DoActions(ActionData[] actions)
        {
            foreach (var action in controller.GetComponents<IAction>())
            {
                foreach (var actionData in actions)
                {
                    action.DoAction(actionData.actionID, actionData.parameters);
                }
            }
        }
    }
}