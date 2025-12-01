using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class DialogueGraphView : GraphView
    {
        private readonly DialogueGraphWindow window;
        private DialogueTree tree;
        private readonly Vector2 defaultNodeSize = new Vector2(200, 150);

        public DialogueTree Tree => tree;

        public DialogueGraphView(DialogueGraphWindow window)
        {
            this.window = window;
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraphStyles"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            graphViewChanged += OnGraphViewChanged;
            nodeCreationRequest += context =>
            {
                var position = contentViewContainer.WorldToLocal(context.screenMousePosition);
                CreateNode("Dialogue", position);
            };
        }

        public void SetTree(DialogueTree newTree)
        {
            tree = newTree;
            var elements = new List<GraphElement>();
            graphElements.ForEach(element =>
            {
                if (element is DialogueNodeView || element is Edge)
                {
                    elements.Add(element);
                }
            });
            DeleteElements(elements);

            if (tree == null)
            {
                return;
            }

            foreach (var nodeData in tree.Nodes)
            {
                CreateNodeView(nodeData);
            }

            foreach (var nodeView in nodes.ToList())
            {
                var data = (nodeView as DialogueNodeView)?.Data;
                if (data == null)
                {
                    continue;
                }

                foreach (var choice in data.Choices)
                {
                    if (string.IsNullOrEmpty(choice.TargetNodeGuid))
                    {
                        continue;
                    }

                    var targetView = FindNodeView(choice.TargetNodeGuid);
                    if (targetView != null)
                    {
                        var outputPort = (nodeView as DialogueNodeView)?.GetPortForChoice(choice);
                        var inputPort = targetView.InputPort;
                        var edge = outputPort.ConnectTo(inputPort);
                        AddElement(edge);
                    }
                }
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (tree == null)
            {
                return change;
            }

            if (change.elementsToRemove != null)
            {
                Undo.RecordObject(tree, "Remove Dialogue Elements");

                foreach (var element in change.elementsToRemove)
                {
                    if (element is DialogueNodeView nodeView)
                    {
                        tree.RemoveNode(nodeView.Data);
                        window.InspectNode(null);
                    }

                    if (element is Edge edge)
                    {
                        var outputNode = edge.output.node as DialogueNodeView;
                        var choice = outputNode?.GetChoiceForPort(edge.output);
                        if (choice != null)
                        {
                            choice.TargetNodeGuid = string.Empty;
                        }
                    }
                }

                EditorUtility.SetDirty(tree);
            }

            if (change.edgesToCreate != null)
            {
                foreach (var edge in change.edgesToCreate)
                {
                    var outputNode = edge.output.node as DialogueNodeView;
                    var inputNode = edge.input.node as DialogueNodeView;

                    if (outputNode == null || inputNode == null || outputNode == inputNode)
                    {
                        RemoveElement(edge);
                        continue;
                    }

                    var choice = outputNode.GetChoiceForPort(edge.output);
                    if (choice == null)
                    {
                        RemoveElement(edge);
                        continue;
                    }

                    if (choice.TargetNodeGuid == inputNode.Data.Guid)
                    {
                        RemoveElement(edge);
                        continue;
                    }

                    Undo.RecordObject(tree, "Link Dialogue Nodes");
                    choice.TargetNodeGuid = inputNode.Data.Guid;
                    EditorUtility.SetDirty(tree);
                }
            }

            if (change.movedElements != null)
            {
                foreach (var element in change.movedElements)
                {
                    if (element is DialogueNodeView nodeView)
                    {
                        Undo.RecordObject(tree, "Move Dialogue Node");
                        nodeView.Data.Position = nodeView.GetPosition().position;
                        EditorUtility.SetDirty(tree);
                    }
                }
            }

            return change;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort == port || startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                if (port.capacity == Port.Capacity.Single && port.connected)
                {
                    return;
                }

                compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        public void CreateNode(string nodeName, Vector2 position)
        {
            if (tree == null)
            {
                return;
            }

            Undo.RecordObject(tree, "Add Dialogue Node");

            var nodeData = new DialogueNodeData
            {
                Guid = System.Guid.NewGuid().ToString(),
                Text = nodeName,
                Position = position
            };

            nodeData.Choices.Add(new DialogueChoiceData { Text = "Next" });

            tree.AddNode(nodeData);
            if (string.IsNullOrEmpty(tree.StartNodeGuid))
            {
                tree.StartNodeGuid = nodeData.Guid;
            }
            EditorUtility.SetDirty(tree);

            CreateNodeView(nodeData);
        }

        private void CreateNodeView(DialogueNodeData nodeData)
        {
            var nodeView = new DialogueNodeView(nodeData, this)
            {
                title = string.IsNullOrEmpty(nodeData.Text) ? "Dialogue" : nodeData.Text
            };
            nodeView.SetPosition(new Rect(nodeData.Position, defaultNodeSize));
            AddElement(nodeView);
        }

        public DialogueNodeView FindNodeView(string guid)
        {
            return GetNodeByGuid(guid) as DialogueNodeView;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            evt.menu.AppendAction("Add Dialogue Node", action =>
            {
                var position = contentViewContainer.WorldToLocal(action.eventInfo.mousePosition);
                CreateNode("Dialogue", position);
            });
        }

        private Node GetNodeByGuid(string guid)
        {
            foreach (var node in nodes)
            {
                if (node is DialogueNodeView dialogueNode && dialogueNode.Data.Guid == guid)
                {
                    return node;
                }
            }

            return null;
        }

        public void NotifySelection(DialogueNodeView nodeView)
        {
            window.InspectNode(nodeView);
        }
    }
}
