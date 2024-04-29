using UnityEngine;

namespace RainbowAssets.BehaviourTree
{
    public class BehaviourTreeController : MonoBehaviour
    {
        [SerializeField] BehaviourTree behaviourTree;

        public BehaviourTree GetBehaviourTree()
        {
            return behaviourTree;
        }

        void Awake()
        {
            behaviourTree = behaviourTree.Clone();
        }

        void Start()
        {
            behaviourTree.Bind(this);
        }

        void Update()
        {
            behaviourTree.Tick();
        }
    }
}