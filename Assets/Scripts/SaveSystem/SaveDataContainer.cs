using System;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.Serialization;

namespace SaveSystem
{
    [Serializable]
    public class SaveDataContainer
    {
        public string Version;
        public long SavedAtTicks;
        public string ActiveScene;
        public GameplayStateData gameplay = new GameplayStateData();
        public List<EntityStateData> Entities = new List<EntityStateData>();
    }

    [Serializable]
    public class GameplayStateData
    {
        public GameState CurrentState = GameState.WaitingForSlicedFish;
        public int CurrentDay = 1;
        public float Hunger = 1f;
    }

    [Serializable]
    public class EntityStateData
    {
        public string Id;
        public string PrefabId;
        public string SceneName;
        public Vector3 Position;
        public List<SerializableKeyValuePair> Data = new List<SerializableKeyValuePair>();

        public void SetValue(string key, string value)
        {
            var existing = Data.Find(pair => pair.Key == key);
            if (existing != null)
            {
                existing.Value = value;
            }
            else
            {
                Data.Add(new SerializableKeyValuePair(key, value));
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            var existing = Data.Find(pair => pair.Key == key);
            if (existing != null)
            {
                value = existing.Value;
                return true;
            }

            value = string.Empty;
            return false;
        }
    }

    [Serializable]
    public class SerializableKeyValuePair
    {
        public string Key;
        public string Value;

        public SerializableKeyValuePair()
        {
        }

        public SerializableKeyValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
