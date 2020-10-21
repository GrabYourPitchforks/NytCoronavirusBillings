using System;
using System.Collections.Generic;

namespace SchoolSim
{
    public class TimekeepingDictionary<TKey> : Dictionary<TKey, TimeSpan>
    {
        public void AddTime(TKey key, TimeSpan value)
        {
            TryGetValue(key, out TimeSpan existingValue); // existingValue will be 0 if key not found
            existingValue += value;
            this[key] = existingValue;
        }
    }
}
