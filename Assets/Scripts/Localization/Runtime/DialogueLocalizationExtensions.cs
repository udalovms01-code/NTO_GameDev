using DialogueSystem.Runtime;

namespace Localization
{
    public static class DialogueLocalizationExtensions
    {
        public static string GetLocalizedText(this DialogueNodeData node)
        {
            if (node == null)
            {
                return string.Empty;
            }

            return string.IsNullOrEmpty(node.LocalizationKey)
                ? node.Text
                : LocalizationManager.GetRaw(node.LocalizationKey, node.Text);
        }

        public static string GetLocalizedText(this DialogueChoiceData choice)
        {
            if (choice == null)
            {
                return string.Empty;
            }

            return string.IsNullOrEmpty(choice.LocalizationKey)
                ? choice.Text
                : LocalizationManager.GetRaw(choice.LocalizationKey, choice.Text);
        }
    }
}
