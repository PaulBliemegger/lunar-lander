using Unity.Mathematics;
using UnityEngine;

namespace ProjectSelene.Code.Gameplay
{
    public class AttributeCounter
    {
        private int _currentValue = 0;
        public int MaxValue { get; private set; }

        public AttributeCounter(int maxValue)
        {
            MaxValue = maxValue;
            _currentValue = maxValue;
        }
        public int CurrentValue
        {
            get => _currentValue;
            set => _currentValue = math.max(0, value);
        }
    }
}