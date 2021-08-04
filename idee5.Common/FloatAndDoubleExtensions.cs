using System;

namespace idee5.Common {
    /// <summary>
    /// Extension methods for <see cref="float"/> and <see cref="double"/> types.
    /// </summary>
    public static class FloatAndDoubleExtensions {
        /// <summary>
        /// Check if two <see cref="float"/>s are nearly equal.
        /// </summary>
        /// <param name="left"><see cref="float"/> to be compared.</param>
        /// <param name="right">Compare with <see cref="float"/></param>
        /// <param name="epsilon">Allowed epsilon</param>
        /// <returns><see langword="true"/> if the difference between both <see cref="float"/> dosen't exceed the <paramref name="epsilon"/>.</returns>
        public static bool NearyEquals(this float left, float right, float epsilon) => Math.Abs(left - right) < epsilon;

        /// <summary>
        /// Check if <see cref="double"/>s are nearly equal.
        /// </summary>
        /// <param name="left"><see cref="double"/> to be compared.</param>
        /// <param name="right">Compare with <see cref="float"/></param>
        /// <param name="epsilon">Allowed epsilon</param>
        /// <returns><see langword="true"/> if the difference between both <see cref="double"/>s dosen't exceed the <paramref name="epsilon"/>.</returns>
        public static bool NearyEquals(this double left, double right, double epsilon) => Math.Abs(left - right) < epsilon;
    }
}