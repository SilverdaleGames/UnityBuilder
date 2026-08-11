using UnityEngine;

namespace Silverdale.UnityBuilder
{
    public static class Logger
    {
        private static string prefix = "DEFAULT";

        // Method to set a custom prefix
        public static void SetPrefix(string value)
        {
            prefix = value;
        }

        public static void Log(string message)
        {
            Debug.Log($"[{prefix}] {message}");
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning($"[{prefix}] {message}");
        }

        public static void LogError(string message)
        {
            Debug.LogError($"[{prefix}] {message}");
        }
    }
}
