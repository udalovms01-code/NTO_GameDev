using System;

namespace SaveSystem
{
    [Serializable]
    public class SaveSlotInfo
    {
        public string SlotName;
        public string Version;
        public long SavedAtTicks;
        public string SceneName;

        public DateTime SavedAt => new DateTime(SavedAtTicks, DateTimeKind.Utc);
    }
}
