using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Runtime;
using Localization;
using Localization.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
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

            title = string.IsNullOrEmpty(data.Text) ? "Dialogue" : data.Text;

            CreateInput();
            CreateOutputs();
            CreateBody();

            RefreshExpandedState();
            RefreshPorts();
        }

        /* ----------------------
         * INPUT PORT (MULTI)
         * ---------------------- */

        private void CreateInput()
        {
            InputPort = InstantiatePort(
                Orientation.Horizontal,
                Direction.Input,
                Port.Capacity.Multi, // несколько входящих связей
                typeof(bool)
            );
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
                var choice = Data.Choices[i];

                var port = InstantiatePort(
                    Orientation.Horizontal,
                    Direction.Output,
                    Port.Capacity.Single,
                    typeof(bool)
                );

                port.portName = choice.Text;
                outputPorts.Add(port);
                outputContainer.Add(port);
            }
        }

        public Port GetPortForChoice(DialogueChoiceData choice)
        {
            int index = Data.Choices.IndexOf(choice);
            return index >= 0 ? outputPorts[index] : null;
        }

        public DialogueChoiceData GetChoiceForPort(Port port)
        {
            int index = outputPorts.IndexOf(port);
            return index >= 0 ? Data.Choices[index] : null;
        }

        /* ----------------------
         * BODY UI
         * ---------------------- */

        private void CreateBody()
        {
            /* RU */
            var ruRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            ruRow.Add(new Label("RU")
            {
                style =
                {
                    width = 30,
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            });

            textRuField = new TextField
            {
                value = Data.Text,
                multiline = true
            };
            textRuField.RegisterValueChangedCallback(evt => OnTextChanged(evt.newValue));
            ruRow.Add(textRuField);
            mainContainer.Add(ruRow);

            /* Localization key */
            localizationKeyField = new TextField("Localization Key")
            {
                value = Data.LocalizationKey
            };
            localizationKeyField.RegisterValueChangedCallback(evt => OnLocalizationKeyChanged(evt.newValue));
            mainContainer.Add(localizationKeyField);

            /* EN (preview, редактируемый) */
            var enRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            enRow.Add(new Label("EN")
            {
                style =
                {
                    width = 30,
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            });

            englishPreviewField = new TextField
            {
                value = GetLocalizedPreview(),
                multiline = true
            };
            englishPreviewField.RegisterValueChangedCallback(evt => OnEnglishPreviewChanged(evt.newValue));
            enRow.Add(englishPreviewField);
            mainContainer.Add(enRow);
            
            var soField = new ObjectField("OnEnter Event:")
            {
                objectType = typeof(Dialogues.Events.Event),
                allowSceneObjects = false,
                value = Data.OnEnter
            };

            soField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(graphView.Tree, "OnEnter Event");
                Data.OnEnter = evt.newValue as Dialogues.Events.Event;
                EditorUtility.SetDirty(graphView.Tree);
            });

            mainContainer.Add(soField);

            /* Translate button */
            var btnTranslateNode = new Button(AutoTranslateNodeText)
            {
                text = "Auto translate to EN"
            };
            mainContainer.Add(btnTranslateNode);

            /* CHOICES SECTION */
            var label = new Label("Choices:")
            {
                style = { unityFontStyleAndWeight = FontStyle.Bold }
            };
            mainContainer.Add(label);

            choicesContainer = new VisualElement();
            mainContainer.Add(choicesContainer);

            DrawChoicesSection();

            var btnAddChoice = new Button(AddChoice)
            {
                text = "+ Add Choice"
            };
            mainContainer.Add(btnAddChoice);
        }

        /* ----------------------
         * CHOICES
         * ---------------------- */

        private void DrawChoicesSection()
        {
            choicesContainer.Clear();

            for (int i = 0; i < Data.Choices.Count; i++)
            {
                int index = i;
                var choice = Data.Choices[index];

                var row = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        marginBottom = 4
                    }
                };

                // RU
                var ruField = new TextField
                {
                    value = choice.Text
                };
                ruField.style.minWidth = 120;
                ruField.RegisterValueChangedCallback(evt =>
                {
                    Undo.RecordObject(graphView.Tree, "Edit choice RU");
                    choice.Text = evt.newValue;
                    if (index < outputPorts.Count)
                        outputPorts[index].portName = evt.newValue;
                    EditorUtility.SetDirty(graphView.Tree);
                });
                row.Add(ruField);

                // EN (используем LocalizationKey как хранение EN-текста для варианта)
                var enField = new TextField
                {
                    value = choice.LocalizationKey
                };
                enField.style.minWidth = 120;
                enField.RegisterValueChangedCallback(evt =>
                {
                    Undo.RecordObject(graphView.Tree, "Edit choice EN");
                    choice.LocalizationKey = evt.newValue;
                    EditorUtility.SetDirty(graphView.Tree);
                });
                row.Add(enField);

                // Auto EN
                var autoButton = new Button(() => AutoTranslateChoice(choice, enField))
                {
                    text = "Auto EN"
                };
                autoButton.style.minWidth = 70;
                row.Add(autoButton);

                // Delete
                var deleteButton = new Button(() => RemoveChoice(index))
                {
                    text = "X"
                };
                deleteButton.style.color = Color.red;
                deleteButton.style.unityFontStyleAndWeight = FontStyle.Bold;
                deleteButton.style.minWidth = 25;
                row.Add(deleteButton);

                choicesContainer.Add(row);
            }
        }

        private void AddChoice()
        {
            Undo.RecordObject(graphView.Tree, "Add choice");

            Data.Choices.Add(new DialogueChoiceData
            {
                Text = "New Choice",
                LocalizationKey = string.Empty
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

            Undo.RecordObject(graphView.Tree, "Remove choice");

            var port = outputPorts[index];

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
         * TEXT & LOCALIZATION
         * ---------------------- */

        private void OnTextChanged(string value)
        {
            Undo.RecordObject(graphView.Tree, "Edit Dialogue RU");
            Data.Text = value;
            title = string.IsNullOrEmpty(value) ? "Dialogue" : value;
            EditorUtility.SetDirty(graphView.Tree);

            UpdateEnglishPreview();
        }

        private void OnLocalizationKeyChanged(string value)
        {
            Undo.RecordObject(graphView.Tree, "Edit Dialogue Key");
            Data.LocalizationKey = value;
            EditorUtility.SetDirty(graphView.Tree);

            UpdateEnglishPreview();
        }

        private void OnEnglishPreviewChanged(string value)
        {
            // Редактируем EN вручную → пишем в таблицу
            Undo.RecordObject(graphView.Tree, "Edit Dialogue EN");

            if (string.IsNullOrEmpty(Data.LocalizationKey))
            {
                Data.LocalizationKey = $"dialogue_{Data.Guid}";
                localizationKeyField.SetValueWithoutNotify(Data.LocalizationKey);
            }

            var table = LocalizationEditorUtility.GetOrCreateDefaultTable();
            table.SetValue(Data.LocalizationKey, LocalizationLanguage.English, value);

            EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(graphView.Tree);
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
            if (englishPreviewField == null)
                return;

            englishPreviewField.SetValueWithoutNotify(GetLocalizedPreview());
        }

        /* ----------------------
         * TRANSLATION
         * ---------------------- */

        private async void AutoTranslateNodeText()
        {
            try
            {
                if (string.IsNullOrEmpty(Data.Text))
                    return;

                if (string.IsNullOrEmpty(Data.LocalizationKey))
                {
                    Data.LocalizationKey = $"dialogue_{Data.Guid}";
                    localizationKeyField.SetValueWithoutNotify(Data.LocalizationKey);
                }

                var table = LocalizationEditorUtility.GetOrCreateDefaultTable();

                var translation = await LocalizationTranslator.TranslateRuToEn(
                    Data.Text,
                    LocalizationTranslatorProvider.Google
                );

                table.SetValue(Data.LocalizationKey, LocalizationLanguage.Russian, Data.Text);
                table.SetValue(Data.LocalizationKey, LocalizationLanguage.English, translation);

                EditorUtility.SetDirty(table);
                EditorUtility.SetDirty(graphView.Tree);

                englishPreviewField.SetValueWithoutNotify(translation);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Translation failed: " + ex.Message);
            }
        }

        private async void AutoTranslateChoice(DialogueChoiceData choice, TextField enField)
        {
            try
            {
                if (string.IsNullOrEmpty(choice.Text))
                    return;

                Undo.RecordObject(graphView.Tree, "Translate choice EN");

                var translation = await LocalizationTranslator.TranslateRuToEn(
                    choice.Text,
                    LocalizationTranslatorProvider.Google
                );

                choice.LocalizationKey = translation; // для выбора EN просто хранится как текст
                enField.SetValueWithoutNotify(translation);

                EditorUtility.SetDirty(graphView.Tree);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Choice translation failed: " + ex.Message);
            }
        }

        /* ----------------------
         * SELECTION / POSITION
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
