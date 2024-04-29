using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RainbowAssets.BehaviourTree
{
    public abstract class CompositeNode : Node
    {
        [SerializeField] List<Node> children = new();

        public override Node Clone()
        {
            CompositeNode clone = Instantiate(this);

            clone.children.Clear();

            foreach (var child in children)
            {
                clone.children.Add(child.Clone());
            }

            return clone;
        }

        public IEnumerable<Node> GetChildren()
        {
            return children;
        }

        public Node GetChild(int index)
        {
            return children[index];
        }

#if UNITY_EDITOR
        public void AddChild(Node child)
        {
            Undo.RecordObject(this, "Child added");
            children.Add(child);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(Node childToRemove)
        {
            Undo.RecordObject(this, "Child removed");
            children.Remove(childToRemove);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}