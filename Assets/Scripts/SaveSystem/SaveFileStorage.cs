using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem
{
    public class SaveFileStorage
    {
        private const string DirectoryName = "Saves";
        private readonly string _saveDirectory;

        public SaveFileStorage()
        {
            _saveDirectory = Path.Combine(Application.persistentDataPath, DirectoryName);
        }

        public async Task WriteAsync(string slotName, SaveDataContainer data)
        {
            EnsureDirectory();

            try
            {
                var json = JsonUtility.ToJson(data, true);
                var path = GetPath(slotName);
                using var writer = new StreamWriter(path, false);
                await writer.WriteAsync(json);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to write save for slot '{slotName}': {exception.Message}\n{exception.StackTrace}");
                throw;
            }
        }

        public async Task<SaveDataContainer> ReadAsync(string slotName)
        {
            var path = GetPath(slotName);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"Save file for slot '{slotName}' not found at {path}");
                return null;
            }

            try
            {
                using var reader = new StreamReader(path);
                var json = await reader.ReadToEndAsync();
                return JsonUtility.FromJson<SaveDataContainer>(json);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to read save for slot '{slotName}': {exception.Message}\n{exception.StackTrace}");
                return null;
            }
        }

        public bool Delete(string slotName)
        {
            var path = GetPath(slotName);
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to delete save '{slotName}': {exception.Message}");
                return false;
            }
        }

        public IEnumerable<SaveSlotInfo> GetAvailableSaves()
        {
            EnsureDirectory();

            if (!Directory.Exists(_saveDirectory))
            {
                yield break;
            }

            foreach (var path in Directory.GetFiles(_saveDirectory, "*.json"))
            {
                var slotName = Path.GetFileNameWithoutExtension(path);
                var info = TryReadMetadata(path, slotName);
                if (info != null)
                {
                    yield return info;
                }
            }
        }

        private void EnsureDirectory()
        {
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }
        }

        private string GetPath(string slotName)
        {
            return Path.Combine(_saveDirectory, $"{slotName}.json");
        }

        private SaveSlotInfo TryReadMetadata(string path, string slotName)
        {
            try
            {
                var json = File.ReadAllText(path);
                var container = JsonUtility.FromJson<SaveDataContainer>(json);

                if (container == null)
                {
                    return null;
                }

                return new SaveSlotInfo
                {
                    SlotName = slotName,
                    Version = container.Version,
                    SavedAtTicks = container.SavedAtTicks,
                    SceneName = container.ActiveScene
                };
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Failed to read save metadata for '{slotName}': {exception.Message}");
                return null;
            }
        }
    }
}
