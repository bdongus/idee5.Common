using idee5.Common.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace idee5.Common;
/// <summary>
/// <see cref="IEnumerable{T}"/> extension methods.
/// </summary>
public static class EnumerableExtensions {
    #region Public Methods

    /// <summary>
    /// Splits an enumerable into chunks of a specified size.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="list">The unchunked list.</param>
    /// <param name="chunkSize">Size of chunks to return.</param>
    /// <returns>List of chunks.</returns>
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> list, int chunkSize) {
        if (chunkSize <= 0) {
            throw new ArgumentOutOfRangeException(nameof(chunkSize), Resources.MustBeGreaterThanZero);
        }

#pragma warning disable RCS1227 // Validate arguments correctly.
        while (list.Any()) {
#pragma warning restore RCS1227 // Validate arguments correctly.
            yield return list.Take(chunkSize);
            list = list.Skip(chunkSize);
        }
    }

    /// <summary>
    /// Compare two collections.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="other"></param>
    /// <returns><c>true</c> if all items in the other collection exist in this collection</returns>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
    public static bool ContainsAll<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> other) {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        bool matches = true;
        foreach (TSource i in other) {
            matches = source.Contains(i);
            if (!matches)
                break;
        }
        return matches;
    }

    /// <summary>
    /// Returns true if the source contains any of the items in the other list
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool ContainsAny<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> other) => other.Any(i => source.Contains(i));

    /// <summary>
    /// Provides a Distinct method that takes a key selector lambda as parameter. The .net
    /// framework only provides a Distinct method that takes an instance of an implementation of
    /// <see cref="IEqualityComparer{T}"/> where the standard parameterless Distinct that uses
    /// the default equality comparer doesn't suffice.
    /// </summary>
    /// <typeparam name="T">The type of the results.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="source">The this.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>List of results.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is <c>null</c>.</exception>
    public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));
        return source.GroupBy(keySelector).Select(grps => grps).Select(e => e.First());
    }

    /// <summary>
    /// Execute a function to each element.
    /// </summary>
    /// <typeparam name="T">The type being enumerated.</typeparam>
    /// <param name="source">The enumerator.</param>
    /// <param name="func">The function to apply.</param>
    /// <exception cref="ArgumentNullException">Thrown if argument is null.</exception>
    /// <remarks>
    /// This extension on <see cref="IEnumerable{T}"/> that fully consumes a stream while
    /// applying a function to each element:
    /// <code>
    /// IEnumerable&lt;int&gt; source = new[] { 2, 99, 8 };
    /// source.Execute(Console.WriteLine);
    /// // output:
    /// // 2
    /// // 99
    /// // 8
    /// </code>
    /// </remarks>
    public static IEnumerable<T> Execute<T>(this IEnumerable<T> source, Action<T> func) {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (func == null)
            throw new ArgumentNullException(nameof(func));

        return TiggertImmediateValidation();

        IEnumerable<T> TiggertImmediateValidation() {
            foreach (T t in source) {
                func(t);
                yield return t;
            }
        }
    }

    /// <summary>
    /// Finds the index of the first item matching an expression in an enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the enumerated objects.</typeparam>
    /// <param name="items">The enumerable to search.</param>
    /// <param name="predicate">The expression to test the items against.</param>
    /// <returns>The index of the first matching item, or -1.</returns>
    public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate) => FindIndex(items, startIndex: 0, predicate: predicate);

    /// <summary>
    /// Finds the index of the first item matching an expression in an enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the enumerated objects.</typeparam>
    /// <param name="items">The enumerable to search.</param>
    /// <param name="startIndex">The index to start at.</param>
    /// <param name="predicate">The expression to test the items against.</param>
    /// <returns>The index of the first matching item, or -1.</returns>
    public static int FindIndex<T>(this IEnumerable<T> items, int startIndex, Func<T, bool> predicate) {
        if (items == null)
            throw new ArgumentNullException(nameof(items));
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        int index = startIndex;
        if (index > 0)
            items = items.Skip(index);

        foreach (T item in items) {
            if (predicate(item))
                return index;
            index++;
        }

        return -1;
    }

    /// <summary>
    /// The flatten list.
    /// </summary>
    /// <param name="e">The items.</param>
    /// <param name="f">The select child.</param>
    /// <typeparam name="T">Item type</typeparam>
    /// <returns>list of TItem</returns>
    public static IEnumerable<T> FlattenList<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f) => e.SelectMany(c => f(c).FlattenList(f)).Concat(e);

    /// <summary>
    /// https://github.com/umbraco/Umbraco-CMS/blob/user-group-permissions/src/Umbraco.Core/EnumerableExtensions.cs/#L212
    /// </summary>
    /// <typeparam name="TBase">The base type.</typeparam>
    /// <typeparam name="TActual"></typeparam>
    /// <param name="source"></param>
    /// <param name="projection"></param>
    /// <returns></returns>
    public static IEnumerable<TBase> ForAllThatAre<TBase, TActual>(this IEnumerable<TBase> source, Action<TActual> projection) where TActual : class => source.Select(x => {
        // use GetTypeInfo because IsAssignableFrom in CPL projects is only available on TypeInfo
        if (typeof(TActual).GetTypeInfo().IsAssignableFrom(x.GetType().GetTypeInfo())) {
            var casted = x as TActual;
            projection.Invoke(casted);
        }
        return x;
    });

    /// <summary>
    /// Execute an operation for each element.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="func">The func.</param>
    /// <typeparam name="TItem">item type</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <returns>the Results</returns>
    public static TResult[] ForEach<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, TResult> func) => items.Select(func).ToArray();

    /// <summary>
    /// Execute an <see cref="Action"/> for each element.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="action">The action.</param>
    /// <typeparam name="TItem">Item type</typeparam>
    /// <returns>list of TItem</returns>
    public static IEnumerable<TItem> ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem> action) {
        if (items != null && action != null) {
            foreach (TItem item in items) { action(item); }
        }

        return items;
    }

    /// <summary>
    /// Formats each element of the stream with the given separator between them.
    /// </summary>
    /// <typeparam name="T">The type being enumerated.</typeparam>
    /// <param name="source">The input stream to format.</param>
    /// <param name="separator">The element separating each element of the stream.</param>
    /// <returns>A formatted string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if argument is null.</exception>
    /// <remarks>
    /// Format is a set of extension method overloads on <see cref="IEnumerable{T}"/> that make
    /// it easy to construct a string from a sequence. Basically, you provide a sequence and a
    /// value separator, like ",", and Format will output each value in the sequence spaced with
    /// the separator:
    /// <code>
    /// IEnumerable&lt;int&gt; source = new[] { 2, 99, 8 };
    /// string formatted = source.Format(", ");
    /// Console.WriteLine(formatted);
    /// // output:
    /// // 2, 99, 8
    /// </code>
    /// </remarks>
    public static string Format<T>(this IEnumerable<T> source, string separator) {
        var sb = new StringBuilder();
        Format(source, separator, sb);
        return sb.Length == 0 ? string.Empty : sb.ToString(startIndex: 0, length: sb.Length - separator.Length);
    }

    /// <summary>
    /// Formats each element of the enumerable with the given separator between them.
    /// </summary>
    /// <typeparam name="T">The type being enumerated.</typeparam>
    /// <param name="source">The input enumerable to format.</param>
    /// <param name="separator">The element separating each element of the stream.</param>
    /// <param name="toString">The function used to convert each element to a string.</param>
    /// <returns>A formatted string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if argument is null.</exception>
    /// <remarks>
    /// Format is a set of extension method overloads on <see cref="IEnumerable{T}"/> that make
    /// it easy to construct a string from a sequence. Basically, you provide a sequence and a
    /// value separator, like ",", and Format will output each value in the sequence spaced with
    /// the separator:
    /// <code>
    /// IEnumerable&lt;int&gt; source = new[] { 2, 99, 8 };
    /// string formatted = source.Format(", ", s =&gt; String.Format(format: "\"{0}\"", arg0: s));
    /// Console.WriteLine(formatted);
    /// // output:
    /// // "2", "99", "8"
    /// </code>
    /// </remarks>
    public static string Format<T>(this IEnumerable<T> source, string separator, Func<T, string> toString) {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (separator == null)
            throw new ArgumentNullException(nameof(separator));
        if (toString == null)
            throw new ArgumentNullException(nameof(toString));
        var sb = new StringBuilder();
        Format(source, separator, sb, toString);
        return sb.Length == 0 ? string.Empty : sb.ToString(startIndex: 0, length: sb.Length - separator.Length);
    }

    /// <summary>
    /// Formats each element of the stream with the given separator between them.
    /// </summary>
    /// <typeparam name="T">The type being enumerated.</typeparam>
    /// <param name="source">The input stream to format.</param>
    /// <param name="separator">The element separating each element of the stream.</param>
    /// <param name="output">The StringBuilder to which the output is written.</param>
    /// <exception cref="ArgumentNullException">Thrown if argument is null.</exception>
    /// <remarks>
    /// Format is a set of extension method overloads on <see cref="IEnumerable{T}"/> that make
    /// it easy to construct a string from a sequence. Basically, you provide a sequence and a
    /// value separator, like ",", and Format will output each value in the sequence spaced with
    /// the separator:
    /// <code>
    /// IEnumerable&lt;int&gt; source = new[] { 2, 99, 8 };
    /// string formatted = source.Format(", ");
    /// Console.WriteLine(formatted);
    /// // output:
    /// // 2, 99, 8
    /// </code>
    /// </remarks>
    public static void Format<T>(this IEnumerable<T> source, string separator, StringBuilder output) {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (separator == null)
            throw new ArgumentNullException(nameof(separator));
        if (output == null)
            throw new ArgumentNullException(nameof(output));
        Format(source, separator, output, s => s.ToString());
    }

    /// <summary>
    /// Formats each element of the stream with the given separator between them.
    /// </summary>
    /// <typeparam name="T">The type being enumerated.</typeparam>
    /// <param name="source">The input stream to format.</param>
    /// <param name="separator">The element separating each element of the stream.</param>
    /// <param name="output">The StringBuilder to which the output is written.</param>
    /// <param name="toString">The function used to convert each element to a string.</param>
    /// <exception cref="ArgumentNullException">Thrown if argument is null.</exception>
    /// <remarks>
    /// Format is a set of extension method overloads on <see cref="IEnumerable{T}"/> that make
    /// it easy to construct a string from a sequence. Basically, you provide a sequence and a
    /// value separator, like ",", and Format will output each value in the sequence spaced with
    /// the separator:
    /// <code>
    /// IEnumerable&lt;int&gt; source = new[] { 2, 99, 8 };
    /// var sb = new StringBuilder();
    /// string formatted = source.Format(", ", sb, s =&gt; String.Format(format: "\"{0}\"", arg0: s));
    /// Console.WriteLine(formatted);
    /// // output:
    /// // "2", "99", "8"
    /// </code>
    /// </remarks>
    public static void Format<T>(this IEnumerable<T> source, string separator, StringBuilder output, Func<T, string> toString) {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (output == null)
            throw new ArgumentNullException(nameof(output));
        if (toString == null)
            throw new ArgumentNullException(nameof(toString));
        foreach (T t in source) {
            string value = t is ValueType || t != null ? toString(t) : string.Empty;
            output.Append(value);
            output.Append(separator);
        }
    }

    /// <summary>
    /// Execute an <see cref="Action"/> on the item in <see cref="IEnumerable{T}"/> if it is not null.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="action">The action to execute.</param>
    /// <typeparam name="TItem">The item type.</typeparam>
    public static void IfNotNull<TItem>(this IEnumerable<TItem> items, Action<TItem> action) where TItem : class {
        if (items != null) {
            foreach (TItem item in items) { item.IfNotNull(action); }
        }
    }

    ///<summary>Finds the index of the first occurence of an item in an enumerable.</summary>
    ///<param name="items">The enumerable to search.</param>
    ///<param name="item">The item to find.</param>
    ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
    public static int IndexOf<T>(this IEnumerable<T> items, T item) => items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));

    /// <summary>
    /// Build groups of <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type contained in the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="source">This instance</param>
    /// <param name="groupSize">Number of entries in each group.</param>
    /// <returns>List of partitions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="groupSize"/> is less or equal 0.</exception>
    public static IEnumerable<IEnumerable<T>> InGroupsOf<T>(this IEnumerable<T> source, int groupSize) {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (groupSize <= 0)
            throw new ArgumentException(Resources.MustBeGreaterThanZero, nameof(groupSize));
        return source.Select((x, i) => Tuple.Create(i / groupSize, x))
            .GroupBy(t => t.Item1, t => t.Item2);
    }

    /// <summary>
    /// Checks wether the given <see cref="IEnumerable{T}"/> object is null or has no item.
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source?.Any() != true;

    /// <summary>
    /// Concatenates the members of a constructed <see cref="IEnumerable{T}"/> collection of type System.String, using the specified separator between each member.
    /// This is a shortcut for string.Join(...)
    /// </summary>
    /// <param name="source">A collection that contains the strings to concatenate.</param>
    /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
    /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns System.String.Empty.</returns>
    public static string JoinAsString(this IEnumerable<string> source, string separator) => string.Join(separator, source);

    /// <summary>
    /// Removes all matching items from an <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    /// <param name="predicate">The predicate.</param>
    public static void RemoveAll<T>(this IList<T> list, Func<T, bool> predicate) {
        if (list != null && predicate != null) {
            for (int i = 0; i < list.Count; i++) {
                if (predicate(list[i])) { list.RemoveAt(i--); }
            }
        }
    }

    /// <summary>
    /// Removes all matching items from an <see cref="ICollection{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    /// <param name="predicate">The predicate.</param>
    public static void RemoveAll<T>(this ICollection<T> list, Func<T, bool> predicate) {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        foreach (T match in list.Where(predicate).ToArray()) { list.Remove(match); }
    }

    /// <summary>
    /// Navigate homogenous hierarchies.
    /// </summary>
    /// <remarks>http://work.j832.com/2008/01/selectrecursive-if-3rd-times-charm-4th.html</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerables"></param>
    /// <param name="recursiveSelector"></param>
    /// <param name="maxRecusionDepth"></param>
    /// <exception cref="InvalidOperationException">Thrown when the maximum recursion depth is reached.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="enumerables"/> is <c>null</c>.</exception>
    public static IEnumerable<T> SelectRecursive<T>(
        this IEnumerable<T> enumerables,
        Func<T, IEnumerable<T>> recursiveSelector, int maxRecusionDepth = 100) {
        if (enumerables != null && recursiveSelector != null)
            return TriggerImmediateValidation();
        return enumerables;

        IEnumerable<T> TriggerImmediateValidation() {
            var stack = new Stack<IEnumerator<T>>();
            stack.Push(enumerables.GetEnumerator());

            try {
                while (stack.Count > 0) {
                    if (stack.Count > maxRecusionDepth)
                        throw new InvalidOperationException("Maximum recursion depth reached of " + maxRecusionDepth.ToString(CultureInfo.CurrentCulture));

                    if (stack.Peek().MoveNext()) {
                        T current = stack.Peek().Current;

                        yield return current;

                        stack.Push(recursiveSelector(current).GetEnumerator());
                    } else { stack.Pop().Dispose(); }
                }
            }
            finally {
                while (stack.Count > 0) { stack.Pop().Dispose(); }
            }
        }
    }

    /// <summary>
    /// Converts an enumerable to a RFC4180 CSV string.
    /// Uses the <see cref="Object.ToString"/> method to support custom types.
    /// </summary>
    /// <typeparam name="T">The type being enumerated.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="quotationMark">The quotation mark.</param>
    /// <returns>The formatted string.</returns>
    public static string ToCsv<T>(this IEnumerable<T> source, string separator = ",", string quotationMark = "\"")
        => source.Format(separator, s => quotationMark + s.ToString() + quotationMark);

    /// <summary>
    /// Converts an IEnumerable to DataTable (supports nullable types)
    /// </summary>
    /// <typeparam name="T">The type of the T.</typeparam>
    /// <param name="varlist">The varlist.</param>
    /// <returns>The resulting <see cref="DataTable"/>. First row contains the property names.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="varlist"/> is <c>null</c>.</exception>
    public static DataTable ToDataTable<T>(this IEnumerable<T> varlist) {
        var dtReturn = new DataTable();

        if (varlist != null) {
            // column names
            PropertyInfo[] oProps = null;

            foreach (T rec in varlist) {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null) {
                    oProps = rec.GetType().GetProperties();
                    for (int i2 = 0; i2 < oProps.Length; i2++) {
                        Type colType = oProps[i2].PropertyType;

                        if (colType.IsGenericType && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                            colType = colType.GetGenericArguments()[0];

                        dtReturn.Columns.Add(new DataColumn(oProps[i2].Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                for (int i3 = 0; i3 < oProps.Length; i3++)
                    dr[oProps[i3].Name] = oProps[i3].GetValue(rec, index: null) ?? DBNull.Value;

                dtReturn.Rows.Add(dr);
            }
        }
        return dtReturn;
    }

    /// <summary>
    /// Converts an enumeration of groupings into a Dictionary of those groupings.
    /// </summary>
    /// <typeparam name="TKey">Key type of the grouping and dictionary.</typeparam>
    /// <typeparam name="TValue">Element type of the grouping and dictionary list.</typeparam>
    /// <param name="groupings">The enumeration of groupings from a GroupBy() clause.</param>
    /// <returns>
    /// A dictionary of groupings such that the key of the dictionary is TKey type and the value
    /// is List of TValue type.
    /// </returns>
    public static Dictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groupings) => groupings.ToDictionary(group => group.Key, group => group.ToList());

    /// <summary>
    /// Transposes the rows and columns of its argument
    /// </summary>
    /// <typeparam name="T">The type of the T.</typeparam>
    /// <param name="source">The source array.</param>
    /// <returns>Transposed two-dimensional matrix.</returns>
    public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> source) {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        return source.Select(a => a.Select(b => Enumerable.Repeat(b, count: 1))).DefaultIfEmpty(Enumerable.Empty<IEnumerable<T>>())
                   .Aggregate((a, b) => a.Zip(b, (f, g) => f.Concat(g)));
    }

    /// <summary>
    /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="source">Enumerable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the enumerable</param>
    /// <returns>Filtered or not filtered enumerable based on <paramref name="condition"/></returns>
    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate) => condition ? source.Where(predicate) : source;

    /// <summary>
    /// Filters a sequence of values to ignore those which are null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="coll">The coll.</param>
    /// <returns></returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> coll) where T : class { return coll.Where(x => x != null); }

    #endregion Public Methods
}