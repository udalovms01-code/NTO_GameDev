using DialogueSystem.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class DialogueGraphWindow : EditorWindow
    {
        private DialogueGraphView graphView;
        private ObjectField treeField;
        private IMGUIContainer inspectorContainer;
        private DialogueNodeView inspectedNode;

        [MenuItem("Window/Dialogue Graph")]
        public static void Open()
        {
            var window = GetWindow<DialogueGraphWindow>();
            window.titleContent = new GUIContent("Dialogue Graph");
            window.Show();
        }

        private void OnEnable()
        {
            rootVisualElement.Clear();
            CreateToolbar();
            CreateGraphView();
            CreateInspector();
        }

        private void CreateToolbar()
        {
            var toolbar = new Toolbar();

            treeField = new ObjectField("Dialogue Tree")
            {
                objectType = typeof(DialogueTree),
                allowSceneObjects = false
            };
            treeField.RegisterValueChangedCallback(evt => LoadTree(evt.newValue as DialogueTree));
            toolbar.Add(treeField);

            var newButton = new ToolbarButton(CreateNewTree)
            {
                text = "New Dialogue"
            };
            toolbar.Add(newButton);

            rootVisualElement.Add(toolbar);
        }

        private void CreateGraphView()
        {
            graphView = new DialogueGraphView(this)
            {
                name = "Dialogue Graph"
            };
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void CreateInspector()
        {
            inspectorContainer = new IMGUIContainer(DrawInspector)
            {
                style =
                {
                    unityTextAlign = TextAnchor.UpperLeft,
                    paddingTop = 4,
                    paddingLeft = 8,
                    paddingRight = 8,
                    paddingBottom = 8
                }
            };

            var inspectorBorder = new VisualElement();
            inspectorBorder.style.borderLeftWidth = 1;
            inspectorBorder.style.borderLeftColor = new Color(0.25f, 0.25f, 0.25f);
            inspectorBorder.style.width = 250;
            inspectorBorder.style.flexShrink = 0;
            inspectorBorder.Add(inspectorContainer);

            var splitView = new TwoPaneSplitView(0, -1, TwoPaneSplitViewOrientation.Horizontal)
            {
                name = "DialogueGraphSplitView"
            };
            splitView.Add(graphView);
            splitView.Add(inspectorBorder);

            rootVisualElement.Add(splitView);
        }

        private void DrawInspector()
        {
            if (inspectedNode == null)
            {
                EditorGUILayout.LabelField("No node selected.");
                return;
            }

            var data = inspectedNode.Data;
            if (data == null)
            {
                EditorGUILayout.LabelField("Invalid selection.");
                return;
            }

            EditorGUILayout.LabelField("Node GUID", data.Guid);
            EditorGUILayout.Space();

            var tree = graphView.Tree;
            if (tree != null)
            {
                var isStart = tree.StartNodeGuid == data.Guid;
                var newStart = EditorGUILayout.Toggle("Start Node", isStart);
                if (newStart != isStart)
                {
                    Undo.RecordObject(tree, "Set Start Node");
                    tree.StartNodeGuid = newStart ? data.Guid : string.Empty;
                    EditorUtility.SetDirty(tree);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Outgoing Links", inspectedNode.outputContainer.childCount.ToString());
        }

        private void CreateNewTree()
        {
            var path = EditorUtility.SaveFilePanelInProject("Create Dialogue Tree", "DialogueTree", "asset", "Choose location for dialogue tree asset.");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var tree = ScriptableObject.CreateInstance<DialogueTree>();
            AssetDatabase.CreateAsset(tree, path);
            AssetDatabase.SaveAssets();
            LoadTree(tree);
            treeField.value = tree;
        }

        public void LoadTree(DialogueTree tree)
        {
            graphView.SetTree(tree);
        }

        public void InspectNode(DialogueNodeView nodeView)
        {
            inspectedNode = nodeView;
            inspectorContainer.MarkDirtyRepaint();
        }
    }
}
