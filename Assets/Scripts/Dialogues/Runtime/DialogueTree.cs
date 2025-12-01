using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DialogueSystem.Runtime
{
    [CreateAssetMenu(fileName = "DialogueTree", menuName = "Dialogue/Dialogue Tree")]
    public class DialogueTree : ScriptableObject
    {
        [SerializeField]
        private List<DialogueNodeData> nodes = new List<DialogueNodeData>();

        [SerializeField]
        private string startNodeGuid = string.Empty;

        public IReadOnlyList<DialogueNodeData> Nodes => nodes;

        public string StartNodeGuid
        {
            get => startNodeGuid;
            set => startNodeGuid = value;
        }

        public DialogueNodeData GetNode(string guid)
        {
            return nodes.FirstOrDefault(n => n.Guid == guid);
        }

        public void AddNode(DialogueNodeData node)
        {
            if (!nodes.Contains(node))
            {
                nodes.Add(node);
            }
        }

        public void RemoveNode(DialogueNodeData node)
        {
            if (nodes.Remove(node))
            {
                foreach (var otherNode in nodes)
                {
                    otherNode.Choices.RemoveAll(choice => choice.TargetNodeGuid == node.Guid);
                }

                if (startNodeGuid == node.Guid)
                {
                    startNodeGuid = string.Empty;
                }
            }
        }
    }
}
