using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
    [CreateAssetMenu(fileName = "LocalizationTable", menuName = "Localization/Table")]
    public class LocalizationTable : ScriptableObject
    {
        [SerializeField]
        private List<LocalizationEntry> entries = new List<LocalizationEntry>();

        private readonly Dictionary<string, LocalizationEntry> lookup = new Dictionary<string, LocalizationEntry>();

        public IReadOnlyList<LocalizationEntry> Entries => entries;

        public IEnumerable<string> GetCategories()
        {
            var categories = new HashSet<string>();

            foreach (var entry in entries)
            {
                if (entry == null)
                {
                    continue;
                }

                categories.Add(entry.Category);
            }

            return categories;
        }

        private void OnEnable()
        {
            RebuildLookup();
        }

        public void RebuildLookup()
        {
            lookup.Clear();
            foreach (var entry in entries)
            {
                if (entry == null || string.IsNullOrEmpty(entry.Key) || lookup.ContainsKey(entry.Key))
                {
                    continue;
                }

                lookup.Add(entry.Key, entry);
            }
        }

        public LocalizationEntry GetEntry(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (lookup.TryGetValue(key, out var entry))
            {
                return entry;
            }

            foreach (var candidate in entries)
            {
                if (candidate != null && candidate.Key == key)
                {
                    lookup[key] = candidate;
                    return candidate;
                }
            }

            return null;
        }

        public bool ContainsKey(string key)
        {
            return GetEntry(key) != null;
        }

        public LocalizationEntry AddEntry(string key, string category = "")
        {
            var candidateKey = key;
            var index = 0;

            while (ContainsKey(candidateKey))
            {
                index++;
                candidateKey = $"{key}_{index}";
            }

            var entry = new LocalizationEntry
            {
                Key = candidateKey,
                Category = category,
            };
            entries.Add(entry);
            lookup[candidateKey] = entry;
            return entry;
        }

        public string GetValue(string key, LocalizationLanguage language, string fallback = "")
        {
            var entry = GetEntry(key);
            return entry != null ? entry.GetValue(language) : fallback;
        }

        public LocalizationEntry GetOrCreateEntry(string key)
        {
            var entry = GetEntry(key);
            if (entry != null)
            {
                return entry;
            }

            entry = new LocalizationEntry { Key = key };
            entries.Add(entry);
            lookup[key] = entry;
            return entry;
        }

        public bool RemoveEntry(LocalizationEntry entry)
        {
            if (entry == null)
            {
                return false;
            }

            if (!entries.Remove(entry))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(entry.Key))
            {
                lookup.Remove(entry.Key);
            }

            return true;
        }

        public void SetValue(string key, LocalizationLanguage language, string value)
        {
            var entry = GetOrCreateEntry(key);
            entry.SetValue(language, value);
        }
    }
}
