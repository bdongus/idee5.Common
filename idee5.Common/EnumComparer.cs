using System;
using System.Collections.Generic;
using System.Linq.Expressions;

// taken from https://www.codeproject.com/articles/33528/accelerating-enum-based-dictionaries-with-generic
namespace idee5.Common {
    public static class EnumComparer {
        public static EnumComparer<TEnum> For<TEnum>()
            where TEnum : struct, IComparable, IConvertible, IFormattable {
            return EnumComparer<TEnum>.Instance;
        }
    }

    /// <summary>
    /// A fast and efficient implementation of <see cref="IEqualityComparer{T}"/> for Enum types.
    /// Useful for dictionaries that use Enums as their keys.
    /// </summary>
    /// <example>
    /// <code>
    /// var dict = new Dictionary&lt;DayOfWeek, string&gt;(EnumComparer&lt;DayOfWeek&gt;.Instance);
    /// </code>
    /// </example>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    public sealed class EnumComparer<TEnum> : IEqualityComparer<TEnum> {
        //where TEnum : IComparable, IConvertible, IFormattable {
        private static readonly Lazy<Func<TEnum, TEnum, bool>> _equals = new Lazy<Func<TEnum, TEnum, bool>>(GenerateEquals);
        private static readonly Lazy<Func<TEnum, int>> _getHashCode = new Lazy<Func<TEnum, int>>(GenerateGetHashCode);

        /// <summary>
        /// The singleton accessor.
        /// </summary>
        private static readonly Lazy<EnumComparer<TEnum>> _instance = new Lazy<EnumComparer<TEnum>>(() => new EnumComparer<TEnum>());

        public static EnumComparer<TEnum> Instance {
            get { return _instance.Value; }
        }

        /// <summary>
        /// A private constructor to prevent user instantiation.
        /// </summary>
        private EnumComparer() {
            AssertTypeIsEnum();
            AssertUnderlyingTypeIsSupported();
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <typeparamref name="TEnum"/> to compare.</param>
        /// <param name="y">The second object of type <typeparamref name="TEnum"/> to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(TEnum x, TEnum y) {
            // call the generated method
            return _equals.Value(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        /// </exception>
        public int GetHashCode(TEnum obj) {
            // call the generated method
            return _getHashCode.Value(obj);
        }

        private static void AssertTypeIsEnum() {
            if (typeof(TEnum).IsEnum)
                return;

            string message =
                $"The type parameter {typeof(TEnum)} is not an Enum.";
            throw new NotSupportedException(message);
        }

        private static void AssertUnderlyingTypeIsSupported() {
            Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
            ICollection<Type> supportedTypes =
                new[]
                    {
                        typeof (byte), typeof (sbyte), typeof (short), typeof (ushort),
                        typeof (int), typeof (uint), typeof (long), typeof (ulong)
                    };

            if (supportedTypes.Contains(underlyingType))
                return;

            string message =
                $"The underlying type of the type parameter {typeof(TEnum)} is {underlyingType}. " +
                "This EnumComparer only supports Enums with underlying type of " +
                "byte, sbyte, short, ushort, int, uint, long, or ulong.";
            throw new NotSupportedException(message);
        }

        /// <summary>
        /// Generates a comparison method similiar to this:
        /// <code>
        /// bool Equals(TEnum x, TEnum y)
        /// {
        ///     return x == y;
        /// }
        /// </code>
        /// </summary>
        /// <returns>The generated method.</returns>
        private static Func<TEnum, TEnum, bool> GenerateEquals() {
            ParameterExpression xParam = Expression.Parameter(typeof(TEnum), "x");
            ParameterExpression yParam = Expression.Parameter(typeof(TEnum), "y");
            BinaryExpression equalExpression = Expression.Equal(xParam, yParam);
            ParameterExpression[] parameters = new[] { xParam, yParam };
            return Expression.Lambda<Func<TEnum, TEnum, bool>>(equalExpression, parameters).Compile();
        }

        /// <summary>
        /// Generates a GetHashCode method similar to this:
        /// <code>
        /// int GetHashCode(TEnum obj)
        /// {
        ///     return ((int)obj).GetHashCode();
        /// }
        /// </code>
        /// </summary>
        /// <returns>The generated method.</returns>
        private static Func<TEnum, int> GenerateGetHashCode() {
            ParameterExpression objParam = Expression.Parameter(typeof(TEnum), "obj");
            Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
            UnaryExpression convertExpression = Expression.Convert(objParam, underlyingType);
            System.Reflection.MethodInfo getHashCodeMethod = underlyingType.GetMethod("GetHashCode");
            MethodCallExpression getHashCodeExpression = Expression.Call(convertExpression, getHashCodeMethod);
            return Expression.Lambda<Func<TEnum, int>>(getHashCodeExpression, new[] { objParam }).Compile();
        }
    }
}
