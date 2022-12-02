using System;
using System.Linq.Expressions;

namespace idee5.Common {
    /// <summary>
    /// Class to cast to type <typeparamref name="TDest"/>
    /// </summary>
    /// <typeparam name="TDest">Target type</typeparam>
    // taken from https://stackoverflow.com/a/23391746
    public static class CastTo<TDest> {
        /// <summary>
        /// Casts <typeparamref name="TSource"/> to <typeparamref name="TDest"/>.
        /// This does not cause boxing for value types.
        /// Useful in generic methods.
        /// </summary>
        /// <typeparam name="TSource">Source type to cast from. Usually a generic type.</typeparam>
        /// <param name="s">Object to cast.</param>
        public static TDest From<TSource>(TSource s) => Cache<TSource>.caster(s);

        private static class Cache<TSource> {
            public static readonly Func<TSource, TDest> caster = Get();

            private static Func<TSource, TDest> Get() {
                ParameterExpression p = Expression.Parameter(typeof(TSource));
                UnaryExpression c = Expression.ConvertChecked(p, typeof(TDest));
                return Expression.Lambda<Func<TSource, TDest>>(c, p).Compile();
            }
        }
    }
}