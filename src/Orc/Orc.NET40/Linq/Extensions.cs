// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="IDateInterval.cs" company="ORC">
// //   MS-PL
// // </copyright>
// // <summary>
// //  
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace Orc.Linq
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Extensions
    {
        public static void Allocate<T>(this IEnumerable<T> items, IEnumerable<Allocator<T>> allocators)
        {
            var _allocators = allocators as Allocator<T>[] ?? allocators.ToArray();

            foreach (var item in items)
            {
                foreach (var allocator in _allocators)
                {
                    if (allocator.Predicate(item))
                    {
                        allocator.Container.Add(item);
                    }
                }
            }
        }

        public static void Allocate<T, TResult>(this IEnumerable<T> items, IEnumerable<Allocator<T, TResult>> allocators)
        {
            var _allocators = allocators as Allocator<T, TResult>[] ?? allocators.ToArray();

            foreach (var item in items)
            {
                foreach (var allocator in _allocators)
                {
                    if (allocator.Predicate(item))
                    {
                        allocator.Container.Add(allocator.Transform(item));
                    }
                }
            }
        }
    }
}