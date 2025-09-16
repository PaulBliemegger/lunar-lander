using System.IO;
using UnityEngine;
namespace ProjectSelene.Code
{
    public static class ConfigFile
    {
        // Base (read-only) files you ship
        public static string BaseDir =>
            Path.Combine(Application.streamingAssetsPath, "Configs");

        // Player overrides
        public static string OverrideDir =>
            Path.Combine(Application.persistentDataPath, "configs");

        public static string BasePath(string key) =>
            Path.Combine(BaseDir, key + ".json");

        public static string OverridePath(string key) =>
            Path.Combine(OverrideDir, key + ".json");

        public static bool TryLoadBase(string key, out string json)
        {
            var path = BasePath(key);
            if (File.Exists(path)) { json = File.ReadAllText(path); return true; }
            json = null; return false;
        }

        public static bool TryLoadOverride(string key, out string json)
        {
            var path = OverridePath(key);
            if (File.Exists(path)) { json = File.ReadAllText(path); return true; }
            json = null; return false;
        }

        public static void SaveOverride(string key, GameConfigData data)
        {
            if (!Directory.Exists(OverrideDir)) Directory.CreateDirectory(OverrideDir);
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(OverridePath(key), json);
#if UNITY_EDITOR
            Debug.Log($"Saved override: {OverridePath(key)}");
#endif
        }

        public static void DeleteOverride(string key)
        {
            var p = OverridePath(key);
            if (File.Exists(p)) File.Delete(p);
        }
    }

}