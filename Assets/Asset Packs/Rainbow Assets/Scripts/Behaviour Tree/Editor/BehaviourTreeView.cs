using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;

namespace RainbowAssets.BehaviourTree.Editor
{
    public class BehaviourTreeView : GraphView
    {
        new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
        BehaviourTree behaviourTree;

        public BehaviourTreeView()
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(BehaviourTreeEditor.path + "BehaviourTreeEditor.uss");
            styleSheets.Add(styleSheet);

            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        public void Refresh(BehaviourTree behaviourTree)
        {
            this.behaviourTree = behaviourTree;

            graphViewChanged -= OnGraphViewChanged;

            DeleteElements(graphElements);

            graphViewChanged += OnGraphViewChanged;

            if (behaviourTree != null)
            {
                foreach (var node in behaviourTree.GetNodes())
                {
                    CreateNodeView(node);
                }

                foreach (var node in behaviourTree.GetNodes())
                {
                    foreach (var child in behaviourTree.GetChildren(node))
                    {
                        if (child == null)
                        {
                            continue;
                        }

                        CreateEdge(node, child);
                    }
                }
            }
        }

        public void DrawStatus()
        {
            foreach (var node in nodes)
            {
                NodeView nodeView = node as NodeView;

                if (nodeView != null)
                {
                    nodeView.DrawStatus();
                }
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            foreach (var endPort in ports)
            {
                if (endPort.direction == startPort.direction)
                {
                    continue;
                }

                if (endPort.node == startPort.node)
                {
                    continue;
                }

                compatiblePorts.Add(endPort);
            }

            return compatiblePorts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (!Application.isPlaying)
            {
                base.BuildContextualMenu(evt);
                
                var nodeTypes = TypeCache.GetTypesDerivedFrom<Node>();

                foreach (var type in nodeTypes)
                {
                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    if (type == typeof(RootNode))
                    {
                        continue;
                    }

                    Vector2 mousePosition = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);

                    evt.menu.AppendAction($"Create Node/{type.Name} ({type.BaseType.Name})", a => CreateNode(type, mousePosition));
                }
            }
        }

        NodeView GetNodeView(string nodeID)
        {
            return GetNodeByGuid(nodeID) as NodeView;
        }

        void CreateNodeView(Node node)
        {
            NodeView nodeView = new(node);
            AddElement(nodeView);
        }

        void CreateNode(Type type, Vector2 position)
        {
            Node newNode = behaviourTree.CreateNode(type, position);
            CreateNodeView(newNode);
        }

        void RemoveNode(NodeView nodeView)
        {
            behaviourTree.RemoveNode(nodeView.GetNode());
        }

        void CreateEdge(Node parent, Node child)
        {
            NodeView parentView = GetNodeView(parent.GetUniqueID());
            NodeView childView = GetNodeView(child.GetUniqueID());
            AddElement(parentView.ConnectTo(childView));
        }

        void AddChild(Edge edge)
        {
            NodeView parentView = edge.output.node as NodeView;
            NodeView childView = edge.input.node as NodeView;

            Node parentNode = parentView.GetNode();
            Node childNode = childView.GetNode();

            DecoratorNode decoratorNode = parentNode as DecoratorNode;

            if (decoratorNode != null)
            {
                decoratorNode.SetChild(childNode);
            }

            CompositeNode compositeNode = parentNode as CompositeNode;

            if (compositeNode != null)
            {
                compositeNode.AddChild(childNode);
            }
        }

        void RemoveChild(Edge edge)
        {
            NodeView parentView = edge.output.node as NodeView;
            NodeView childView = edge.input.node as NodeView;

            Node parentNode = parentView.GetNode();
            Node childNode = childView.GetNode();

            DecoratorNode decoratorNode = parentNode as DecoratorNode;

            if (decoratorNode != null)
            {
                decoratorNode.UnsetChild();
            }

            CompositeNode compositeNode = parentNode as CompositeNode;

            if (compositeNode != null)
            {
                compositeNode.RemoveChild(childNode);
            }
        }

        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            var edgesToCreate = graphViewChange.edgesToCreate;

            if (edgesToCreate != null)
            {
                foreach (var edge in edgesToCreate)
                {
                    AddChild(edge);
                }
            }

            var elementsToRemove = graphViewChange.elementsToRemove;

            if (elementsToRemove != null)
            {
                foreach(var element in elementsToRemove)
                {
                    NodeView nodeView = element as NodeView;

                    if (nodeView != null)
                    {
                        RemoveNode(nodeView);
                    }

                    Edge edge = element as Edge;

                    if(edge != null)
                    {
                        RemoveChild(edge);
                    }
                }
            }

            var movedElements = graphViewChange.movedElements;

            if (movedElements != null)
            {
                foreach(var node in nodes)
                {
                    NodeView nodeView = node as NodeView;
                    nodeView.SortChildren();
                }
            }

            return graphViewChange;
        }

        void OnUndoRedo()
        {
            Refresh(behaviourTree);
        }
    }
}