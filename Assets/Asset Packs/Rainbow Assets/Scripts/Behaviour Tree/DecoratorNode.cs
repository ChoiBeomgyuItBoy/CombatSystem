using RainbowAssets.Utils;
using UnityEditor;
using UnityEngine;

namespace RainbowAssets.BehaviourTree
{
    public abstract class DecoratorNode : Node
    {
        [SerializeField] Node child;
        [SerializeField] Condition abortCondition;

        public override Node Clone()
        {
            DecoratorNode clone = Instantiate(this);

            clone.child = child.Clone();
            
            return clone;
        }

        public Node GetChild()
        {
            return child;
        }

        public override Status Tick()
        {
            if(!abortCondition.IsEmpty() && abortCondition.Check(controller.GetComponents<IPredicateEvaluator>()))
            {
                child.Abort();

                Abort();

                return Status.Failure;
            }

            return base.Tick();
        }

#if UNITY_EDITOR
        public void SetChild(Node child)
        {
            Undo.RecordObject(this, "Child Set");
            this.child = child;
            EditorUtility.SetDirty(this);
        }

        public void UnsetChild()
        {
            Undo.RecordObject(this, "Child removed");
            child = null;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}   