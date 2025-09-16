using System.IO;
using UnityEngine;
namespace ProjectSelene.Code
{

    public static class ConfigFile
    {
        static string Path => System.IO.Path.Combine(Application.persistentDataPath, "gameconfig.json");

        public static void Save(GameConfigData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Path, json);
        }

        public static bool TryLoad(out GameConfigData data)
        {
            if (File.Exists(Path))
            {
                string json = File.ReadAllText(Path);
                data = JsonUtility.FromJson<GameConfigData>(json);
                return true;
            }
            data = null;
            return false;
        }
    }

}