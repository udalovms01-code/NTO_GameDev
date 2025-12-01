using System;

namespace DialogueSystem.Runtime
{
    [Serializable]
    public class DialogueChoiceData
    {
        public string Text = "";
        public string TargetNodeGuid = string.Empty;
    }
}
