using System;
using System.Collections.Generic;

namespace SchoolSim
{
    public class CountingDictionary<TKey> : Dictionary<TKey, int>
    {
        public void AddCount(TKey key, int value)
        {
            TryGetValue(key, out int existingValue); // existingValue will be 0 if key not found
            existingValue += value;
            this[key] = existingValue;
        }
    }
}
