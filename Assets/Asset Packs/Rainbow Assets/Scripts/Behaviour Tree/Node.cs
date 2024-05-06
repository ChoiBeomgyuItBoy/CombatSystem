using UnityEditor;
using UnityEngine;

namespace RainbowAssets.BehaviourTree
{
    public abstract class Node : ScriptableObject
    {
        [SerializeField] string uniqueID;
        [SerializeField] string description;
        [SerializeField] Vector2 position;
        [SerializeField] int priority;
        Status status = Status.Running;
        bool started = false;
        protected BehaviourTreeController controller;

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        public void Bind(BehaviourTreeController controller)
        {
            this.controller = controller;
        }

        public string GetUniqueID()
        {
            return uniqueID;
        }

        public void SetUniqueID(string uniqueID)
        {
            this.uniqueID = uniqueID;
        }

        public string GetDescription()
        {
            return description;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public Status GetStatus()
        {
            return status;
        }

        public int GetPriority()
        {
            return priority;
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 position)
        {
            Undo.RecordObject(this, "Node Moved");
            this.position = position;
            EditorUtility.SetDirty(this);
        }
#endif

        public virtual void Abort()
        {
            started = false;
            status = Status.Failure;
        }

        public virtual Status Tick()
        {
            if (!started)
            {
                OnEnter();
                started = true;
            }

            status = OnTick();

            if (status == Status.Success || status == Status.Failure)
            {
                OnExit();
                started = false;
            }

            return status;
        }

        protected abstract void OnEnter();
        protected abstract Status OnTick();
        protected abstract void OnExit();
    }
}