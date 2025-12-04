using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
    public class LocalizationTableWindow : EditorWindow
    {
        private LocalizationTable table;
        private LocalizationTranslatorProvider translator = LocalizationTranslatorProvider.Google;
        private Vector2 scrollPosition;
        private readonly Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
        private string quickRussianSample = string.Empty;
        private string quickEnglishResult = string.Empty;

        [MenuItem("Window/Localization/Table")]
        public static void Open()
        {
            var window = GetWindow<LocalizationTableWindow>();
            window.titleContent = new GUIContent("Localization");
            window.Show();
        }

        private void OnEnable()
        {
            table = LocalizationEditorUtility.LoadDefaultTable();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                DrawHeader();
                EditorGUILayout.Space(8);
                DrawTranslator();
                EditorGUILayout.Space(8);
                DrawEntries();
            }
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Localization Source", EditorStyles.boldLabel);
            var newTable = (LocalizationTable)EditorGUILayout.ObjectField("Table", table, typeof(LocalizationTable), false);
            if (newTable != table)
            {
                table = newTable;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Create/Use Default Table"))
                {
                    table = LocalizationEditorUtility.GetOrCreateDefaultTable();
                }

                if (table != null && GUILayout.Button("Rebuild Cache"))
                {
                    table.RebuildLookup();
                    EditorUtility.SetDirty(table);
                }
            }
        }

        private void DrawTranslator()
        {
            EditorGUILayout.LabelField("Quick Auto-Translate (RU → EN)", EditorStyles.boldLabel);
            translator = (LocalizationTranslatorProvider)EditorGUILayout.EnumPopup("Service", translator);

            if (translator == LocalizationTranslatorProvider.Yandex)
            {
                var apiKey = LocalizationTranslator.GetYandexApiKey();
                var newKey = EditorGUILayout.TextField("Yandex API Key", apiKey);
                if (newKey != apiKey)
                {
                    LocalizationTranslator.SetYandexApiKey(newKey);
                }
            }

            quickRussianSample = EditorGUILayout.TextField("Russian text", quickRussianSample);
            EditorGUI.BeginDisabledGroup(true);
            quickEnglishResult = EditorGUILayout.TextField("Result", quickEnglishResult);
            EditorGUI.EndDisabledGroup();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Translate"))
                {
                    TriggerTranslation(quickRussianSample, text => quickEnglishResult = text);
                }

                if (GUILayout.Button("Clear"))
                {
                    quickRussianSample = string.Empty;
                    quickEnglishResult = string.Empty;
                }
            }
        }

        private void DrawEntries()
        {
            if (table == null)
            {
                EditorGUILayout.HelpBox("Select or create a localization table to edit entries.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Entries", EditorStyles.boldLabel);
            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scroll.scrollPosition;
                for (var i = 0; i < table.Entries.Count; i++)
                {
                    var entry = table.Entries[i];
                    if (entry == null)
                        continue;

                    var foldoutKey = i.ToString();

                    if (!foldoutStates.ContainsKey(foldoutKey))
                        foldoutStates.Add(foldoutKey, false);

                    foldoutStates[foldoutKey] =
                        EditorGUILayout.Foldout(foldoutStates[foldoutKey], entry.Key, true);

                    if (!foldoutStates[foldoutKey])
                        continue;

                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();

                    var key = EditorGUILayout.TextField("Key", entry.Key);
                    var ru = EditorGUILayout.TextField("Russian", entry.Russian);
                    var en = EditorGUILayout.TextField("English", entry.English);

                    if (EditorGUI.EndChangeCheck())
                    {
                        entry.Key = key;
                        entry.Russian = ru;
                        entry.English = en;
                        table.RebuildLookup();
                        EditorUtility.SetDirty(table);
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Auto translate from RU"))
                        {
                            TriggerTranslation(entry.Russian, text =>
                            {
                                entry.English = text;
                                EditorUtility.SetDirty(table);
                            });
                        }

                        if (GUILayout.Button("Delete"))
                        {
                            Undo.RecordObject(table, "Remove localization entry");
                            table.RemoveEntry(entry);
                            table.RebuildLookup();
                            EditorUtility.SetDirty(table);
                            break;
                        }
                    }

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel--;
                }
            }

            if (GUILayout.Button("Add Entry"))
            {
                Undo.RecordObject(table, "Add localization entry");
                table.AddEntry($"key_{table.Entries.Count}");
                table.RebuildLookup();
                EditorUtility.SetDirty(table);
            }
        }

        private void TriggerTranslation(string russianText, System.Action<string> onResult)
        {
            async void TranslateAsync()
            {
                try
                {
                    var translated = await LocalizationTranslator.TranslateRuToEn(russianText, translator);
                    onResult?.Invoke(translated);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }

            TranslateAsync();
        }
    }
}
