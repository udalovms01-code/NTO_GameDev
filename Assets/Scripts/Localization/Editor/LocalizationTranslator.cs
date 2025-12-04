using System;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Networking;

namespace Localization.Editor
{
    public enum LocalizationTranslatorProvider
    {
        Google,
        Yandex
    }

    public static class LocalizationTranslator
    {
        private const string GoogleUrl = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=ru&tl=en&dt=t&q={0}";
        private const string YandexUrl = "https://translate.yandex.net/api/v1.5/tr.json/translate?key={0}&text={1}&lang=ru-en";
        private const string YandexApiKeyPref = "Localization.YandexApiKey";

        public static void SetYandexApiKey(string apiKey)
        {
            EditorPrefs.SetString(YandexApiKeyPref, apiKey);
        }

        public static string GetYandexApiKey()
        {
            return EditorPrefs.GetString(YandexApiKeyPref, string.Empty);
        }

        public static Task<string> TranslateRuToEn(string text, LocalizationTranslatorProvider provider)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult(string.Empty);
            }

            return provider == LocalizationTranslatorProvider.Yandex ? TranslateWithYandex(text) : TranslateWithGoogle(text);
        }

        private static async Task<string> TranslateWithGoogle(string text)
        {
            var url = string.Format(GoogleUrl, UnityWebRequest.EscapeURL(text));
            using (var request = UnityWebRequest.Get(url))
            {
                request.timeout = 10;
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await System.Threading.Tasks.Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new InvalidOperationException($"Google translate error: {request.error}");
                }

                return ParseGoogleResponse(request.downloadHandler.text);
            }
        }

        private static async Task<string> TranslateWithYandex(string text)
        {
            var apiKey = GetYandexApiKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Yandex API key is not set. Specify it in the localization window.");
            }

            var url = string.Format(YandexUrl, apiKey, UnityWebRequest.EscapeURL(text));
            using (var request = UnityWebRequest.Get(url))
            {
                request.timeout = 10;
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await System.Threading.Tasks.Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new InvalidOperationException($"Yandex translate error: {request.error}");
                }

                var response = request.downloadHandler.text;
                var marker = "\"text\":";
                var index = response.IndexOf(marker, StringComparison.Ordinal);
                if (index < 0)
                {
                    return string.Empty;
                }

                var start = response.IndexOf('"', index + marker.Length);
                var end = response.IndexOf('"', start + 1);
                return start >= 0 && end > start ? response.Substring(start + 1, end - start - 1) : string.Empty;
            }
        }

        private static string ParseGoogleResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            var insideText = false;
            for (var i = 0; i < response.Length; i++)
            {
                var c = response[i];
                if (!insideText)
                {
                    if (c == '\"')
                    {
                        insideText = true;
                    }

                    continue;
                }

                if (c == '\\')
                {
                    if (i + 1 < response.Length)
                    {
                        builder.Append(response[i + 1]);
                        i++;
                    }

                    continue;
                }

                if (c == '\"')
                {
                    break;
                }

                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
