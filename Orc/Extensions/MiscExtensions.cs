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

        public static IEnumerable<T> MergeOrderedCollections<T>(IEnumerable<T> orderedCollection1, IEnumerable<T> orderedCollection2) where T : IComparable<T>
        {
            var result = new List<T>();

            var orderedList1 = new List<T>();
            var orderedList2 = new List<T>();

            if(orderedCollection1 is List<T>)
            {
                orderedList1 = orderedCollection1 as List<T>;
            }
            else
            {
                orderedList1 = new List<T>(orderedCollection1);
            }


            if (orderedCollection2 is List<T>)
            {
                orderedList2 = orderedCollection2 as List<T>;
            }
            else
            {
                orderedList2 = new List<T>(orderedCollection2);
            }
            

            int i = 0;
            int j = 0;

            while (i < orderedList1.Count && j < orderedList2.Count)
            {
                if (orderedList1[i].CompareTo(orderedList2[j]) != 1)
                {
                    result.Add(orderedList1[i++]);
                }
                else
                {
                    result.Add(orderedList2[j++]);
                }
            }

            while (i < orderedList1.Count)
            {
                result.Add(orderedList1[i++]);
            }

            while (j < orderedList2.Count)
            {
                result.Add(orderedList2[j++]);
            }

            return result;
        }
    }
}