using DialogueSystem.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
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
            BuildSplitLayout();
        }

        // -------------------------
        // TOOLBAR
        // -------------------------
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

        // -------------------------
        // GRAPH VIEW
        // -------------------------
        private void CreateGraphView()
        {
            graphView = new DialogueGraphView(this)
            {
                name = "Dialogue Graph"
            };
        }

        // -------------------------
        // INSPECTOR PANEL
        // -------------------------
        private void CreateInspector()
        {
            inspectorContainer = new IMGUIContainer(DrawInspector)
            {
                style =
                {
                    unityTextAlign = TextAnchor.UpperLeft,
                    paddingTop = 6,
                    paddingLeft = 8,
                    paddingRight = 8,
                    paddingBottom = 8
                }
            };
        }

        // -------------------------
        // SPLIT VIEW LAYOUT
        // -------------------------
        private void BuildSplitLayout()
        {
            var splitView = new TwoPaneSplitView(0, 650, TwoPaneSplitViewOrientation.Horizontal)
            {
                name = "DialogueGraphSplitView"
            };

            // Left panel — Graph
            splitView.Add(graphView);

            // Right panel — Inspector
            var inspectorBorder = new VisualElement();
            inspectorBorder.style.width = 280;
            inspectorBorder.style.flexShrink = 0;
            inspectorBorder.style.borderLeftWidth = 1;
            inspectorBorder.style.borderLeftColor = new Color(.25f, .25f, .25f);
            inspectorBorder.Add(inspectorContainer);

            splitView.Add(inspectorBorder);

            rootVisualElement.Add(splitView);
        }

        // -------------------------
        // INSPECTOR DRAW
        // -------------------------
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
                EditorGUILayout.LabelField("Invalid node data.");
                return;
            }

            EditorGUILayout.LabelField("Node GUID", data.Guid);
            EditorGUILayout.Space();

            var tree = graphView.Tree;
            if (tree != null)
            {
                bool isStart = tree.StartNodeGuid == data.Guid;
                bool newStart = EditorGUILayout.Toggle("Start Node", isStart);

                if (newStart != isStart)
                {
                    Undo.RecordObject(tree, "Set Start Node");
                    tree.StartNodeGuid = newStart ? data.Guid : string.Empty;
                    EditorUtility.SetDirty(tree);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Outgoing Links", inspectedNode.outputContainer.childCount.ToString());
        }

        // -------------------------
        // TREE MANAGEMENT
        // -------------------------
        private void CreateNewTree()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                "Create Dialogue Tree",
                "DialogueTree",
                "asset",
                "Choose location"
            );

            if (string.IsNullOrEmpty(path))
                return;

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
