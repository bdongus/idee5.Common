using System;
using System.Collections.Generic;
using System.Globalization;

namespace idee5.Common {
    /// <summary>
    /// Generic helper class for the <see cref="Enum"/> class.
    /// Provides faster solutions for already existing functionalities in the <see cref="Enum"/> class along with
    /// some additional functionality.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
    public static class EnumUtils<TEnum> where TEnum : struct, IComparable, IConvertible, IFormattable {
        #region Private Fields

        private static readonly Type _enumType = typeof(TEnum);
        private static readonly bool _isSigned = new TypeCode[] { TypeCode.SByte, TypeCode.Int16, TypeCode.Int32, TypeCode.Int64 }.IndexOf(_typeCode) > -1;
#pragma warning disable HAA0502 // Explicit new reference type allocation
        private static readonly object _syncRoot = new object();
#pragma warning restore HAA0502 // Explicit new reference type allocation
        private static readonly TypeCode _typeCode = Type.GetTypeCode(Enum.GetUnderlyingType(_enumType));

        // locks are used so that multiple threads may assign a field multiple times but it is still faster than locking fields even on non-null access

        private static Dictionary<int, string> _intNamePairs;
        private static string[] _names;
        private static Dictionary<TEnum, string> _valueNamePairs;
        private static TEnum[] _values;

        #endregion Private Fields

        #region Public Properties

#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        /// Count the members of an enum.
        /// </summary>
        /// <returns>Number of members.</returns>
        public static int Count => Names.Length;
#pragma warning restore CA1000 // Do not declare static members on generic types

#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        /// Create a dictionary for an enum.
        /// <c>DayOfWeek.Monday.ToDictionary</c>
        /// </summary>
        /// <returns>A dictionary with the integer values as keys and the names as values.</returns>
        public static Dictionary<int, string> ToDictionary {
#pragma warning restore CA1000 // Do not declare static members on generic types
            get {
                // avoid locking if the cache is filled
                if (_intNamePairs == null) {
                    // lock and maybe wait for the release
                    lock (_syncRoot) {
                        // check again after the lock/wait
                        if (_intNamePairs == null) {
                            // Names is used to ensure _names is filled
                            _intNamePairs = new Dictionary<int, string>(Names.Length);
                            // Values is used to ensure _values is filled
                            for (int i = 0; i < Values.Length; i++) {
                                int key = CastTo<int>.From(_values[i]);
                                if (!_intNamePairs.ContainsKey(key))
                                    _intNamePairs.Add(key, _names[i]);
                            }
                        }
                    }
                }
                // as we are rechecking the cahce, we can have the return outside the lock
                return _intNamePairs;
            }
        }

        #endregion Public Properties

        #region Private Properties

        private static string[] Names {
            get {
                if (_names == null) {
                    lock (_syncRoot) {
                        if (_names == null) _names = Enum.GetNames(_enumType);
                    }
                }
                return _names;
            }
        }

        private static Dictionary<TEnum, string> ValueNamePairs {
            get {
                // avoid locking if the cache is filled
                if (_valueNamePairs == null) {
                    // lock and maybe wait for the release
                    lock (_syncRoot) {
                        // check again after the lock/wait
                        if (_valueNamePairs == null) {
                            IEqualityComparer<TEnum> comparer = _typeCode == TypeCode.Int32
                                ? (IEqualityComparer<TEnum>) EqualityComparer<TEnum>.Default
                                : EnumComparer<TEnum>.Instance;
                            _valueNamePairs = new Dictionary<TEnum, string>(Names.Length, comparer);
                            for (int i = 0; i < Values.Length; i++) {
                                // avoiding duplicated keys (multiple names for the same value)
                                if (!_valueNamePairs.ContainsKey(_values[i]))
                                    _valueNamePairs.Add(_values[i], _names[i]);
                            }
                        }
                    }
                }
                return _valueNamePairs;
            }
        }

        private static TEnum[] Values {
            get {
                if (_values == null) {
                    lock (_syncRoot) {
                        if (_values == null) _values = (TEnum[]) Enum.GetValues(_enumType);
                    }
                }
                return _values;
            }
        }

        #endregion Private Properties

        #region Public Methods

#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        /// Clears caches associated with <typeparamref name="TEnum"/> enumeration.
        /// </summary>
        public static void ClearCaches() {
#pragma warning restore CA1000 // Do not declare static members on generic types
            lock (_syncRoot) {
                _values = null;
                _names = null;
                _valueNamePairs = null;
                _intNamePairs = null;
            }
        }

#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        /// Returns the <see cref="string"/> representation of the given <see langword="enum"/>&#160;value specified in the <paramref name="value"/> parameter.
        /// </summary>
        /// <param name="value">A <typeparamref name="TEnum"/> value that has to be converted to <see cref="string"/>.</param>
        /// <returns>The string representation of <paramref name="value"/>.</returns>
        public static string ToString(TEnum value) {
#pragma warning restore CA1000 // Do not declare static members on generic types
            // defined value exists
            if (ValueNamePairs.TryGetValue(value, out string name))
                return name;
            else return _isSigned ? value.ToInt64(null).ToString(CultureInfo.InvariantCulture) : value.ToUInt64(null).ToString(CultureInfo.InvariantCulture);
        }

        #endregion Public Methods
    }
}