using System;
using System.Globalization;

namespace idee5.Common {
    /// <summary>
    /// Base ambient context class.
    /// </summary>
    /// <remarks>Taken from https://github.com/nrjohnstone/AmbientContext.
    /// Inspired by http://blog.ploeh.dk/2010/04/27/FunwithliteralextensionsandAmbientContext/
    /// </remarks>
    /// <typeparam name="T">Ambient service type.</typeparam>
    public abstract class AmbientService<T> where T : class {
        /// <summary>
        /// Delegate creating a <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A new instance to <typeparamref name="T"/>.</returns>
        public delegate T CreateDelegate();

        private T _instance;

        /// <summary>
        /// Alternative instance creator.
        /// </summary>
        public CreateDelegate Create { get; set; }

        /// <summary>
        /// Default instance creator.
        /// </summary>
        /// <returns>The default implementation.</returns>
        protected virtual T DefaultCreate() => null;

        /// <summary>
        /// Instance within the ambient context.
        /// </summary>
        public T Instance {
            get {
                if (_instance == null) {
                    if (Create != null) _instance = Create();
                    if (_instance == null) {
                        _instance = DefaultCreate();
                        if (_instance == null) {
                            NoCreate();
                        }
                    }
                }
                return _instance;
            }
            set {
                _instance = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        private static T NoCreate() {
            string message = String.Format(CultureInfo.InvariantCulture, Properties.Resources.AmbientServiceNotConfigured, typeof(T).Name);
            throw new Exception(message);
        }
    }
}