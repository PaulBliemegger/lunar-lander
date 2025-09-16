using UnityEngine;

namespace ProjectSelene.Code
{
    public class ConfigBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private MonoBehaviour[] consumers;

        void Awake()
        {
            if (!config) { Debug.LogError("No GameConfig assigned."); return; }

            foreach (var mb in consumers)
            {
                if (mb is IConfigConsumer c)
                {
                    c.ApplyConfig(config);
                }
            }
        }
    }
}