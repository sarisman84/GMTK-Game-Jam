using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public static class ColliderExtension
    {
        public static Heap<T> ToHeap<T>(this IEnumerable<T> collection) where T : IHeapItem<T>
        {
            var enumerable = collection as T[] ?? collection.ToArray();
            Heap<T> newHeap = new Heap<T>(enumerable.Length);

            foreach (var element in enumerable)
            {
                newHeap.Add(element);
            }

            return newHeap;
        }

        public static IEnumerable<A> ConvertTo<T, A>(this IEnumerable<T> collection, Func<T, A> callback)
        {
            List<A> result = new List<A>();
            foreach (var element in collection)
            {
                result.Add(callback.Invoke(element));
            }

            return result;
        }
    }
}