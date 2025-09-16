using UnityEngine;

namespace ProjectSelene.Code
{
    public class GameConfigLoader
    {
        public static bool TryLoadEffective(string key, out GameConfigData data)
        {
            // Load base (must exist)
            if (!ConfigFile.TryLoadBase(key, out var baseJson))
            { data = null; return false; }

            var baseData = JsonUtility.FromJson<GameConfigData>(baseJson);

            // If an override exists, prefer it entirely
            if (ConfigFile.TryLoadOverride(key, out var overJson))
            {
                data = JsonUtility.FromJson<GameConfigData>(overJson);
                // Optional: merge missing/new fields from base into override here.
                return true;
            }

            data = baseData;
            return true;
        }
    }
}