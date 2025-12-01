using System;

namespace DialogueSystem.Runtime
{
    [Serializable]
    public class DialogueChoiceData
    {
        public string Text = "";
        public string LocalizationKey = string.Empty;
        public string TargetNodeGuid = string.Empty;
    }
}
