using UnityEditor;

namespace Config
{
    public static class ManualTooltipFunctions
    {
        public static void OnRestartPlayerPrefs()
        {
            if (G.main == null) return;
            G.main.RestartPlayerPrefs();
        }

        public static void OnDevModeToggleChanged()
        {
            if (G.main == null) return;
            G.main.devMod = EditorPrefs.GetBool("DevMode");
        }

        public static void OnInvincibleModeToggleChanged()
        {
            if (G.main == null) return;
            G.main.invincibleMod = EditorPrefs.GetBool("InvincibleMode");
        }

        public static void Abcdef()
        {
            
        }
    }
}