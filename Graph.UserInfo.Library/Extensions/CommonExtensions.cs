using System.Collections.Generic;

namespace UserInfo.Library.Extensions
{
    /// <summary>
    /// Common extensions.
    /// </summary>
    internal static class CommonExtensions
    {
        /// <summary>
        /// Devides a list in specified group of items.
        /// </summary>
        /// <typeparam name="T">object T.</typeparam>
        /// <param name="source">the list source.</param>
        /// <param name="count">the size of the group.</param>
        /// <returns>The list of object.</returns>
        internal static IEnumerable<IEnumerable<T>> MakeGroupsOf<T>(this IEnumerable<T> source, int count)
        {
            var grouping = new List<T>();
            foreach (var item in source)
            {
                grouping.Add(item);
                if (grouping.Count == count)
                {
                    yield return grouping;
                    grouping = new List<T>();
                }
            }

            if (grouping.Count != 0)
            {
                yield return grouping;
            }
        }
    }
}
