using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Orcomp.Extensions
{
    public static class LibraryExtensions
    {
        public static void AddOrUpdate<TKey, TValue>( this IDictionary<TKey, TValue> dct, TKey key, TValue value )
        {
            if ( dct.ContainsKey( key ) )
            {
                dct[key] = value;
            }
            else
            {
                dct.Add( key, value );
            }
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

        /// <summary>
        /// Similar to .Foreach except that it works for all IEnumerable types
        /// and not just List and Arrays
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <param name="source">the source</param>
        /// <param name="action">the action</param>
        public static void Iter<T>( this IEnumerable<T> source, Action<T> action )
        {
            foreach ( T item in source )
            {
                action( item );
            }
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
                MessageBox.Show( str, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
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
            number = Math.Truncate( number );
            number = number / Math.Pow( 10, decimalPlaces );
            return string.Format( "{0:N" + Math.Abs( decimalPlaces ) + "}", number );
        }
    }
}