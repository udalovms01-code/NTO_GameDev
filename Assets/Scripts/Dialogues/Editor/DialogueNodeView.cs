using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class DialogueNodeView : Node
    {
        public DialogueNodeData Data { get; }
        public Port InputPort { get; private set; }
        private readonly List<Port> outputPorts = new List<Port>();
        private readonly DialogueGraphView graphView;

        public DialogueNodeView(DialogueNodeData data, DialogueGraphView graphView)
        {
            Data = data;
            this.graphView = graphView;
            viewDataKey = data.Guid;
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraphStyles"));

            CreateInput();
            CreateOutputs();
            CreateBody();
        }

        private void CreateInput()
        {
            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float));
            InputPort.portName = "In";
            inputContainer.Add(InputPort);
        }

        private void CreateOutputs()
        {
            outputPorts.Clear();
            outputContainer.Clear();

            foreach (var choice in Data.Choices)
            {
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
                port.portName = string.IsNullOrEmpty(choice.Text) ? "Choice" : choice.Text;
                outputPorts.Add(port);

                var textField = new TextField { value = choice.Text };
                textField.RegisterValueChangedCallback(evt => OnChoiceTextChanged(choice, port, evt.newValue));
                port.Add(textField);

                var deleteButton = new Button(() => RemoveChoicePort(choice, port)) { text = "X" };
                port.Add(deleteButton);

                outputContainer.Add(port);
            }

            var addButton = new Button(AddChoicePort) { text = "+ Choice" };
            outputContainer.Add(addButton);
        }

        private void CreateBody()
        {
            var textField = new TextField("Dialogue") { value = Data.Text, multiline = true };
            textField.RegisterValueChangedCallback(evt => OnTextChanged(evt.newValue));
            mainContainer.Add(textField);
        }

        public Port GetPortForChoice(DialogueChoiceData choice)
        {
            var index = Data.Choices.IndexOf(choice);
            if (index >= 0 && index < outputPorts.Count)
            {
                return outputPorts[index];
            }

            return null;
        }

        public DialogueChoiceData GetChoiceForPort(Port port)
        {
            var index = outputPorts.IndexOf(port);
            if (index >= 0 && index < Data.Choices.Count)
            {
                return Data.Choices[index];
            }

            return null;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Data.Position = newPos.position;
            EditorUtility.SetDirty(graphView.Tree);
        }

        private void OnTextChanged(string value)
        {
            Undo.RecordObject(graphView.Tree, "Edit Dialogue Text");
            Data.Text = value;
            title = string.IsNullOrEmpty(value) ? "Dialogue" : value;
            EditorUtility.SetDirty(graphView.Tree);
        }

        private void OnChoiceTextChanged(DialogueChoiceData choice, Port port, string value)
        {
            Undo.RecordObject(graphView.Tree, "Edit Dialogue Choice");
            choice.Text = value;
            port.portName = string.IsNullOrEmpty(value) ? "Choice" : value;
            EditorUtility.SetDirty(graphView.Tree);
        }

        private void AddChoicePort()
        {
            Undo.RecordObject(graphView.Tree, "Add Dialogue Choice");
            var choice = new DialogueChoiceData { Text = "Choice" };
            Data.Choices.Add(choice);
            EditorUtility.SetDirty(graphView.Tree);
            graphView.SetTree(graphView.Tree);
        }

        private void RemoveChoicePort(DialogueChoiceData choice, Port port)
        {
            Undo.RecordObject(graphView.Tree, "Remove Dialogue Choice");

            var connectedEdges = port.connections.ToList();
            foreach (var edge in connectedEdges)
            {
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                graphView.RemoveElement(edge);
            }

            Data.Choices.Remove(choice);
            EditorUtility.SetDirty(graphView.Tree);
            graphView.SetTree(graphView.Tree);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            graphView.NotifySelection(this);
        }
    }
}
