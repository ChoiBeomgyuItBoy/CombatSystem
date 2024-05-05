using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RainbowAssets.BehaviourTree.Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        Node node;
        Port outputPort;
        Port inputPort;

        public NodeView(Node node) : base(BehaviourTreeEditor.path + "NodeView.uxml")
        {
            this.node = node;

            viewDataKey = node.GetUniqueID();
            
            title = node.name;

            style.left = node.GetPosition().x;
            style.top = node.GetPosition().y;

            CreatePorts();
            SetCapabilites();
            SetStyle();
            BindDescription();
        }

        public Node GetNode()
        {
            return node;
        }

        public Edge ConnectTo(NodeView child)
        {
            return outputPort.ConnectTo(child.inputPort);
        }

        public void SortChildren()
        {
            CompositeNode compositeNode = node as CompositeNode;

            if(compositeNode != null)
            {
                compositeNode.SortChildrenByPosition();
            }
        }

        public void DrawStatus()
        {
            RemoveFromClassList("runningStatus");
            RemoveFromClassList("successStatus");
            RemoveFromClassList("failureStatus");

            if (Application.isPlaying)
            {
                switch (node.GetStatus())
                {
                    case Status.Running:
                        AddToClassList("runningStatus");
                        break;

                    case Status.Success:
                        AddToClassList("successStatus");
                        break;

                    case Status.Failure:
                        AddToClassList("failureStatus");
                        break;
                }
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.SetPosition(new Vector2(newPos.x, newPos.y));
        }

        public override void OnSelected()
        {
            base.OnSelected();

            if (node is not RootNode)
            {
                Selection.activeObject = node;
            }
        }

        void SetCapabilites()
        {
            if (node is RootNode)
            {
                capabilities -= Capabilities.Deletable;
            }
        }

        void CreatePorts()
        {
            if (node is not RootNode)
            {
                inputPort = GetPort(Direction.Input, Port.Capacity.Single);
            }

            if (node is DecoratorNode)
            {
                outputPort = GetPort(Direction.Output, Port.Capacity.Single);
            }

            if (node is CompositeNode)
            {
                outputPort = GetPort(Direction.Output, Port.Capacity.Multi);
            }
        }

        Port GetPort(Direction direction, Port.Capacity capacity)
        {
            Port newPort = InstantiatePort(Orientation.Vertical, direction, capacity, typeof(bool));

            if (direction == Direction.Input)
            {
                inputContainer.Add(newPort);
            }

            if (direction == Direction.Output)
            {
                outputContainer.Add(newPort);
            }

            return newPort;
        }

        void SetStyle()
        {
            if (node is RootNode)
            {
                AddToClassList("rootNode");
            }

            else if (node is DecoratorNode)
            {
                AddToClassList("decoratorNode");
            }

            if (node is CompositeNode)
            {
                AddToClassList("compositeNode");
            }

            if (node is ActionNode)
            {
                AddToClassList("actionNode");
            }
        }

        void BindDescription()
        {
            Label descriptionLabel = this.Q<Label>("description");

            descriptionLabel.text = node.GetDescription();

            if (node is not RootNode)
            {
                descriptionLabel.bindingPath = "description";
                descriptionLabel.Bind(new SerializedObject(node));
            }
        }
    }
}