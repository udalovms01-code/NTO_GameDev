using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Runtime
{
    public class DialogueRunner
    {
        private readonly DialogueTree tree;
        private DialogueNodeData currentNode;

        public DialogueRunner(DialogueTree tree)
        {
            this.tree = tree;
        }

        public DialogueNodeData CurrentNode => currentNode;

        public bool Begin()
        {
            if (tree == null || string.IsNullOrEmpty(tree.StartNodeGuid))
            {
                currentNode = null;
                return false;
            }

            currentNode = tree.GetNode(tree.StartNodeGuid);
            return currentNode != null;
        }

        public IReadOnlyList<DialogueChoiceData> GetChoices()
        {
            return currentNode?.Choices;
        }

        public bool TryChoose(int index)
        {
            if (currentNode == null || index < 0 || index >= currentNode.Choices.Count)
            {
                return false;
            }

            var targetGuid = currentNode.Choices[index].TargetNodeGuid;
            if (string.IsNullOrEmpty(targetGuid))
            {
                currentNode = null;
                return false;
            }

            var nextNode = tree.GetNode(targetGuid);
            if (nextNode == null)
            {
                currentNode = null;
                return false;
            }

            currentNode = nextNode;
            return true;
        }

        public void Reset()
        {
            currentNode = null;
        }
    }
}
