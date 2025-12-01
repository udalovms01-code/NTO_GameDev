using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Runtime;
using Localization;
using Localization.Editor;
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
        private readonly List<Port> outputPorts = new();
        private readonly DialogueGraphView graphView;

        private TextField textRuField;
        private TextField localizationKeyField;
        private TextField englishPreviewField;

        private VisualElement choicesContainer;

        public DialogueNodeView(DialogueNodeData data, DialogueGraphView graphView)
        {
            Data = data;
            this.graphView = graphView;

            viewDataKey = data.Guid;

            title = data.Text is { Length: > 0 } ? data.Text : "Dialogue";

            CreateInput();
            CreateOutputs();
            CreateBody();

            RefreshExpandedState();
            RefreshPorts();
        }

        /* ----------------------
         * INPUT PORT
         * ---------------------- */

        private void CreateInput()
        {
            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            InputPort.portName = "Input";
            inputContainer.Add(InputPort);
        }

        /* ----------------------
         * OUTPUT PORTS
         * ---------------------- */

        private void CreateOutputs()
        {
            outputPorts.Clear();
            outputContainer.Clear();

            for (var i = 0; i < Data.Choices.Count; i++)
            {
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                port.portName = Data.Choices[i].Text; 
                outputPorts.Add(port);
                outputContainer.Add(port);
            }
        }

        public Port GetPortForChoice(DialogueChoiceData choice)
        {
            int index = Data.Choices.IndexOf(choice);
            if (index >= 0 && index < outputPorts.Count)
                return outputPorts[index];
            return null;
        }

        public DialogueChoiceData GetChoiceForPort(Port port)
        {
            int index = outputPorts.IndexOf(port);
            if (index >= 0 && index < Data.Choices.Count)
                return Data.Choices[index];
            return null;
        }

        /* ----------------------
         * BODY (UI)
         * ---------------------- */

        private void CreateBody()
        {
            /* --- Dialogue RU --- */
            textRuField = new TextField("Dialogue (RU)")
            {
                value = Data.Text,
                multiline = true
            };
            textRuField.RegisterValueChangedCallback(evt => OnTextChanged(evt.newValue));
            mainContainer.Add(textRuField);

            /* --- Localization Key --- */
            localizationKeyField = new TextField("Localization Key")
            {
                value = Data.LocalizationKey,
                multiline = false
            };
            localizationKeyField.RegisterValueChangedCallback(evt => OnLocalizationKeyChanged(evt.newValue));
            mainContainer.Add(localizationKeyField);

            /* --- English preview (readonly) --- */
            englishPreviewField = new TextField("English (preview)")
            {
                value = GetLocalizedPreview(),
                multiline = true
            };
            englishPreviewField.SetEnabled(false);
            mainContainer.Add(englishPreviewField);

            /* --- Auto-translate (one button) --- */
            var btnTranslateNode = new Button(AutoTranslateNodeText)
            {
                text = "Auto translate to EN"
            };
            mainContainer.Add(btnTranslateNode);

            /* -----------------------
             * CHOICES SECTION
             * ----------------------- */

            var choicesTitle = new Label("Choices:")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginTop = 6,
                    marginBottom = 4
                }
            };
            mainContainer.Add(choicesTitle);

            choicesContainer = new VisualElement();
            mainContainer.Add(choicesContainer);

            DrawChoicesSection();

            var btnAddChoice = new Button(AddChoice)
            {
                text = "+ Add Choice"
            };
            mainContainer.Add(btnAddChoice);
        }

        private void DrawChoicesSection()
        {
            choicesContainer.Clear();

            for (int i = 0; i < Data.Choices.Count; i++)
            {
                int index = i;
                DialogueChoiceData choice = Data.Choices[index];

                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.marginBottom = 4;

                /* --- Text --- */
                var textField = new TextField()
                {
                    value = choice.Text
                };
                textField.style.minWidth = 120;
                textField.RegisterValueChangedCallback(evt =>
                {
                    Undo.RecordObject(graphView.Tree, "Edit Choice Text");
                    choice.Text = evt.newValue;
                    UpdateOutputPortName(index, evt.newValue);
                    EditorUtility.SetDirty(graphView.Tree);
                });
                row.Add(textField);

                /* --- Key --- */
                var keyField = new TextField()
                {
                    value = choice.LocalizationKey,
                };
                keyField.style.minWidth = 120;
                keyField.RegisterValueChangedCallback(evt =>
                {
                    Undo.RecordObject(graphView.Tree, "Edit Choice Key");
                    choice.LocalizationKey = evt.newValue;
                    EditorUtility.SetDirty(graphView.Tree);
                });
                row.Add(keyField);

                /* --- Delete --- */
                var deleteButton = new Button(() => RemoveChoice(index))
                {
                    text = "X"
                };
                deleteButton.style.unityFontStyleAndWeight = FontStyle.Bold;
                deleteButton.style.color = Color.red;
                row.Add(deleteButton);

                choicesContainer.Add(row);
            }
        }

        /* ----------------------
         * LOGIC: update text, keys, choices
         * ---------------------- */

        private void UpdateOutputPortName(int index, string newName)
        {
            if (index >= 0 && index < outputPorts.Count)
            {
                outputPorts[index].portName = newName;
            }
        }

        private void AddChoice()
        {
            Undo.RecordObject(graphView.Tree, "Add Dialogue Choice");

            Data.Choices.Add(new DialogueChoiceData
            {
                Text = "New Choice",
                LocalizationKey = ""
            });

            EditorUtility.SetDirty(graphView.Tree);

            CreateOutputs();
            DrawChoicesSection();
            RefreshExpandedState();
            RefreshPorts();
        }

        private void RemoveChoice(int index)
        {
            if (index < 0 || index >= Data.Choices.Count)
                return;

            Undo.RecordObject(graphView.Tree, "Remove Dialogue Choice");

            var choice = Data.Choices[index];
            var port = outputPorts[index];

            // Disconnect edges
            foreach (var edge in port.connections.ToList())
                graphView.RemoveElement(edge);

            Data.Choices.RemoveAt(index);
            EditorUtility.SetDirty(graphView.Tree);

            CreateOutputs();
            DrawChoicesSection();
            RefreshExpandedState();
            RefreshPorts();
        }

        /* ----------------------
         * TEXT + LOCALIZATION
         * ---------------------- */

        private void OnTextChanged(string value)
        {
            Undo.RecordObject(graphView.Tree, "Edit Dialogue Text");
            Data.Text = value;
            title = string.IsNullOrEmpty(value) ? "Dialogue" : value;
            EditorUtility.SetDirty(graphView.Tree);
            UpdateEnglishPreview();
        }

        private void OnLocalizationKeyChanged(string value)
        {
            Undo.RecordObject(graphView.Tree, "Edit Dialogue Localization Key");
            Data.LocalizationKey = value;
            EditorUtility.SetDirty(graphView.Tree);
            UpdateEnglishPreview();
        }

        private string GetLocalizedPreview()
        {
            var table = LocalizationEditorUtility.LoadDefaultTable();
            if (table == null || string.IsNullOrEmpty(Data.LocalizationKey))
                return string.Empty;

            return table.GetValue(Data.LocalizationKey, LocalizationLanguage.English, string.Empty);
        }

        private void UpdateEnglishPreview()
        {
            if (englishPreviewField != null)
                englishPreviewField.value = GetLocalizedPreview();
        }

        /* ----------------------
         * AUTO TRANSLATE
         * ---------------------- */

        private async void AutoTranslateNodeText()
        {
            try
            {
                var table = LocalizationEditorUtility.GetOrCreateDefaultTable();

                if (string.IsNullOrEmpty(Data.LocalizationKey))
                {
                    Data.LocalizationKey = $"dialogue_{Data.Guid}";
                    localizationKeyField.value = Data.LocalizationKey;
                }

                var translation = await LocalizationTranslator.TranslateRuToEn(
                    Data.Text, LocalizationTranslatorProvider.Google);

                table.SetValue(Data.LocalizationKey, LocalizationLanguage.Russian, Data.Text);
                table.SetValue(Data.LocalizationKey, LocalizationLanguage.English, translation);

                EditorUtility.SetDirty(table);
                EditorUtility.SetDirty(graphView.Tree);

                englishPreviewField.value = translation;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Localization translation failed: {ex.Message}");
            }
        }

        /* ----------------------
         * SELECTION
         * ---------------------- */

        public override void OnSelected()
        {
            base.OnSelected();
            graphView.NotifySelection(this);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Data.Position = newPos.position;
            EditorUtility.SetDirty(graphView.Tree);
        }
    }
}
