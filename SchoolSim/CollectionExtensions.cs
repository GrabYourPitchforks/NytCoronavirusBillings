using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SchoolSim
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Given a <see cref="List{T}"/>, choose one element at random
        /// from the list, and remove it from the list.
        /// </summary>
        /// <returns>
        /// Returns the chosen element.
        /// </returns>
        public static T ChooseAndRemoveRandom<T>(this List<T> list)
        {
            if (list is null) { throw new ArgumentNullException(nameof(list)); }
            if (list.Count == 0) { throw new ArgumentException("Empty list!", nameof(list)); }

            int chosenIndex = RandomNumberGenerator.GetInt32(list.Count);
            T chosenElement = list[chosenIndex];

            // now remove from the list
            list[chosenIndex] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);

            return chosenElement;
        }
    }
}
