using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Runtime
{
    [Serializable]
    public class DialogueNodeData
    {
        public string Guid = string.Empty;
        public string Text = "";
        public string LocalizationKey = string.Empty;
        public Dialogues.Events.Event OnEnter = null;
        public Vector2 Position;
        public List<DialogueChoiceData> Choices = new List<DialogueChoiceData>();
    }
}
