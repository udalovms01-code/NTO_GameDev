using System.IO;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
    public static class LocalizationEditorUtility
    {
        private const string DefaultTablePath = "Assets/Resources/Localization/LocalizationTable.asset";

        public static LocalizationTable GetOrCreateDefaultTable()
        {
            var table = LoadDefaultTable();
            if (table != null)
            {
                return table;
            }

            var directory = Path.GetDirectoryName(DefaultTablePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            table = ScriptableObject.CreateInstance<LocalizationTable>();
            AssetDatabase.CreateAsset(table, DefaultTablePath);
            AssetDatabase.SaveAssets();
            return table;
        }

        public static LocalizationTable LoadDefaultTable()
        {
            var table = AssetDatabase.LoadAssetAtPath<LocalizationTable>(DefaultTablePath);
            if (table != null)
            {
                return table;
            }

            var guids = AssetDatabase.FindAssets("t:LocalizationTable");
            if (guids.Length == 0)
            {
                return null;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<LocalizationTable>(path);
        }
    }
}
