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
    using System;
    using System.Collections.Generic;

    public class Allocator<T, TResult>
    {
        public ICollection<TResult> Container { get; set; }
        public Predicate<T> Predicate { get; set; }
        public Func<T, TResult> Transform { get; set; }

        public Allocator(ICollection<TResult> container, Predicate<T> predicate, Func<T, TResult> transform)
        {
            Container = container;
            Predicate = predicate;
            Transform = transform;
        }
    }

    public class Allocator<T>
    {
        public ICollection<T> Container { get; set; }
        public Predicate<T> Predicate { get; set; }

        public Allocator(ICollection<T> container, Predicate<T> predicate)
        {
            Container = container;
            Predicate = predicate;
        }
    }
}