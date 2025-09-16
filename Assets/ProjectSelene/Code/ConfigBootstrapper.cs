using UnityEngine;

namespace ProjectSelene.Code
{
    public class ConfigBootstrapper : MonoBehaviour
    {
        [SerializeField] public GameConfig Config;
        [SerializeField] private MonoBehaviour[] consumers;

        void Start()
        {
            if (!Config) { Debug.LogError("No GameConfig assigned."); return; }

            foreach (var mb in consumers)
            {
                if (mb is IConfigConsumer c)
                {
                    c.ApplyConfig(Config);
                }
            }
        }
    }
}