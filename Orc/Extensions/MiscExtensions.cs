namespace Orc.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
#if (SILVERLIGHT)
    using System.Windows;
#else
    using System.Windows.Forms;
#endif

    public static class MiscExtensions
    {
        public static void AddOrUpdate<TKey, TValue>( this IDictionary<TKey, TValue> dct, TKey key, TValue value )
        {
                dct[key] = value;
        }

        [DebuggerStepThrough]
        public static void BreakIfEqualTo<T>( this T source, T comparer )
        {
            if ( source.Equals( comparer ) )
            {
                Debugger.Break();
            }
        }

        public static string FlattenToString<T>( this IEnumerable<T> source, string separator )
        {
            if ( string.IsNullOrEmpty( separator ) )
            {
                separator = ",";
            }

            return string.Join( separator, source.Select( x => x.ToString() ).ToArray() );
        }

        public static void ShowMessage( this string str )
        {
            ShowMessage( str, true );
        }

        public static void ShowMessage( this string str, bool? enabled )
        {
            if ( enabled.GetValueOrDefault( true ) )
            {
                ShowMessage( str, true );
            }
        }

        public static void ShowMessage( this string str, bool enabled )
        {
            if ( enabled )
            {
                MessageBox.Show( str );
            }
        }

        public static void ShowWarning( this string str )
        {
            ShowWarning( str, true );
        }

        public static void ShowWarning( this string str, bool? enabled )
        {
            if ( enabled.GetValueOrDefault( true ) )
            {
                ShowWarning( str, true );
            }
        }

        public static void ShowWarning( this string str, bool enabled )
        {
            if ( enabled )
            {
#if (SILVERLIGHT)
                MessageBox.Show(str, "Warning", MessageBoxButton.OK);
#else
                MessageBox.Show( str, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
#endif
            }
        }

        /// <summary>
        /// Check that the collection only has one item, otherwise throw an exception.
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <param name="source">IEnumerable source</param>
        /// <param name="info">info string</param>
        /// <returns>single value</returns>
        public static T Singular<T>( this IEnumerable<T> source, string info )
        {
            if ( source != null && source.Count() > 1 )
            {
                throw new ArgumentException( "There are more than one returned values. " + info );
            }

            if ( source != null && !source.Any() )
            {
                throw new ArgumentException( "There are no items in the collection. " + info );
            }

            return source.Single();
        }

        public static string ToStringDecimal( this double number, int decimalPlaces )
        {
            number = number * Math.Pow( 10, decimalPlaces );
#if (SILVERLIGHT)
            number = (number >= 0 ? Math.Floor(number) : -Math.Floor(-number));
#else
            number = Math.Truncate( number );
#endif
            number = number / Math.Pow( 10, decimalPlaces );
            return string.Format( "{0:N" + Math.Abs( decimalPlaces ) + "}", number );
        }

        /// <summary>
        /// This function will take 2 ordered collections and merge them together, preserving the order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderedCollection1"></param>
        /// <param name="orderedCollection2"></param>
        /// <param name="comparer">Optional comparer for T</param>
        /// <returns></returns>
        public static IEnumerable<T> MergeOrderedCollections<T>(IEnumerable<T> orderedCollection1, IEnumerable<T> orderedCollection2, IComparer<T> comparer=null)
            where T : IComparable<T>
        {
            var enumerator1 = orderedCollection1.GetEnumerator();
            var enumerator2 = orderedCollection2.GetEnumerator();

            var hasNext1 = enumerator1.MoveNext();
            var hasNext2 = enumerator2.MoveNext();

            if (comparer == null)
            {
                comparer = Comparer<T>.Default;
            }

            while (hasNext1 && hasNext2)
            {
                if (comparer.Compare(enumerator1.Current, enumerator2.Current) <= 0)
                {
                    yield return enumerator1.Current;
                    hasNext1 = enumerator1.MoveNext();
                }
                else
                {
                    yield return enumerator2.Current;
                    hasNext2 = enumerator2.MoveNext();
                }
            }

            while (hasNext1)
            {
                yield return enumerator1.Current;
                hasNext1 = enumerator1.MoveNext();
            }

            while (hasNext2)
            {
                yield return enumerator2.Current;
                hasNext2 = enumerator2.MoveNext();
            }
        }
    }
}